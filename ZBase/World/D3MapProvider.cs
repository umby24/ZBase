using ZBase.Common;

namespace ZBase.World {
    public class D3MapProvider : IMapProvider {
        public string MapName {
            get {
                return _underMap.Name;
            }

            set {
                _underMap.Name = value;
            }
        }

        public string CreatingUser { get; set; }
        public string CreatingService { get; set; }
        public static string MapDirectory => "D3Maps";
        private D3Map _underMap;

        public D3MapProvider() {
            CreatingUser = "D3";
            CreatingService = "D3 Server";
        }

        public void CreateNew(Vector3S size, string filePath, string mapName) {
            _underMap = new D3Map(filePath, mapName, size.X, size.Y, size.Z);
            _underMap.Save();
        }

        public byte GetBlock(short x, short y, short z) {
            return (byte)_underMap.GetBlock(x, y, z);
        }

        public byte[] GetBlocks() {
            var preparedBlocks = new byte[_underMap.MapSize.X * _underMap.MapSize.Y * _underMap.MapSize.Z];
            var offset = 0;

            for (var i = 0; i < preparedBlocks.Length; i++) {
                byte blockId = _underMap.MapData[i * 4];
                preparedBlocks[offset++] = blockId;
            }

            return preparedBlocks;
        }

        public Vector3S GetSize() {
            return _underMap.MapSize;
        }

        public MinecraftLocation GetSpawn() {
            return _underMap.MapSpawn;
        }

        public void Load(string filePath) {
            if (_underMap == null)
                _underMap = new D3Map(filePath);

            _underMap.Load();
        }

        public bool Reload() {
            return _underMap.Load(); 
        }

        public bool Save(string filePath) {
            return _underMap.Save(); 
        }

        public void SetBlock(short x, short y, short z, byte type) {
            _underMap.SetBlock(x, y, z, type);
        }

        public void SetBlocks(byte[] blockData) {
            _underMap.MapData = new byte[blockData.Length * 4];

            for(var i = 0; i < blockData.Length; i++) {
                _underMap.MapData[i * 4] = blockData[i]; // -- Set block type
                _underMap.MapData[(i * 4) + 1] = 0; // -- Zero out the metadata..
                _underMap.MapData[(i * 4) + 2] = 255; // -- Set last change to 0.
                _underMap.MapData[(i * 4) + 3] = 255;
            }
        }

        public void SetSize(Vector3S newSize) {
            _underMap.Resize(newSize.X, newSize.Y, newSize.Z);
        }

        public void SetSpawn(MinecraftLocation spawnLocation) {
            _underMap.MapSpawn = spawnLocation;
        }

        public bool Unload() {
            _underMap.MapData = null;
            return true;
        }
    }
}
