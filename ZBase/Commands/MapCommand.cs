using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
    public class MapCommand : Command {
        public MapCommand() {
            CommandString = "map";
            CommandAliases = new[] {"goto", "g", "j"};
            Group = "Map";
            MinRank = 0;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0 || args.Length > 1) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            HcMap newMap;

            if (!HcMap.Maps.TryGetValue(args[0], out newMap)) {
                SendExecutorMessage($"§EMap '{args[0]}' not found.");
                return;
            }

            if (newMap.Showrank > ExecutingClient.ClientPlayer.CurrentRank.Value) {
                SendExecutorMessage($"§EMap '{args[0]}' not found.");
                return;
            }

            if (newMap.Joinrank > ExecutingClient.ClientPlayer.CurrentRank.Value) {
                var joinRank = Rank.GetRank(newMap.Joinrank);
                SendExecutorMessage($"§EYou need to be {joinRank}+ to join this map.");
                return;
            }

            ExecutingClient.ClientPlayer.ChangeMap(newMap);
        }
    }
}
