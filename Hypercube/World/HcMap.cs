﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.World {
    public delegate void BlockchangeArgs(Vector3S location, byte type);
    public delegate void MapEventArgs(HcMap map);

    public class HcMap : TaskItem {
        public static HcMap DefaultMap { get; set; } // -- static main map
        public static Dictionary<string, HcMap> Maps { get; set; } // -- static list of all loaded maps.

        internal string Filename; // -- this maps physical location
        //internal Classicworld CwMap; // -- this map's format loader..
        internal IMapProvider MapProvider;

        private Stack<byte> _entityStack; // -- for entity IDs
        internal bool Loaded = true; // -- state flag
        private DateTime _lastClient; // -- used for loading/unloading..
        //private HypercubeMetadata _metadata; // -- CW meta

        // -- permissions..
        public short BuildRank, Showrank, Joinrank;
        
        // -- Events Generated.

        public event BlockchangeArgs BlockChanged;
        public event EntityEventArgs EntityCreated, EntityDestroyed;
        public event MapEventArgs Saved, Resized, MapLoaded, MapUnloaded;
        public event StringEventArgs MapChatSent;
        #region Constructors

        /// <summary>
        /// Create a new map
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="mapName"></param>
        /// <param name="size"></param>
        public HcMap(string filename, string mapName, Vector3S size) {
            // -- When creating a new map, we will always use the classic world format.
            MapProvider = new ClassicWorldMapProvider();
            MapProvider.CreateNew(size, filename, mapName);

            BuildRank = 0;
            Showrank = 0;
            Joinrank = 0;

            Filename = filename;
            Save(filename);

            Interval = new TimeSpan(0, 1, 0);
            TaskScheduler.RegisterTask($"Map save ({mapName})", this);
            LoadStack();
            Loaded = true;
            _lastClient = DateTime.UtcNow;
        }

        /// <summary>
        /// Load an existing map
        /// </summary>
        /// <param name="filename"></param>
        public HcMap(string filename) {
            //_metadata = new HypercubeMetadata();
            if (filename.EndsWith(".cw") || filename.EndsWith(".cwu"))
                MapProvider = new ClassicWorldMapProvider();
            else
                MapProvider = new D3MapProvider();

            Load(filename);
            Filename = filename;

            Interval = new TimeSpan(0, 1, 0);
            TaskScheduler.RegisterTask($"Map save ({MapProvider.MapName})", this);
            LoadStack();
            
            _lastClient = DateTime.UtcNow;
        }

        #endregion

        #region implemented abstract members of TaskItem

        public override void Setup() {
        }

        public override void Main() {
            if (Loaded) {
                Save(Filename);

                bool clientCount = Entity.AllEntities.All(a => a.CurrentMap != this);

                if ((DateTime.UtcNow - _lastClient).TotalSeconds >= 45 && clientCount) {
                    Unload();
                } else if (!clientCount)
                    _lastClient = DateTime.UtcNow;
            }

            LastRun = DateTime.UtcNow;
        }

        public override void Teardown() {
            Save(Filename);
        }

        #endregion
        #region Entity Management
        private void LoadStack() {
            _entityStack = new Stack<byte>();
            for (byte i = 127; i > 0; i--) {
                _entityStack.Push(i);
            }
        }

        public byte GetEntityId() {
            return _entityStack.Pop();
        }

        public void ReturnEntityId(byte id) {
            _entityStack.Push(id);
        }

        public void EntityAdd(Entity e) {
            EntityCreated?.Invoke(e);
        }

        public void EntityRemove(Entity e) {
            EntityDestroyed?.Invoke(e);
        }
        #endregion
        #region Block Placement

        public Vector3S GetSize() {
            return MapProvider.GetSize();
        }
        /// <summary>
        /// Sets all the blocks in the map
        /// </summary>
        /// <param name="blockdata"></param>
        public void SetMap(byte[] blockdata) {
            if (!Loaded) {
                Logger.Log(LogType.Warning, "Attempted to fill an unloaded map.");
                return;
            }
            MapProvider.SetBlocks(blockdata);
            //sCwMap.BlockData = blockdata;
        }
        /// <summary>
        /// Gets the ID of a block at the given location.
        /// </summary>
        /// <param name="x">X Location of the block to retreive.</param>
        /// <param name="z">Y Location of the block to retreive.</param>
        /// <param name="y">Z Location of the block to retreive.</param>
        /// <returns>The blocktype for this location.</returns>
        public byte GetBlockId(short x, short y, short z) {
            if (!BlockInBounds(x, y, z))
                return 254;

            return !Loaded ? (byte)254 : MapProvider.GetBlock(x, y, z);

            //int index = (y * CwMap.SizeZ + z) * CwMap.SizeX + x;
            //return CwMap.BlockData[index];
        }
        public byte GetBlockId(Vector3S location) {
            return GetBlockId(location.X, location.Y, location.Z);
        }
        /// <summary>
        /// Sets the block at the given location, and optionally records map history.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        public void SetBlockId(short x, short y, short z, byte type) {
            if (!BlockInBounds(x, y, z))
                return;

            if (!Loaded)
                return;

            //int index = (y * CwMap.SizeZ + z) * CwMap.SizeX + x;

            //CwMap.BlockData[index] = type;
            MapProvider.SetBlock(x, y, z, type);
            BlockChanged?.Invoke(new Vector3S(x, y, z), type);
        }

        /// <summary>
        /// Determines if the given coordinates are in-bounds of the map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool BlockInBounds(short x, short y, short z) {
            Vector3S mapSize = MapProvider.GetSize();
            bool result1 = (0 <= x && (mapSize.X - 1) >= x) && (0 <= z && (mapSize.Z - 1) >= z);
            bool result2 = result1 && (0 <= y && (mapSize.Y - 1) >= y);
            return result2;
        }
        #endregion
        #region Map Management
        public void InvokeMapChat(string message) { MapChatSent?.Invoke(message); }

        public MinecraftLocation GetSpawn() {
            var spawnLoc = MapProvider.GetSpawn();
            return spawnLoc;
        }

        public void SetSpawn(Vector3S location, byte look, byte rot)
        {
            var spawnLoc = new MinecraftLocation(location, rot, look);
            spawnLoc.SetAsBlockCoords(location);
            MapProvider.SetSpawn(spawnLoc);
        }

        public void Delete() {
            List<Entity> clientsToMove = Entity.AllEntities.Where(a => a.CurrentMap == this && a.AssoClient != null).ToList();

            foreach (Entity client in clientsToMove) {
                client.AssoClient.ClientPlayer.ChangeMap(DefaultMap);
                Chat.SendClientChat("§SThe map you were on was deleted.", 0, client.AssoClient);
            }

            TaskScheduler.UnregisterTask($"Map save ({MapProvider.MapName})");

            Maps.Remove(MapProvider.MapName);
            File.Delete(Filename);
            MapProvider.Unload();
            _entityStack.Clear();
            MapProvider = null;

        }

        public void Resize(Vector3S size) {
            var unload = false;

            if (!Loaded) {
                Reload();
                unload = true;
            }

            MapProvider.SetSize(size);
            Resized?.Invoke(this);

            if (unload)
                Unload();
            else
                Resend();
        }

        private void Unload() {
            if (!Loaded)
                return;

            Save(Filename);
            MapProvider.Unload();
            GC.Collect();
            Loaded = false;
            Logger.Log(LogType.Info, $"Map '{MapProvider.MapName}' unloaded.");
            MapUnloaded?.Invoke(this);
        }

        private void Reload() {
            if (Loaded)
                return;

            MapProvider.Reload();
            Loaded = true;
            _lastClient = DateTime.UtcNow;
            Logger.Log(LogType.Info, $"Map '{MapProvider.MapName}' reloaded.");
            MapLoaded?.Invoke(this);
        }
        #endregion

        /// <summary>
        /// Loads a new file into this map object.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        public void Load(string filename) {
            Filename = filename;
            MapProvider.Load(filename);
            _lastClient = DateTime.UtcNow;
            Loaded = true;
            Logger.Log(LogType.Info, $"Map {MapProvider.MapName} (by {MapProvider.CreatingUser}) loaded.");
            MapLoaded?.Invoke(this);
        }

        public override string ToString() {
            Vector3S mapSize = MapProvider.GetSize();
            return
                $"{MapProvider.MapName} by {MapProvider.CreatingUser} on {MapProvider.CreatingService}. ({mapSize.X} x {mapSize.Y} x {mapSize.Z})";
            
        }

        public void Save() {
            Save(Filename);
        }

        /// <summary>
        /// Save the map to disk
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename) {
            if (!Loaded)
                return;

            bool result = MapProvider.Save(filename);

            Saved?.Invoke(this);

            if (result) // -- TODO: Move this message maybe?
                Logger.Log(LogType.Info, $"Map saved successfully ({MapProvider.MapName})");
            else
                Logger.Log(LogType.Error, $"Error saving map {MapProvider.MapName}!");
        }


        public byte[][] GetChunks() {
            if (!Loaded)
                Reload();
            // -- TODO: Cache this stuff.

            Vector3S mapSize = MapProvider.GetSize();
            int blockDataSize = mapSize.X * mapSize.Y * mapSize.Z;
            byte[] blockData = MapProvider.GetBlocks();

            var sendMap = new byte[blockDataSize + 4]; // -- The full mapdata that will be sent
            byte[] lenBytes = BitConverter.GetBytes(blockDataSize); // -- Get the length of the map
            Array.Reverse(lenBytes); // -- Reverse it for proper endianness
            Buffer.BlockCopy(lenBytes, 0, sendMap, 0, 4); // -- Copy the length into the send map
            Buffer.BlockCopy(blockData, 0, sendMap, 4, blockDataSize); // -- Copy all of the block data in

            sendMap = GZip.Compress(sendMap); // -- Compress it

            // -- Chunks the compresed map by chunks of 1024.
            var offset = 0;
            var chunks = new List<byte[]>();

            while (offset != sendMap.Length) {
                

                if (sendMap.Length - offset > 1024) {
                    var send = new byte[1024];
                    // -- If the map has more than 1024 left in it
                    Buffer.BlockCopy(sendMap, offset, send, 0, 1024); // -- copy this data in..
                    chunks.Add(send);
                    offset += 1024;
                } else {
                    var send = new byte[sendMap.Length - offset];
                    Buffer.BlockCopy(sendMap, offset, send, 0, sendMap.Length - offset);
                    chunks.Add(send);
                    offset += sendMap.Length - offset;
                }
            }

            blockData = null;
            sendMap = null;
            return chunks.ToArray();
        }

        public void Resend() {
            Entity[] entities;

            lock (Entity.AllEntities) {
                entities = Entity.AllEntities.Where(a => a.CurrentMap == this && a.AssoClient != null).ToArray();
            }

            foreach (Entity entity in entities) {
                entity.AssoClient.ClientPlayer.ChangeMap(this);
            }
        }

    }
}
