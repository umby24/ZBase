using System;
using ClassicWorldCore;
using ZBase.Common;

namespace ZBase.World {
    public class ClassicWorldMapProvider : IMapProvider {
          public string MapName {
            get { return _cwMap.MapName; }
            set { _cwMap.MapName = value; }
        }

        public string CreatingUser {
            get { return _cwMap.CreatingUsername; }
            set { _cwMap.CreatingUsername = value;  }
        }

        public string CreatingService {
            get { return _cwMap.CreatingService;  }
            set { _cwMap.CreatingService = value;  }
        }

        public static string MapDirectory = "Maps";
        
        private Classicworld _cwMap;
        private string _currentFilePath;

        public ClassicWorldMapProvider() {

        }

        public void CreateNew(Vector3S size, string filePath, string mapName) {
            _cwMap = new Classicworld(size.X, size.Z, size.Y) {
                MapName = mapName,
                GeneratingSoftware = "Hypercube",
                GeneratorName = "Blank",
                CreatingUsername = "[SERVER]",
                CreatingService = "Classicube",
                SpawnX = (short)(size.X / 2),
                SpawnY = (short)(size.Z / 2),
                SpawnZ = (short)(size.Y / 2),
            };

            _currentFilePath = filePath;
            _cwMap.Save(filePath);
        }

        public byte[] GetBlocks() {
            return _cwMap.BlockData;
        }

        public Vector3S GetSize() {
            return new Vector3S(_cwMap.SizeX, _cwMap.SizeZ, _cwMap.SizeY);
        }

        public MinecraftLocation GetSpawn() {
            return new MinecraftLocation( 
                new Vector3S(_cwMap.SpawnX, _cwMap.SpawnZ, _cwMap.SpawnY), 
                _cwMap.SpawnRotation, 
                _cwMap.SpawnLook
               );
        }

        public void Load(string filePath) {
            if (_cwMap == null) {
                _cwMap = new Classicworld(filePath);
            }

            _currentFilePath = filePath;
            _cwMap.Load();
        }

        public bool Reload() {
            _cwMap = new Classicworld(_currentFilePath);
            _cwMap.Load();
            return true;
        }

        public bool Save(string filePath) {
            _cwMap.Save(filePath);
            return true;
        }

        public void SetBlocks(byte[] blockData) {
            _cwMap.BlockData = blockData;
        }

        public void SetSize(Vector3S newSize) { // -- TODO: Make resizing an action? // -- BUG: This is broken :<
            _cwMap.SizeX = newSize.X;
            _cwMap.SizeY = newSize.Z;
            _cwMap.SizeZ = newSize.Y;

            var newBlockArray = new byte[newSize.X * newSize.Y * newSize.Z];
            Buffer.BlockCopy(_cwMap.BlockData, 0, newBlockArray, 0, newBlockArray.Length);
            _cwMap.BlockData = newBlockArray;
        }

        public bool Unload() {
            _cwMap.BlockData = null;
            return true;
        }

        public void SetBlock(short x, short z, short y, byte type) {
            int index = (y * _cwMap.SizeZ + z) * _cwMap.SizeX + x;
            _cwMap.BlockData[index] = type;
        }

        public byte GetBlock(short x, short z, short y) {
            int index = (y * _cwMap.SizeZ + z) * _cwMap.SizeX + x;
            return _cwMap.BlockData[index];
        }

        public void SetSpawn(MinecraftLocation spawnLocation) {
            _cwMap.SpawnX = spawnLocation.Location.X;
            _cwMap.SpawnY = spawnLocation.Location.Z;
            _cwMap.SpawnZ = spawnLocation.Location.Y;
            _cwMap.SpawnRotation = spawnLocation.Rotation;
            _cwMap.SpawnLook = spawnLocation.Look;
        }
    }
}