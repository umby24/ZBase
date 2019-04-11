using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
    public class MapsCommand : Command {
        public MapsCommand() {
            CommandString = "maps";
            CommandAliases = new string[0];
            Group = "General";
            MinRank = 0;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length > 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            // -- Holy linq batman..
            string mapString = HcMap.Maps.Where(a => a.Value.Showrank <= executingClient.ClientPlayer.CurrentRank.Value)
                .ToDictionary(b => b.Key, c => c.Value).
                Keys.Aggregate("§SMaps:<br>", (current, m) => current + ("§S" + m + " §D "));

            Chat.SendClientChat(mapString, 0, executingClient);
        }
    }
}
