﻿using System;
using System.Collections.Generic;
using ZBase.Build;
using ZBase.Common;
using ZBase.Network;
using ZBase.Persistence;

namespace ZBase.World {
    // -- TODO: Organize
    // -- TODO: Move event handlers out?

    public class Player {
		public static PlayerDb Database = new PlayerDb();
        public string Name { get; set; }
		public string PrettyName { get; set; }
        public HcMap CurrentMap { get; private set; }
		public Rank CurrentRank { get; set; }
        public DateTime MutedUntil { get; set; }
        public BuildState CurrentState { get; set; }
        public string ChatBuffer { get; set; }
        
        public Entity Entity;
        public List<Entity> Entities;
        private readonly Client _client;
        public Block Material;
        public Block LastMaterial;
        public bool Stopped;
        private bool _banned;

        public Player(Client client) {
            _client = client;
            CurrentMap = HcMap.DefaultMap;
            Entities = new List<Entity>();
            CurrentState = new BuildState();
            ChatBuffer = "";
        }

        public void Login() {
            GetDbInfo ();

            if (_banned) {
                _client.Kick(Constants.DefaultBanMessage);
                return;
            }

            Entity = new Entity { // -- Create our entity
                Name = Name,
				PrettyName = PrettyName,
                CurrentMap = CurrentMap,
                ClientId = (sbyte)CurrentMap.GetEntityId(),
                Location = CurrentMap.GetSpawn(),
                AssoClient = _client,
            };

            _client.SendHandshake(CurrentRank.ClientOp); // -- Send the handshake (acknowledgement)
            
            MapSend(CurrentMap.GetChunks()); // -- Send the map to the client.

            CurrentMap.BlockChanged += MapBlockChange; // -- Subscribe to block change events
            CurrentMap.MapChatSent += MapChatReceived; // -- sub to map chat.

            _client.Verified = true; // -- Register the client, annouce their arrival, and set them as verified (Can perform actions)
			
            Entity.Spawn(); // -- Spawn this client for everyone (including themselves)

            lock (Entity.AllEntities) { // -- Spawn all other entities on this client.
                foreach (Entity e in Entity.AllEntities) {
                    if (e.CurrentMap != CurrentMap || e.Name == Name)
                        continue;

                    SpawnEntity(e);
                }
            }

            Entity.HandleMove(); // -- make sure intial position is set.
            
            // -- Register this person for global chat messages.
            Chat.GlobalChatSent += GlobalChatReceived;

			Chat.SendGlobalChat($"{PrettyName}§S logged in.",0,true);
        }

        public void Logout() {
            Entity.Despawn();

            Chat.SendGlobalChat($"§S{Name} logged out.", 0, true);
            CurrentMap.BlockChanged -= MapBlockChange;
            CurrentMap.MapChatSent -= MapChatReceived;
            Chat.GlobalChatSent -= GlobalChatReceived;
        }

        private void SendSplitChat(IEnumerable<string> messages) {
            foreach (string msg in messages) {
                string final = msg;
                if (Chat.FirstIsEmote(final[0])) {
                    final = "." + final;
                }

                _client.SendPacket(PacketCreator.CreateChat(final));
            }
        }

        #region Event Handlers
        private void GlobalChatReceived(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        private void MapChatReceived(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        public void PersonalChatReceived(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        private void MapBlockChange(Vector3S location, byte type) {
            var bc = new SetBlockServer {
                X = location.X,
                Y = location.Y,
                Z = location.Z,
                Block = type
            };
            
            _client.BlockChanges.Enqueue(bc);
            _client.DataAvailable = true;
            //_client.SendPacket(bc);
        }

        /// <summary>
        /// Handle a client moving.
        /// </summary>
        /// <param name="entity"></param>
        private void SomeoneMoved(Entity entity) {
            var t = new PlayerTeleport { // -- It's less math and hassle to just send a teleport instead of anything else.
                Location = entity.Location.Location,
                Pitch = entity.Location.Look,
                PlayerId = entity.ClientId,
                Yaw = entity.Location.Rotation
            };

            if (entity == Entity && entity.SendOwn)
                t.PlayerId = -1;
            else if (entity == Entity && !entity.SendOwn)
                return;

            _client.SendPacket(t);
        }
        #endregion

        #region Incoming Handlers
        /// <summary>
        /// Spawns the entity.
        /// </summary>
        /// <param name="e">E.</param>
        public void SpawnEntity(Entity e) {
            lock (Entities) { // -- Add them to our personal entities list..
                if (!Entities.Contains(e)) // -- Enumeration..
                    Entities.Add(e);
                else
                    return;
            }
            e.EntityMoved += SomeoneMoved;

            var spawn = new SpawnPlayer {
                // -- Spawn them on this client.
                Location = e.Location,
                PlayerId = e.ClientId,
                PlayerName = e.PrettyName
            };

            if (e == Entity) // -- If this is us, we need to flip the ID to -1.
                spawn.PlayerId = -1;

            _client.SendPacket(spawn);     
        }

        public void DespawnEntity(Entity e) {
            lock (Entities) { // -- Add them to our personal entities list..
                if (Entities.Contains(e)) // -- Enumeration..
                    Entities.Remove(e);
            }

            e.EntityMoved -= SomeoneMoved;

            var despawn = new DespawnPlayer {
                PlayerId = e.ClientId
            };

            if (e == Entity)
                return;

            _client.SendPacket(despawn);
        }

        // -- Handles events this player is performing
        public void HandleMove(Vector3S location, byte rot, byte look) {
            if (location == Entity.Location.Location && rot == Entity.Location.Rotation && Entity.Location.Look == look)
                return;

            var newLoc = new MinecraftLocation(location, rot, look);
            Entity.Location = newLoc;

            var portal = CurrentMap.Portals.GetPortal(newLoc);
            
            if (portal != null)
            {
                newLoc = portal.Destination;
                Entity.Location = newLoc;
                Entity.SendOwn = true;
            }

            Entity.HandleMove();
        }

        public bool CanPlaceBlock(byte blockType) {
            // -- Make sure the player hasn't been stopped.
            if (Stopped) {
                Chat.SendClientChat("§SYou are stopped!", 0, _client);
                return false;
            }

            // -- Check map build permissions
            if (CurrentMap.BuildRank > CurrentRank.Value) {
                Chat.SendClientChat("§EOnly " + Rank.GetRank(CurrentMap.BuildRank) + "+ can build here.", 0, _client);
                return false;
            }

            // -- Deny placing of bedrock(?) if not an op.
            if (blockType == 13 && !CurrentRank.ClientOp)
                return false;

            // -- TODO: Per-area build permissions based on rank..

            return true;
        }

        public void BounceBlock(Vector3S location) {
            var bc = new SetBlockServer {
                X = location.X,
                Y = location.Y,
                Z = location.Z,
                Block = CurrentMap.GetBlockId(location.X, location.Y, location.Z)
            };
            
            _client.BlockChanges.Enqueue(bc);
            _client.DataAvailable = true;
            //_client.SendPacket(bc);
        }
        // -- TODO: Maybe create a 'CanBuildAt(Location, Entity) method on the map?'

        public void HandleBlockPlace(Vector3S location, byte type, byte mode) {
            if (!CanPlaceBlock(type)) {
                BounceBlock(location);
                return;
            }

            byte actualType = 0;

            if (mode == 1)
                actualType = type;

            // -- Handle the case that a player does not support block permissions but isn't allowed to place something..

            LastMaterial = BlockManager.GetBlock(type);
            CurrentMap.SetBlockId(location.X, location.Y, location.Z, actualType);
        }

        #endregion

        private void MapSend(IReadOnlyList<byte[]> data) {
            _client.SendPacket(new LevelInit());

            for (var i = 0; i < data.Count; i++) {
                byte[] a = data[i];
                _client.SendPacket(PacketCreator.CreateMapChunk(a, (byte)((i / data.Count) * 100), a.Length));
            }

            _client.SendPacket(PacketCreator.CreateMapFinal(CurrentMap.GetSize()));
        }

        /// <summary>
        /// Changes the map this player is on to another map
        /// </summary>
        /// <param name="map">The map to send this player to.</param>
        public void ChangeMap(HcMap map) {
            // -- Annouce
            Chat.SendGlobalChat($"§SPlayer {Name} changed to map '{map.MapProvider.MapName}'", 0); // -- TODO: Move this into an event handler.
            DespawnEntities();

            // -- Cleanup events
            CurrentMap.BlockChanged -= MapBlockChange;
            CurrentMap.MapChatSent -= MapChatReceived;

            // -- Remove this entity from all other clients on that map
            Entity.Despawn();

            // -- Send the new map
            CurrentMap = map;

            _client.SendHandshake(CurrentRank.ClientOp);
            MapSend(CurrentMap.GetChunks());
            CurrentMap.BlockChanged += MapBlockChange; // -- sub to block changes
            CurrentMap.MapChatSent += MapChatReceived;

            // -- setup our entity
            Entity.CurrentMap = map;
            Entity.Location = CurrentMap.GetSpawn();
            Entity.ClientId = (sbyte)CurrentMap.GetEntityId();
            Entity.Spawn(); // -- spawn us for other people


            lock (Entity.AllEntities) { // -- Spawn all other entities on this client.
                foreach (Entity e in Entity.AllEntities) {
                    if (e.CurrentMap != CurrentMap || e.Name == Name)
                        continue;

                    SpawnEntity(e);
                }
            }

            Entity.HandleMove(); // -- make sure initial position is set.
        }

        private void DespawnEntities() {
            // -- Remove all entities from this client

            Entity[] roEntities;

            lock (Entities) {
                roEntities = Entities.ToArray();
            }

            foreach (Entity roEntity in roEntities) {
                DespawnEntity(roEntity);
            }
        }

        public void ReloadDb() {
            PlayerModel dbEntry = Database.GetPlayerModel(Name);
            CurrentRank = Rank.GetRank(dbEntry.Rank);
            PrettyName = CurrentRank.Prefix + Name + CurrentRank.Suffix;

            Entity.PrettyName = PrettyName;
        }
        
		private void GetDbInfo() {
            if (!Database.ContainsPlayer(Name)) {
                var newPlayer = new PlayerModel {
                    Name = Name,
                    Ip = _client.Ip,
                    Rank = 0,
                    GlobalChat = true
                };
                
                Database.CreatePlayer(newPlayer);
            }

            PlayerModel dbEntry = Database.GetPlayerModel(Name);
            
			CurrentRank = Rank.GetRank (dbEntry.Rank);
			PrettyName = CurrentRank.Prefix + Name + CurrentRank.Suffix;
            Stopped = dbEntry.Stopped;
            _banned = dbEntry.Banned;

            var bannedUntil = dbEntry.BannedUntil;
            var mutedUntil = dbEntry.TimeMuted;

		    DateTime banTime = Utils.GetUnixEpoch().AddSeconds(bannedUntil);

		    if (banTime > DateTime.UtcNow) {
		        _banned = true;
		    }

		    MutedUntil = Utils.GetUnixEpoch().AddSeconds(mutedUntil);
        }
    }
}
