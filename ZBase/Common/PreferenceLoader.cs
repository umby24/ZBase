using System;
using System.Collections.Generic;
using System.IO;

namespace ZBase.Common {
    /// <summary>
    /// A flexible library for managing settings files in a key = value format, seperated by groups
    /// Groups are defined by a name enclosed in brackets. ex. [example]
    /// Each group may have its own unique key value pairs that share the same keys as other groups, without causing interference.
    /// This imitates the behavior by PureBasic's built-in setting system.
    /// </summary>
   public class PreferenceLoader {
        public string Filename { get; set; }
        public string CurrentGroup { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, Dictionary<string, string>> SettingsDictionary { get; set; }
        public object LoadFunction { get; set; }
        public bool Save { get; set; }

        public delegate void LoadSettings();

        /// <summary>
        /// Creates a new settings object for easy management of settings.
        /// </summary>
        /// <param name="filename">The file to watch, load, and save from</param>
        /// <param name="folder">The folder that this file will be saved in.</param>
        /// <param name="save">True to make the settings loader handle saving of this file.</param>
        /// <param name="readFunction">The function that will be called when this file is loaded.</param>
        public PreferenceLoader(string filename, LoadSettings readFunction, string folder = "Settings/", bool save = true) {
            Filename = Path.Combine(folder, filename);
            CurrentGroup = "";
            SettingsDictionary = new Dictionary<string, Dictionary<string, string>>();
            SettingsDictionary.Add("", new Dictionary<string, string>());
            Save = save;
            LoadFunction = readFunction;
        }

        /// <summary>
        /// Clears all the settings currently loaded, and completely reloads the settings file.
        /// </summary>
        public void LoadFile() {
            if (!File.Exists(Filename))
                File.WriteAllText(Filename, "");

            LoadFile_();
            LastModified = File.GetLastWriteTime(Filename);
            ((LoadSettings)LoadFunction)?.Invoke();
        }

        /// <summary>
        /// Stub function, this does the actual file loading. Run LoadFile(), not this function directly.
        /// </summary>
        private void LoadFile_() {
            CurrentGroup = "";
            SettingsDictionary.Clear();

            using (var sr = new StreamReader(Filename)) {
                while (!sr.EndOfStream) {
                    var thisLine = sr.ReadLine();

                    if (thisLine == null || thisLine.StartsWith(";")) // -- Comment
                        continue;

                    if (thisLine.Contains(";")) // -- Deal with mid-line comments.
                        thisLine = thisLine.Substring(0, thisLine.IndexOf(";"));

                    if (!thisLine.Contains("=") && (!thisLine.Contains("[") && !thisLine.Contains("]")))
                        continue;

                    if ((thisLine.StartsWith("[") && thisLine.EndsWith("]"))) { // -- Group. 
                        var grpName = thisLine.Substring(1, thisLine.Length - 2);
                        Dictionary<string, string> group;

                        while (SettingsDictionary.TryGetValue(grpName, out group)) {
                            grpName += "§";
                            continue;
                        }

                        SettingsDictionary.Add(grpName, new Dictionary<string, string>());
                        CurrentGroup = grpName;
                        continue;
                    }

                    if (!thisLine.Contains("="))
                        continue;

                    var key = thisLine.Substring(0, thisLine.IndexOf("=")).Trim(' ');
                    var value =
                        thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1))
                            .Trim(' ');

                    // -- Setting.
                    if (CurrentGroup == "" && !SettingsDictionary.ContainsKey(""))
                        SettingsDictionary.Add("", new Dictionary<string, string>());

                    if (SettingsDictionary[CurrentGroup].ContainsKey(key))
                        SettingsDictionary[CurrentGroup][key] = value;
                    else
                        SettingsDictionary[CurrentGroup].Add(key, value);
                }
            }
        }

        /// <summary>
        /// Saves this settings object to disk.
        /// </summary>
        public void SaveFile() {
            if (!Save)
                return;

            using (var fileWriter = new StreamWriter(Filename)) {
                foreach (var pair in SettingsDictionary) {
                    if (pair.Key != "")
                        fileWriter.WriteLine("[" + pair.Key.Replace("§", "") + "]");

                    foreach (var subset in pair.Value)
                        fileWriter.WriteLine(subset.Key + " = " + subset.Value);
                }
            }

            LastModified = File.GetLastWriteTime(Filename);
        }

        /// <summary>
        /// Selects a different group of settings. If the group does not exist, it is created.
        /// </summary>
        /// <param name="group">The group of settings to select.</param>
        public void SelectGroup(string group) {
            Dictionary<string, string> table;
            CurrentGroup = group;

            if (SettingsDictionary.TryGetValue(group, out table))
                return;


            SettingsDictionary.Add(group, new Dictionary<string, string>());
        }

        /// <summary>
        /// Reads a setting from the current settings group. Creates the setting if not found.
        /// </summary>
        /// <param name="key">The settings key to read the value for.</param>
        /// <param name="def">Default value to return if value not found.</param>
        /// <returns>[String] stored value.</returns>
        public string Read(string key, string def) {
            string value;

            if (SettingsDictionary[CurrentGroup].TryGetValue(key, out value))
                return value;

            SettingsDictionary[CurrentGroup].Add(key, def);
            return def;
        }

        /// <summary>
        /// Reads a setting from the current settings group. Creates the setting if not found.
        /// Attempts to convert to int once the value is found.
        /// </summary>
        /// <param name="key">The settings key to read the value for.</param>
        /// <param name="def">Default value to return if value not found.</param>
        /// <returns>[Int] stored value.</returns>
        public int Read(string key, int def) {
            string value;

            if (SettingsDictionary[CurrentGroup].TryGetValue(key, out value))
                return int.Parse(value);

            SettingsDictionary[CurrentGroup].Add(key, def.ToString());
            return def;
        }

        /// <summary>
        /// Writes/Overwrites a setting in the current group.
        /// </summary>
        /// <param name="key">The key to write to</param>
        /// <param name="value">The value to write</param>
        public void Write(string key, string value) {
            string va;

            if (SettingsDictionary[CurrentGroup].TryGetValue(key, out va)) {
                SettingsDictionary[CurrentGroup][key] = value;
                return;
            }

            SettingsDictionary[CurrentGroup].Add(key, value);
        }

        /// <summary>
        /// Writes/Overwrites a setting in the current group.
        /// </summary>
        /// <param name="key">The key to write to</param>
        /// <param name="value">The value to write</param>
        public void Write(string key, int value) {
            string va;

            if (SettingsDictionary[CurrentGroup].TryGetValue(key, out va)) {
                SettingsDictionary[CurrentGroup][key] = value.ToString();
                return;
            }

            SettingsDictionary[CurrentGroup].Add(key, value.ToString());
        }
    }
}
