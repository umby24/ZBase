using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ZBase.Common {
    public class BlockManager : TaskItem {
        // -- TODO: Seperate server block ID from on-client block ID.
        // -- Use the above to allow the creation of 65535 different block types..

        public static string BlockFile = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config", "blocks.json");
        //public static string CustomBlockFile = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config", "customblocks.json");

        // -- Quick lookup table for what blocks to replace for non-supporting clients.
        //public static Dictionary<byte, byte> CpeReplace = new Dictionary<byte, byte>();

        // -- Another quick lookup table, but for custom blocks instead of built in cpe blocks.
        //public static Dictionary<byte, byte> CustomReplace = new Dictionary<byte, byte>();

        // -- Quick block lookup table..
        private static Block[] _blocks = new Block[256];
        //public  static CustomBlock[] CustomBlocks = new CustomBlock[128];
        private DateTime _lastNormal;
        //private DateTime _lastCustom;

        public BlockManager() {
            LastRun = new DateTime();
            Interval = TimeSpan.FromSeconds(1);
        }

        #region File Management
        /// <summary>
        /// Generates the blocks file when it doesn't exist.
        /// </summary>
        public void GenerateFile() {
            string path = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            CreateBlock("Air", 0, 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Stone", 1, 0, 0, 0, "", false, false, 6645093, 0, 1, false, -1);
            CreateBlock("Grass", 2, 0, 1200, 1200, "", false, false, 4960630, 0, 0, false, -1);
            CreateBlock("Dirt", 3, 0, 1200, 1200, "", false, false, 3624555, 0, 0, false, -1);
            CreateBlock("Cobblestone", 4, 0, 0, 0, "", false, false, 6250336, 0, 0, false, -1);
            CreateBlock("Planks", 5, 0, 0, 0, "", false, false, 4220797, 0, 0, false, -1);
            CreateBlock("Sapling", 6, 0, 0, 0, "", false, false, 11401600, 0, 0, false, -1);
            CreateBlock("Solid", 7, 0, 0, 0, "", false, false, 4539717, 0, 0, false, -1);
            CreateBlock("Water", 8, 20, 100, 100, "", false, false, 10438957, 0, 0, false, -1);
            CreateBlock("Still Water", 9, 21, 100, 100, "", false, false, 10438957, 0, 0, true, -1);
            CreateBlock("Lava", 10, 20, 500, 100, "", false, false, 1729750, 0, 0, false, -1);
            CreateBlock("Still Lava", 11, 21, 500, 100, "", false, false, 1729750, 0, 0, true, -1);
            CreateBlock("Sand", 12, 11, 200, 100, "", false, false, 8431790, 0, 0, false, -1);
            CreateBlock("Gravel", 13, 10, 200, 100, "", false, false, 6710894, 0, 0, false, -1);
            CreateBlock("Gold ore", 14, 0, 0, 0, "", false, false, 6648180, 0, 0, false, -1);
            CreateBlock("Iron ore", 15, 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Coal", 16, 0, 0, 0, "", false, false, 6118749, 0, 0, false, -1);
            CreateBlock("Log", 17, 0, 0, 0, "", false, false, 2703954, 0, 0, false, -1);
            CreateBlock("Leaves", 18, 0, 0, 0, "", false, false, 2535736, 0, 0, false, -1);
            CreateBlock("Sponge", 19, 0, 0, 0, "", false, false, 3117714, 0, 0, false, -1);
            CreateBlock("Glass", 20, 0, 0, 0, "", false, false, 16118490, 0, 0, false, -1);
            CreateBlock("Red Cloth", 21, 0, 0, 0, "", false, false, 2763442, 0, 0, false, -1);
            CreateBlock("Orange Cloth", 22, 0, 0, 0, "", false, false, 2780594, 0, 0, false, -1);
            CreateBlock("Yellow Cloth", 23, 0, 0, 0, "", false, false, 2798258, 0, 0, false, -1);
            CreateBlock("Light Green Cloth", 24, 0, 0, 0, "", false, false, 2798189, 0, 0, false, -1);
            CreateBlock("Green Cloth", 25, 0, 0, 0, "", false, false, 2798122, 0, 0, false, -1);
            CreateBlock("Aqua Cloth", 26, 0, 0, 0, "", false, false, 7254570, 0, 0, false, -1);
            CreateBlock("Cyan Cloth", 27, 0, 0, 0, "", false, false, 11711018, 0, 0, false, -1);
            CreateBlock("Light Blue Cloth", 28, 0, 0, 0, "", false, false, 11699029, 0, 0, false, -1);
            CreateBlock("Blue", 29, 0, 0, 0, "", false, false, 11690337, 0, 0, false, -1);
            CreateBlock("Purple", 30, 0, 0, 0, "", false, false, 11676269, 0, 0, false, -1);
            CreateBlock("Light Purple Cloth", 31, 0, 0, 0, "", false, false, 11680908, 0, 0, false, -1);
            CreateBlock("Pink Cloth", 32, 0, 0, 0, "", false, false, 11676338, 0, 0, false, -1);
            CreateBlock("Dark Pink Cloth", 33, 0, 0, 0, "", false, false, 7154354, 0, 0, false, -1);
            CreateBlock("Dark Grey Cloth", 34, 0, 0, 0, "", false, false, 4144959, 0, 0, false, -1);
            CreateBlock("Light Grey Cloth", 35, 0, 0, 0, "", false, false, 7566195, 0, 0, false, -1);
            CreateBlock("White Cloth", 36, 0, 0, 0, "", false, false, 11711154, 0, 0, false, -1);
            CreateBlock("Yellow Flower", 37, 0, 0, 0, "", false, false, 8454143, 0, 0, false, -1);
            CreateBlock("Red Flower", 38, 0, 0, 0, "", false, false, 255, 0, 0, false, -1);
            CreateBlock("Brown Mushroom", 39, 0, 0, 0, "", false, false, 2565927, 0, 0, false, -1);
            CreateBlock("Red Mushroom", 40, 0, 0, 0, "", false, false, 2631720, 0, 0, false, -1);
            CreateBlock("Gold Block", 41, 0, 0, 0, "", false, false, 2590138, 0, 0, false, -1);
            CreateBlock("Iron Block", 42, 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Double Stair", 43, 0, 0, 0, "", false, false, 2829099, 0, 0, false, -1);
            CreateBlock("Stair", 44, 0, 0, 0, "", false, false, 2894892, 0, 0, false, -1);
            CreateBlock("Bricks", 45, 0, 0, 0, "", false, false, 4282014, 0, 0, false, -1);
            CreateBlock("TNT", 46, 0, 0, 0, "", false, false, 3951751, 0, 0, false, -1);
            CreateBlock("Bookcase", 47, 0, 0, 0, "", false, false, 3098197, 0, 0, false, -1);
            CreateBlock("Mossy Cobblestone", 48, 0, 0, 0, "", false, false, 4806729, 0, 0, false, -1);
            CreateBlock("Obsidian", 49, 0, 0, 0, "", false, false, 1708562, 0, 0, false, -1);

            // -- CPE Blocks
            CreateBlock("Cobblestone Slab", 50, 0, 0, 0, "", false, false, 8421504, 1, 44, false, -1);
            CreateBlock("Rope", 51, 0, 0, 0, "", false, false, 4220797, 1, 39, false, -1);
            CreateBlock("Sandstone", 52, 0, 0, 0, "", false, false, 8431790, 1, 12, false, -1);
            CreateBlock("Snow", 53, 22, 200, 50, "", false, false, 15461355, 1, 0, false, -1);
            CreateBlock("Fire", 54, 0, 0, 0, "", false, false, 33023, 1, 10, false, -1);
            CreateBlock("Light Pink Wool", 55, 0, 0, 0, "", false, false, 16744703, 1, 33, false, -1);
            CreateBlock("Forest Green Wool", 56, 0, 0, 0, "", false, false, 16384, 1, 25, false, -1);
            CreateBlock("Brown Wool", 57, 0, 0, 0, "", false, false, 4019043, 1, 3, false, -1);
            CreateBlock("Deep Blue Wool", 58, 0, 0, 0, "", false, false, 16711680, 1, 29, false, -1);
            CreateBlock("Turquoise Wool", 59, 0, 0, 0, "", false, false, 16744448, 1, 28, false, -1);
            CreateBlock("Ice", 60, 0, 0, 0, "", false, false, 16777139, 1, 20, false, -1);
            CreateBlock("Ceramic Tile", 61, 0, 0, 0, "", false, false, 12632256, 1, 42, false, -1);
            CreateBlock("Magma", 62, 0, 0, 0, "", false, false, 128, 1, 49, false, -1);
            CreateBlock("Pillar", 63, 0, 0, 0, "", false, false, 12632256, 1, 36, false, -1);
            CreateBlock("Crate", 64, 0, 0, 0, "", false, false, 4220797, 1, 5, false, -1);
            CreateBlock("Stone Brick", 65, 0, 0, 0, "", false, false, 12632256, 1, 1, false, -1);

            File.WriteAllText(BlockFile, JsonConvert.SerializeObject(_blocks, Formatting.Indented));
            _lastNormal = File.GetLastWriteTime(BlockFile);
            Logger.Log(LogType.Info, "Blocks created");
        }

        /// <summary>
        /// Creates a new block and saves it to file.
        /// </summary>
        /// <param name="name">Name of the block.</param>
        /// <param name="onClient">The block ID to send to the client.</param>
        /// <param name="physics">The physics type to be processed for this block.</param>
        /// <param name="physicsDelay">The amount of time between physics ticks for this block.</param>
        /// <param name="physicsRandom">A random time added to the base physics delay.</param>
        /// <param name="physicsPlugin">The plugin that will be called to handle physics.</param>
        /// <param name="replacePhysics">If the block should be re-added to the physics queue after physics has completed.</param>
        /// <param name="kills">True if a player will be killed upon contact with this block.</param>
        /// <param name="color">The color code for this block.</param>
        /// <param name="cpeLevel">The CustomBlocks level that this block is in.</param>
        /// <param name="cpeReplace">The block to replace this block with if the client doesn't support the above CPE Level.</param>
        /// <param name="special">True to show this block on the custom materials list.</param>
        /// <param name="replaceOnLoad">-1 for none. Replaces this block with another on map load.</param>
        public void CreateBlock(string name, byte onClient, int physics,
            int physicsDelay, int physicsRandom, string physicsPlugin, bool replacePhysics, bool kills, int color,
            int cpeLevel, int cpeReplace, bool special, int replaceOnLoad) {

            var newBlock = new Block {
                Id = onClient,
                Name = name,
                OnClient = onClient,
                Physics = physics,
                PhysicsDelay = physicsDelay,
                PhysicsRandom = physicsRandom,
                PhysicsPlugin = physicsPlugin,
                RepeatPhysics = replacePhysics,
                Kill = kills,
                Color = color,
                Special = special,
                ReplaceOnLoad = replaceOnLoad,
                PlaceRank = 0,
                DeleteRank = 0
            };

            _blocks[onClient] = newBlock;
        }


        public void LoadNormal() {
            if (!File.Exists(BlockFile)) {
                GenerateFile();
                return;
            }

            // -- Load all normal + CPE Blocks.
            try {
                var tmp = JsonConvert.DeserializeObject<Block[]>(File.ReadAllText(BlockFile));

                foreach (Block block in tmp) {
                    if (block == null)
                        continue;

                    if (block.Id > 255)
                        continue;

                    _blocks[block.Id] = block;
                }
            }
            catch (Exception e) {
                Logger.Log(LogType.Error, "Error loading blocks, reverting to default.");
                Logger.Log(LogType.Debug, e.Message);
                Logger.Log(LogType.Verbose, e.StackTrace);
                GenerateFile();
            }
            _lastNormal = File.GetLastWriteTime(BlockFile);
            Logger.Log(LogType.Info, "Blocks loaded.");
        }

     

        public void Save() {
            try {
                File.WriteAllText(BlockFile, JsonConvert.SerializeObject(_blocks, Formatting.Indented));
                Logger.Log(LogType.Info, "Blocks saved.");
                _lastNormal = File.GetLastWriteTime(BlockFile);
            }
            catch (Exception e) {
                Logger.Log(LogType.Error, "Error saving blocks.");
                Logger.Log(LogType.Debug, e.Message);
                Logger.Log(LogType.Verbose, e.StackTrace);
            }
        }


        #endregion

        public static Block GetBlock(byte id) {
            return _blocks[id];
        }

        public static Block GetBlock(string input) {
            List<Block> normal = _blocks.Where(a => a != null && String.Equals(a.Name, input, StringComparison.CurrentCultureIgnoreCase)).ToList();
            return normal.Any() ? normal.First() : null;
        }

        public override void Setup() {
            LoadNormal();
        }

        public override void Main() {
            if (File.GetLastWriteTime(BlockFile) != _lastNormal)
                LoadNormal();
            
        }

        public override void Teardown() {
            Save();
        }
    }
}
