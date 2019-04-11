using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZBase.Common {
    public class Logger : TaskItem {
        private static string _filename;
        private static LogType _minimumLevel;
        private bool _setup;
        private static readonly object LogLock = new object();
        private static bool _fileOutput;

        public static event LogEventArgs LogItemAdded;

        public override void Setup() {
            if (_setup) {
                if (!Directory.Exists(Configuration.Settings.General.LogDirectory))
                    Directory.CreateDirectory(Configuration.Settings.General.LogDirectory);

                _minimumLevel = (LogType)Enum.Parse(typeof(LogType), Configuration.Settings.General.LogLevel, true);
                _fileOutput = true;
                DateTime nowTime = DateTime.UtcNow;
                _filename = "log." + nowTime.Year + nowTime.Month + nowTime.Day + nowTime.Hour + nowTime.Minute + ".txt";
                _filename = Path.Combine(Configuration.Settings.General.LogDirectory, _filename);
                File.AppendAllText(_filename, "# Log Start at " + nowTime.ToLongDateString() + " - " + nowTime.ToLongTimeString() + Environment.NewLine);

                PruneLogs();
                return;
            }

            LastRun = new DateTime();
            Interval = new TimeSpan(0, 0, 2);
            _setup = true;
        }

        public override void Main() {
            _minimumLevel = (LogType)Enum.Parse(typeof (LogType), Configuration.Settings.General.LogLevel, true);
        }

        public override void Teardown() {
        }

        public static void Log(LogType type, string message) {
            var item = new LogItem { Type = type, Time = DateTime.UtcNow, Message = message };

            LogItemAdded?.Invoke(item);

            lock (LogLock) {
                if (_fileOutput)
                    File.AppendAllText(_filename, $"{item.Time.ToLongTimeString()} > [{item.Type}] {item.Message}" + Environment.NewLine);

                ConsoleOutput(item);
            }
        }

        private static void PruneLogs() {
            string[] files = Directory.GetFiles(Configuration.Settings.General.LogDirectory);

            if (files.Length <= Configuration.Settings.General.LogPrune)
                return;

            Dictionary<string, DateTime> listing = files.ToDictionary(file => file, File.GetLastWriteTime);

            List<KeyValuePair<string, DateTime>> sorted =
                listing.OrderByDescending(pair => pair.Value).ToList();

            for (var i = 0; i < Configuration.Settings.General.LogPrune; i++) {
                sorted.Remove(sorted[0]);
            }
            foreach (KeyValuePair<string, DateTime> keyValuePair in sorted) {
                if (File.Exists(keyValuePair.Key))
                    File.Delete(keyValuePair.Key);
            }

            Log(LogType.Debug, "Log files pruned.");
        }

        private static void ConsoleOutput(LogItem item) {
            if ((int) item.Type < (int) _minimumLevel)
                return;

            string line = $"{item.Time.ToLongTimeString()} > ";

            switch (item.Type) {
                case LogType.Verbose:
                    line += "&8[Verbose]";
                    break;
                case LogType.Debug:
                    line += "&8[Debug]";
                    break;
                case LogType.Warning:
                    line += "&e[Warning]&f";
                    break;
                case LogType.Error:
                    line += "&4[Error]&f";
                    break;
                case LogType.Chat:
                    line += "&5[Chat]&f";
                    break;
                case LogType.Command:
                    line += "&a[Command]&8";
                    break;
                case LogType.Info:
                    line += "[Info]&f";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            line += $" {item.Message}";
            ColorConvertingConsole.WriteLine(line);
        }
    }
}
