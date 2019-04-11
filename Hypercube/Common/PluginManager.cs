using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ZBase.Common {
    public class PluginManager : TaskItem {
        private const int ApiVersion = 1;
        private static readonly List<ZBasePlugin> InternalPlugins = new List<ZBasePlugin>();
        private static readonly Dictionary<string, DateTime> FileTracking = new Dictionary<string, DateTime>();
        
        public PluginManager() {
            Interval = TimeSpan.FromSeconds(1);
            LastRun = new DateTime();
        }

        private static void LoadPlugins() {
            if (!Directory.Exists("Plugins"))
                Directory.CreateDirectory("Plugins");
            
            string[] files = Directory.GetFiles("Plugins", "*.dll");

            foreach (string file in files) {
                TryPluginLoad(Path.GetFullPath(file));
                Logger.Log(LogType.Verbose, $"Loading {file}");
            }
        }
        
        private static void TryPluginLoad(string filepath) {
            try {
                Assembly myAsm = Assembly.LoadFile(filepath); // -- Load the plugin into the CLR
                Type[] applicableTypes =
                    myAsm.GetTypes()
                        .Where(a => a.IsClass && !a.IsAbstract && a.IsSubclassOf(typeof(ZBasePlugin)))
                        .ToArray(); // -- Grab any types that are ZBase Plugins

                foreach (Type type in applicableTypes) { // -- Go through and find which type in this class were the ZBase plugin types..
                    if (type.IsAbstract || type.IsInterface || !type.IsSubclassOf(typeof(ZBasePlugin)))
                        continue;
                    var inst = (ZBasePlugin) Activator.CreateInstance(type); // -- Create an instance of that class

                    if (inst.ApiVersion != ApiVersion) {
                        Logger.Log(LogType.Warning,
                            $"Could not load {inst.PluginName} version {inst.PluginVersion}, incorrect API Version.");
                        continue;
                    }

                    InternalPlugins.Add(inst);

                    if (!FileTracking.ContainsKey(filepath))
                        FileTracking.Add(filepath, File.GetLastWriteTime(filepath));
                    else
                        FileTracking[filepath] = File.GetLastWriteTime(filepath);

                    inst.PluginInit(); // -- Formally Initialize the plugin.
                    Logger.Log(LogType.Verbose,
                        $"Registered plugin {inst.PluginName} (v.{inst.PluginVersion}) by {inst.PluginAuthor}.");
                }
            } catch(Exception ex) {
                Logger.Log(LogType.Error, $"Error while loading {filepath} : {ex.Message}");
                Logger.Log(LogType.Debug, ex.StackTrace);
            }
        }
        
        public override void Setup() {
            LoadPlugins();
        }

        public override void Main() {
            List<string> updateList = FileTracking.Keys.Where(fp => File.GetLastWriteTime(fp) != FileTracking[fp]).ToList();
            updateList.ForEach(TryPluginLoad);
        }

        public override void Teardown() {
            foreach (ZBasePlugin internalPlugin in InternalPlugins) {
                internalPlugin.PluginUnload();
            }
        }
    }
}