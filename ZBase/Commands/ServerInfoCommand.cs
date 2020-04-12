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
        public override void Execute(string[] args) {
            SendExecutorMessage("§SServer Info:");
            SendExecutorMessage("§SSoftware: ZBase");
            SendExecutorMessage("§SServer Version: " + Assembly.GetExecutingAssembly().GetName().Version + " on .NET " +
                                Environment.Version + " (" + Environment.OSVersion + ")");
            SendExecutorMessage("§SWritten in C# (From scratch) by umby24.");
        }
    }
}
