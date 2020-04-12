using System.Collections.Generic;
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

        public override void Execute(string[] args) {
            if (args.Length > 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            Dictionary<string, HcMap> dictionary = new Dictionary<string, HcMap>();

            foreach (var a in HcMap.Maps) {
                if (a.Value.Showrank <= ExecutingClient.ClientPlayer.CurrentRank.Value) dictionary.Add(a.Key, a.Value);
            }

            var mapString = dictionary.
                Keys.Aggregate("§SMaps:<br>", (current, m) => current + ("§S" + m + " §D "));

            SendExecutorMessage(mapString);
        }
    }
}
