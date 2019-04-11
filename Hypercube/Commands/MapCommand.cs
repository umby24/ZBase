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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0 || args.Length > 1) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            HcMap newMap;

            if (!HcMap.Maps.TryGetValue(args[0], out newMap)) {
                Chat.SendClientChat($"§EMap '{args[0]}' not found.", 0, executingClient);
                return;
            }

            if (newMap.Showrank > executingClient.ClientPlayer.CurrentRank.Value) {
                Chat.SendClientChat($"§EMap '{args[0]}' not found.", 0, executingClient);
                return;
            }

            if (newMap.Joinrank > executingClient.ClientPlayer.CurrentRank.Value) {
                var joinRank = Rank.GetRank(newMap.Joinrank);
                Chat.SendClientChat($"§EYou need to be {joinRank}+ to join this map.", 0, executingClient);
                return;
            }

            executingClient.ClientPlayer.ChangeMap(newMap);
        }
    }
}
