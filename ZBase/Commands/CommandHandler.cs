using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class CommandHandler : TaskItem {
        public static string CommandPrefix = "/";
        public static string CommandFile = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config", "commands.json");
        public static Dictionary<string, Command> Commands = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);
		public static List<string> Groups = new List<string>();
        private static DateTime _lastModified;

        public CommandHandler() {
            LastRun = new DateTime();
            Interval = TimeSpan.FromSeconds(1);
        }

        public static void HandleCommand(Client c, string message) {
            if (!message.Contains(" "))
                message += " ";

            // -- Split the message into its subsections.
            // -- The command (ex. /help)
            string command = message.Substring(0, message.IndexOf(" "));
            // -- An array of arguments provided to the command.
            string[] splits = message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1)).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            // -- All text after the actual command
           // string text = message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1));
            // -- All text starting after the first argument of a command.
         //   string text2 = text.Substring(text.IndexOf(" ") + 1, text.Length - (text.IndexOf(" ") + 1));

            // -- Log the command usage
            if (!Configuration.Settings.General.LogArguments)
                Logger.Log(LogType.Command, $"Player '{c.ClientPlayer.Entity.PrettyName}&f' used command '{command}'");
            else
                Logger.Log(LogType.Command, $"Player '{c.ClientPlayer.Entity.PrettyName}&f' used command '{command}' (\"{string.Join("\", \"", splits)}\")");

            Command toExecute;

            if (!Commands.TryGetValue(command.Replace(CommandPrefix, ""), out toExecute)) {
                Chat.SendClientChat($"§EInvalid command: {command}", 0, c);
                return;
            }

			if (toExecute.MinRank > c.ClientPlayer.CurrentRank.Value) {
				Chat.SendClientChat ("§EYou don't have permission to use this command.", 0, c);
				return;
			}

            toExecute.ExecutingClient = c;
            toExecute.Execute(splits);
        }

        /// <summary>
        /// Loads the commands configuration file, which allows overriding of existing command values.
        /// </summary>
        public static void LoadCommands() {
            string folder = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "Config");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // -- Using an actual command instead of the abstract due to json serilization
            // -- Although we only use the inheritive properties of 'Command'
            List<BanCommand> commands;

            if (!File.Exists(CommandFile)) {
                SaveCommands();
                LoadCommands();
                return;
            } else {
                try {
                    commands = JsonConvert.DeserializeObject<List<BanCommand>>(File.ReadAllText(CommandFile));
                } catch (Exception ex) {
                    Logger.Log(LogType.Warning, "Could not load commands file!");
                    Logger.Log(LogType.Debug, ex.Message);
                    Logger.Log(LogType.Verbose, ex.StackTrace);

                    commands = Commands.Values.Cast<BanCommand>().ToList();
                    SaveCommands();
                }
            }

            // -- Updates the properties of all loaded commands from file.
            foreach (BanCommand command in commands) {
                Command c;

                if (!Commands.TryGetValue(command.CommandString, out c))
                    continue;

                // -- Rebuild the aliases and groups based on if anything changed here.
                RebuildAlias(Commands[command.CommandString].CommandAliases, command.CommandAliases, Commands[command.CommandString]);
                RebuildGroups(Commands[command.CommandString].Group, command.Group);

                Commands[command.CommandString].CommandAliases = command.CommandAliases;
                Commands[command.CommandString].Description = command.Description;
                Commands[command.CommandString].Group = command.Group;
                Commands[command.CommandString].MinRank = command.MinRank;
            }

            _lastModified = File.GetLastWriteTime(CommandFile);
            
        }
        private static void RebuildAlias(string[] old, string[] newA, Command co) {
            List<string> removeList = old.Where(s => !newA.Contains(s)).ToList();
            List<string> addList = newA.Where(s => !old.Contains(s)).ToList();

            // -- Remove any aliases that have now been removed.
            foreach (KeyValuePair<string, Command> c in Commands.Where(c => removeList.Contains(c.Key)).ToList()) {
                Commands.Remove(c.Key);
            }

            // -- Register any new aliases
            foreach(string a in addList) {
                Commands.Add(a, co);
            }
        }

        private static void RebuildGroups(string oldGroup, string newGroup) {
            if (!Groups.Contains(newGroup.ToLower())) // -- If there is a new group..
                Groups.Add(newGroup.ToLower()); // -- Add it.
            else if (!Commands.Any(a => a.Value.Group == oldGroup)) {
                Groups.Remove(oldGroup.ToLower()); // -- If nothing else exists in the old group, remove it.
            }
        }

        public static void SaveCommands() {
            File.WriteAllText(CommandFile, JsonConvert.SerializeObject(Commands.Values, Formatting.Indented));
            Logger.Log(LogType.Info, "Command file saved.");
        }

        /// <summary>
        /// Uses reflection to search the application for all commands and register them.
        /// </summary>
        public static void RegisterInternalCommands() {
            Type[] types = Assembly.GetAssembly(typeof(Command)).GetTypes();
            types = types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Command))).ToArray();

            foreach (Type type in types) {
                var cmd = (Command) Activator.CreateInstance(type);
                RegisterCommand(cmd);
            }
        }

        /// <summary>
        /// Adds a command to the server recognized commands.
        /// </summary>
        /// <param name="c"></param>
        public static void RegisterCommand(Command c) {
            Command existing;

            if (Commands.TryGetValue(c.CommandString, out existing)) {
                Logger.Log(LogType.Warning, $"Attempted to register already existing command: {c.CommandString}");
                return;
            }

            Commands.Add(c.CommandString, c);

            foreach (string aliase in c.CommandAliases) {
                if (Commands.TryGetValue(aliase, out existing))
                    continue;

                Commands.Add(aliase, c);
            }

			if (!Groups.Contains (c.Group.ToLower ()))
				Groups.Add (c.Group.ToLower ());
			
            Logger.Log(LogType.Verbose, $"Registered command {c.CommandString}");
        }

        public override void Setup() {
            LoadCommands();
        }

        public override void Main() {
            if (File.GetLastWriteTime(CommandFile) == _lastModified)
                return;

            LoadCommands();
            Logger.Log(LogType.Info, "Commands file loaded.");
        }

        public override void Teardown() {
            SaveCommands();
        }
    }
}
