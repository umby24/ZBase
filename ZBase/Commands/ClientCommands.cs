using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class ClientInfoCommand : Command {
        public ClientInfoCommand() {
            CommandString = "client";
            CommandAliases = new string[0];
            Group = "General";
            MinRank = -1;
        }
        public override void Execute(string[] args) {
            SendExecutorMessage($"§SHello {ExecutingClient.ClientPlayer.Entity.PrettyName}!");
            //Chat.SendClientChat($"§SYou are running {c.App} supporting {c.ExtensionsCount} CPE Extensions.", 0, c);
            //Chat.SendClientChat($"§SExtensions:&f {string.Join(" §D ", c.Extensions.Keys)}", 0, c);
        }
    }
}
