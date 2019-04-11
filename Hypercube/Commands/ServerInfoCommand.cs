using System;
using System.Reflection;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class ServerInfoCommand : Command {
        public ServerInfoCommand() {
            CommandString = "ServerInfo";
            CommandAliases = new string[0];
			Group = "General";
			MinRank = 0;
        }
        public override void Execute(Client executingPlayer, string[] args) {
            Chat.SendClientChat("§SServer Info:", 0, executingPlayer);
            Chat.SendClientChat("§SSoftware: ZBase", 0, executingPlayer);
			Chat.SendClientChat("§SServer Version: " + Assembly.GetExecutingAssembly().GetName().Version + " on .NET " + Environment.Version + " (" + Environment.OSVersion + ")", 0, executingPlayer);
			Chat.SendClientChat("§SWritten in C# (From scratch) by umby24.", 0, executingPlayer);
        }
    }
}
