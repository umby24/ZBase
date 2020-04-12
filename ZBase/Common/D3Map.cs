using System;
using System.IO;

namespace ZBase.Common {
    /// <summary>
    /// Which type of overview image the D3 software would generate
    /// in D3, at an automated interval, a .png image of the map would be generated and placed in the same folder as the map.
    /// The Overview type determined what that image would look like.
    /// 2d: essentially a heightmap
    /// Iso: 3D, ISO-graphic map image.
    /// </summary>
    /// 
    public enum D3OverviewType {
        None,
        TwoDee,
        Iso,
    }

    /// <summary>
    /// Provides a complete implementation of the D3 v1+ Folder-based map format, in c#.
    /// </summary>
    public class D3Map {
        // -- File names for each part of the map.
        private const string ConfigName = "Config.txt";
        private const string BlocksName = "Data-Layer.gz";
        private const string RBoxName = "Rank-Layer.txt";
        private const string PortalsName = "Teleporter.txt";

        public Vector3S MapSize;
        public MinecraftLocation MapSpawn;

        private readonly PreferenceLoader _configFile;

        // -- ID info.
        private string _UUID;
        public string Name { get; set; }
        // -- Perms
        private int _buildRank;
        private int _joinRank;
        private int _showRank;
        // -- Settings
        private bool _physics;
        private string _motd;
        private int _saveInterval;
        private int _serverVersion;
        private D3OverviewType _overviewType;
        // -- Internal class state
        private string _mapPath;
        public byte[] MapData;

        public D3Map(string folder) {
            _mapPath = folder;
            MapSize = new Vector3S();
            MapSpawn = new MinecraftLocation();
            _configFile = new PreferenceLoader(ConfigName, null, folder);
            Load();
        }
        
        public D3Map(string folder, string name, short sizeX, short sizeY, short sizeZ) {
            _mapPath = folder;
            _configFile = new PreferenceLoader(ConfigName, null, folder);
            _UUID = GenerateUuid();
            _saveInterval = 10;
            _serverVersion = 1004;
            _overviewType = D3OverviewType.Iso;
            Name = name;
            MapSize = new Vector3S(sizeX, sizeY, sizeZ);
            
            MapSpawn = new MinecraftLocation();
            MapSpawn.SetAsBlockCoords(new Vector3S(sizeX / 2, sizeY / 2, sizeZ / 2));
            
            MapData = new byte[(sizeX * sizeY * sizeZ) * 4];
        }
        
        /// <summary>
        /// C# port of D3's UUID Gen.
        /// </summary>
        /// <returns>16 character random string.(A-Z)</returns>
        private static string GenerateUuid() {
            var final = "";

            var rand = new Random();

            for (var i = 0; i < 16; i++) {
                final += (char)(65 + rand.Next(25));
            }
            
            return final;
        }

        public bool Load() {
            try {
                _configFile.LoadFile();
                
                ReadConfig(Path.Combine(_mapPath, ConfigName));
                
                if (_serverVersion == 0 || MapSize.X == 0 || MapSize.Y == 0 || MapSize.Z == 0) {
                    Logger.Log(LogType.Error, "Invalid map config!");
                    return false;
                }
                
                ReadData(Path.Combine(_mapPath, BlocksName));
                return true;
            } catch (Exception ex) {
                Logger.Log(LogType.Error, "Failed to load " + _mapPath + "! - " + ex.Message);
                Logger.Log(LogType.Debug, ex.StackTrace);
                return false;
            }
        }

        public bool Save() {
            if (!Directory.Exists(_mapPath)) {
                Directory.CreateDirectory(_mapPath);
            }
            
            return SaveMapData() && SaveConfig();
        }

        private bool SaveConfig() {
            try {
                // -- Update the values in the configuration library
                _configFile.Write("Server_Version", _serverVersion);
                _configFile.Write("Size_X", MapSize.X);
                _configFile.Write("Size_Y", MapSize.Y);
                _configFile.Write("Size_Z", MapSize.Z);

                _configFile.Write("Unique_ID", _UUID);
                _configFile.Write("Name", Name);
                _configFile.Write("Rank_Build", _buildRank);
                _configFile.Write("Rank_Show", _showRank);
                _configFile.Write("Rank_Join", _joinRank);
                _configFile.Write("Physic_Stopped", _physics ? "1" : "0");
                _configFile.Write("MOTD_Override", _motd);
                _configFile.Write("Save_Intervall", _saveInterval);
                _configFile.Write("Overview_Type", (int)_overviewType);
                _configFile.Write("Spawn_X", (MapSpawn.Location.X / 32f).ToString());
                _configFile.Write("Spawn_Y", (MapSpawn.Location.Y / 32f).ToString());
                _configFile.Write("Spawn_Z", (MapSpawn.Location.Z / 32f).ToString());
                _configFile.Write("Spawn_Rot", MapSpawn.Rotation);
                _configFile.Write("Spawn_Look", MapSpawn.Look);

                _configFile.SaveFile(); // -- Call out the save method.
                return true;
            } catch (Exception ex) {
                Logger.Log(LogType.Error, "Error occured saving D3Map config " + Name + " : " + ex.Message);
                Logger.Log(LogType.Debug, ex.StackTrace);
                return false;
            }
        }

        private bool SaveMapData() {
            try {
                var compressed = GZip.Compress(MapData);
                File.WriteAllBytes(Path.Combine(_mapPath, BlocksName), compressed);
                return true;
            } catch (Exception ex) {
                Logger.Log(LogType.Error, "Error occured saving D3Map " + Name + " : " + ex.Message);
                Logger.Log(LogType.Debug, ex.StackTrace);
                return false;
            }
        }

        private void ReadConfig(string fileName) {
            var spawnLoc = new Vector3S();

            _serverVersion = _configFile.Read("Server_Version", 0);
            MapSize.X = short.Parse(_configFile.Read("Size_X", "0"));
            MapSize.Y = short.Parse(_configFile.Read("Size_Y", "0"));
            MapSize.Z = short.Parse(_configFile.Read("Size_Z", "0"));
            _UUID = _configFile.Read("Unique_ID", GenerateUuid());
            Name = _configFile.Read("Name", "Map_Name_Here");
            _buildRank = _configFile.Read("Rank_Build", 0);
            _showRank = _configFile.Read("Rank_Show", 0);
            _joinRank = _configFile.Read("Rank_Join", 0);
            _physics = _configFile.Read("Physic_Stopped", 0) == 1;
            _motd = _configFile.Read("MOTD_Override", "");
            _saveInterval = _configFile.Read("Save_Intervall", 10);
            _overviewType = (D3OverviewType)_configFile.Read("Overview_Type", 2);
            spawnLoc.X = (short)(double.Parse(_configFile.Read("Spawn_X", "0")) * 32);
            spawnLoc.Y = (short)(double.Parse(_configFile.Read("Spawn_Y", "0")) * 32);
            spawnLoc.Z = (short)(double.Parse(_configFile.Read("Spawn_Z", "0")) * 32);
            MapSpawn.Rotation = (byte)_configFile.Read("Spawn_Rot", 0);
            MapSpawn.Look = (byte)_configFile.Read("Spawn_Look", 0);

            MapSpawn.SetAsPlayerCoords(spawnLoc);

        }

        private void ReadData(string filePath) {
            var compressed = File.ReadAllBytes(filePath);
            MapData = GZip.Decompress(compressed);
            compressed = null;
            if (MapData == null) {
                // -- Map load error!!!!
                Logger.Log(LogType.Error, $"Error occured reading D3 Map Data: {Name}.");
            }
        }

         public void Resize(int x, int y, int z) {
            if (x < 16 || y < 16 || z < 16 || x > 32767 || y > 32767 || z > 32767) 
                return;
            
            var mem1 = new byte[(x * y * z) * 4];
                
            int copyX = Math.Min(MapSize.X, x);
            int copyY = Math.Min(MapSize.Y, y);
            int copyZ = Math.Min(MapSize.Z, z);
                
            // -- Copy in existing blocks as much as we can
                
            for (var ix = 0; ix < copyX - 1; ix++) {
                for (var iy = 0; iy < copyY - 1; iy++) {
                    for (var iz = 0; iz < copyZ -1; iz++) {
                        int oldIndex = GetBlockIndex(ix, iy, iz);
                        int newIndex = GetBlockIndex(ix, iy, iz, x, y);
                        mem1[newIndex] = MapData[oldIndex]; // -- Copy existing block into new array.
                    }
                }
            }
            
            
            // -- Adjust spawn location
             Vector3S current = MapSpawn.GetAsBlockCoords();
             short newX = current.X;
             short newY = current.Y;
             
             if (current.X > x - 1)
                 newX = (short)(x - 1);

             if (current.Y > y - 1)
                 newY = (short)(y - 1);
                
            MapSpawn.SetAsBlockCoords(new Vector3S(newX, newY, current.Z));
            MapSize = new Vector3S(x, y, z);
            MapData = mem1;
        }

        public int GetBlock(int x, int y, int z) {
            if (!BlockInBounds(x, y, z)) return -1;
            
            int index = GetBlockIndex(x, y, z);
            return MapData[(index * 4)];
        }

        public void SetBlock(int x, int y, int z, byte block) {
            if (!BlockInBounds(x, y, z)) 
                return;
            
            int index = GetBlockIndex(x, y, z); // -- (Y * Size_Z + Z) * Size_X + x

            // -- * 4: Each entry in the d3 map format takes 4 bytes.
            // -- [byte]BlockID [byte]Metadata [short]PlayerID

            MapData[(index * 4)] = block;
        }

        public void SetBlock(int x, int y, int z, byte block, short playerId) {
            if (!BlockInBounds(x, y, z)) return;
            
            int index = GetBlockIndex(x, y, z);

            MapData[(index * 4)] = block;
            byte[] tempBytes = BitConverter.GetBytes(playerId);
            MapData[(index * 4) + 2] = tempBytes[0];
            MapData[(index * 4) + 3] = tempBytes[1];
        }

        public void SetHistory(int x, int y, int z, short playerId) {
            if (!BlockInBounds(x, y, z)) 
                return;
            
            int index = GetBlockIndex(x, y, z); 

            byte[] tempBytes = BitConverter.GetBytes(playerId);
            MapData[(index * 4) + 2] = tempBytes[0];
            MapData[(index * 4) + 3] = tempBytes[1];

        }

        internal bool BlockInBounds(int x, int y, int z) {
            return (x >= 0 && y >= 0 && z >= 0) && (MapSize.X > x && MapSize.Y > y && MapSize.Z > z);
        }

        internal int GetBlockIndex(int x, int y, int z) {
            return GetBlockIndex(x, y, z, MapSize.X, MapSize.Y);  
        }
        internal int GetBlockIndex(int x, int y, int z, int sizeX, int sizeY) {
            return (x + y * sizeX + z * sizeX * sizeY) * 1;
        }
    }
}
