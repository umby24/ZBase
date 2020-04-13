using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZBase.BuildModes;
using ZBase.Commands;
using ZBase.Common;
using ZBase.Fills;
using ZBase.Network;
using ZBase.World;
using TaskScheduler = ZBase.Common.TaskScheduler;

/*
 * TODO:
 * Permissions
 * Plugins??
 * Physics??
 * How should I do blocks??
 * Building assistance commands (/box, /line, etc) -> Inside plugins, or server?
 * Administrative commands (world history, etc)
 * Block send Queues, per player.. (*)
 * Update GUI and CLI to be fully featured
 * portals? Zones? (*)
 * Reboustify things
 * Copy in some D3 Features.. (Configurable file paths, backup times per-map, (*)
 */

 /* ZBASE-SIMPLIFY COMPLETION TASKS
  * - Implement some basic extensibility - Done!
  * - Portals -> Done
  * - Zones
  * - Block Send Queue, Per Player
  * 
  */

namespace ZBase {
    public static class Main {
        public static bool Running;
        private static Server _server;

        /// <summary>
        /// Starts the Hypercube server.
        /// </summary>
        public static void Start()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new MinecraftLocationConverter());
                return settings;
            };

            Setup();
            Running = true;
            Task.Run(() => MainLoop());
        }

        private static void MainLoop() {
            while (Running) {
                TaskScheduler.RunMainTasks();
                Thread.Sleep(1);
            }
        }

        public static void Stop() {
            Running = false;
            TaskScheduler.RunTeardownTasks();
        }

        private static void Setup() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            var myLogger = new Logger();
            myLogger.Setup();

            Logger.Log(LogType.Debug, "Loading configuration...");
            Configuration.Load(); // -- Load server settings.
            Logger.Log(LogType.Debug, "Done");

            var wd = new Watchdog();
            TaskScheduler.RegisterTask("Watchdog", wd);
            TaskScheduler.RegisterTask("Heartbeat", new Heartbeat());
            TaskScheduler.RegisterTask("Blocks", new BlockManager());
            TaskScheduler.RegisterTask("PluginManager", new PluginManager());
            TaskScheduler.RegisterTask("BuildModeManager", new BuildModeManager());
            CommandHandler.RegisterInternalCommands();
            TaskScheduler.RegisterTask("Commands", new CommandHandler());

            TaskScheduler.RunSetupTasks();
            _server = new Server();

            LoadMaps();

            FillManager.LoadFills();
        }

        private static void LoadD3Maps() {
            string[] mapFolders = Directory.GetDirectories("D3Maps");

            foreach(string folder in mapFolders) {
                if (folder == "default")
                    continue;
                
                string[] files = Directory.GetFiles(folder);
                
                if (!files.Contains(Path.Combine(folder, "Data-Layer.gz")) && !files.Contains(Path.Combine(folder, "Config.txt")))
                    continue;

                var nMap = new HcMap(folder);
                if (HcMap.Maps.ContainsKey(nMap.MapProvider.MapName)) {
                    Logger.Log(LogType.Error, $"Could not load D3Map {folder}, a map with the same name is already loaded.");
                    continue;
                }

                HcMap.Maps.Add(nMap.MapProvider.MapName, nMap);
            }
        }

        private static void LoadMaps() {
            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            if (!Directory.Exists("D3Maps"))
                Directory.CreateDirectory("D3Maps");

            HcMap.Maps = new Dictionary<string, HcMap>();
            LoadD3Maps();
            // -- Load the default map
            string defaultPath = Path.Combine("Maps", Configuration.Settings.General.DefaultMap);

            if (!File.Exists(defaultPath) && !File.Exists(defaultPath + "u")) {
                HcMap.DefaultMap = new HcMap(defaultPath, "default", new Vector3S(128, 128, 128));
                HcMap.Maps.Add("default", HcMap.DefaultMap);
                Logger.Log(LogType.Info, "Default map created");
            }
            else {
                if (!File.Exists(defaultPath))
                    defaultPath += "u";

                HcMap.DefaultMap = new HcMap(defaultPath);
                HcMap.Maps.Add(HcMap.DefaultMap.MapProvider.MapName, HcMap.DefaultMap);
            }

            // -- load all other maps (if they exist)
            string[] maps = Directory.GetFiles("Maps", "*.cw?");

            foreach (string map in maps) {
                if (map == defaultPath)
                    continue;

                var newMap = new HcMap(map);

                if (HcMap.Maps.ContainsKey(newMap.MapProvider.MapName)) {
                    Logger.Log(LogType.Error, $"Could not load {map}, a map with the same name is already loaded.");
                    continue;
                }

                HcMap.Maps.Add(newMap.MapProvider.MapName, newMap);
            }
        }
    }
}
