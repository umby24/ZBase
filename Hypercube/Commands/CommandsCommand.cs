﻿// -- Created by umby24

using System.Collections.Generic;
using System.Linq;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class CommandsCommand : Command {
        public CommandsCommand() {
            CommandString = "commands";
            CommandAliases = new[] { "c", "help", "cmds" };
            Group = "General";
            MinRank = 0;
        }


        public override void Execute(Client executingPlayer, string[] args) {
            switch (args.Length) {
                case 0:
                    // -- Display all command groups.
                    SendGroups(executingPlayer);
                    return;
                case 1:
                    if (args[0].ToLower() == "all") {
                        SendAllCommands(executingPlayer);
                        return;
                    }

                    if (!CommandHandler.Groups.Contains(args[0].ToLower())) {
                        Chat.SendClientChat($"§EInvalid command group: {args[0]}", 0, executingPlayer);
                        return;
                    }

                    SendGroup(executingPlayer, args[0]);
                    return;
                default:
                    Chat.SendClientChat("§EInvalid number of arguments.", 0, executingPlayer);
                    return;
            }

        }

        public void SendGroups(Client c) {
            string commandString = CommandHandler.Groups.Aggregate("§SCommand groups:<br>&a    All", (current, grp) => current + ("<br>&a    " + grp.UpperFirst()));

            Chat.SendClientChat(commandString, 0, c);
        }

        public void SendAllCommands(Client c) {
            var commandString = "§D&f ";
            var currentLen = 5;

            foreach (string b in CommandHandler.Commands.Keys) {
                if ((b + " §D ").Length + currentLen >= 59) {
                    commandString += "<br>§D " + b + " §D ";
                    currentLen = ("§D " + b + " §D ").Length;
                } else {
                    commandString += b + " §D ";
                    currentLen += (b + " §D ").Length;
                }
            }

            Chat.SendClientChat("&aAll Commands:<br>" + commandString, 0, c);
        }

        public void SendGroup(Client c, string groupName) {
            IEnumerable<KeyValuePair<string, Command>> cmds = CommandHandler.Commands.Where(a => a.Value.Group.ToLower() == groupName.ToLower());
            var commandString = "§D ";
            var currentLen = 5;

            foreach (KeyValuePair<string, Command> cmd in cmds) {
                string thisCmd = cmd.Key + " §D ";

                if (thisCmd.Length + currentLen >= 59) {
                    commandString += "<br>§D " + thisCmd;
                    currentLen = ("<br>§D " + thisCmd).Length;
                } else {
                    commandString += thisCmd;
                    currentLen += thisCmd.Length;
                }
            }

            Chat.SendClientChat("&aGroup " + groupName, 0, c);
            Chat.SendClientChat(commandString, 0, c);
        }
    }
    public class CommandHelp : Command {
        public CommandHelp() {
            CommandString = "cmdhelp";
            CommandAliases = new string[0];
            MinRank = -1;
            Group = "General";
            Description = "Usage: /cmdhelp [command]. For a list of commands, see /commands.";
        }

        public override void Execute(Client c, string[] args) {
            if (args.Length != 1) {
                Chat.SendClientChat("§S" + Description, 0, c);
                return;
            }

            if (!CommandHandler.Commands.ContainsKey(args[0])) {
                Chat.SendClientChat($"§ECommand '{args[0]}' not found.", 0, c);
                return;
            }

            Chat.SendClientChat("§S" + CommandHandler.Commands[args[0]].Description, 0, c);
        }
    }
}

