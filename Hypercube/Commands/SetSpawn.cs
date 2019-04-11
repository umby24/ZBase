using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class SetSpawn : Command {
        public SetSpawn() {
            CommandString = "setspawn";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 100;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length > 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            executingClient.ClientPlayer.CurrentMap.SetSpawn(executingClient.ClientPlayer.Entity.Location.GetAsBlockCoords(), executingClient.ClientPlayer.Entity.Location.Look,
                executingClient.ClientPlayer.Entity.Location.Rotation);

            Chat.SendClientChat("§SSpawn location updated.", 0, executingClient);
        }
    }
}