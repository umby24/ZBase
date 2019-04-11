using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ZBase.Common {
    public class Configuration : TaskItem {
        public NetworkSettings Network { get; set; }
        public GeneralSettings General { get; set; }
        public TextSettings Formats { get; set; }
		public CpeSettings Cpe {get; set;}
		public Rank[] Ranks { get; set; }
        public static Configuration Settings { get; private set; }

        public static event EmptyEventArgs SettingsLoaded;
        public static event EmptyEventArgs SettingsSaved;

        public DateTime LastModified;

        private static readonly string ConfigPath = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config.json");

        public Configuration() {
            Network = new NetworkSettings();
            General = new GeneralSettings();
            Formats = new TextSettings();
			Cpe = new CpeSettings ();
			Ranks = Rank.GetDefaultRanks ();

            Interval = TimeSpan.FromSeconds(1);
            TaskScheduler.UnregisterTask("Settings Reload");
            TaskScheduler.RegisterTask("Settings Reload", this);
        }

        public static bool Load() {
            if (!File.Exists(ConfigPath)) {
                Settings = new Configuration {Ranks = Rank.GetDefaultRanks()};

                Logger.Log(LogType.Info, "New config file generated");

                if (Save()) {
                    return true;
                }

                Logger.Log(LogType.Warning, "Failed to save new config file");
                return false;
            }

            try {
                Settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath));
                SettingsLoaded?.Invoke();
                return Save();
            }
            catch (Exception e) {
                Logger.Log(LogType.Error, $"Failed to load config file: {e.Message}");
                Logger.Log(LogType.Debug, $"Stack Trace: {e.StackTrace}");
                Settings = new Configuration();
				Settings.Ranks = Rank.GetDefaultRanks ();
                return false;
            }
        }

        public static bool Save() {
            try {
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                Settings.LastModified = File.GetLastWriteTime(ConfigPath);
                SettingsSaved?.Invoke();
                return true;
            }
            catch (Exception e) {
                Logger.Log(LogType.Error, $"Failed to save config file: {e.Message}");
                Logger.Log(LogType.Debug, $"Stacktrace: {e.StackTrace}");
                return false;
            }
        }

        public override void Setup() {
            // -- Dont Need
        }

        public override void Main() {
            if (File.GetLastWriteTime(ConfigPath) == LastModified)
                return;

            Load();
            LastModified = File.GetLastWriteTime(ConfigPath);
            Logger.Log(LogType.Info, "Config file loaded.");

        }

        public override void Teardown() {
            // -- not needed.
        }
    }
}
