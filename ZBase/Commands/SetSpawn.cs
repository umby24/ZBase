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

        public override void Execute(string[] args) {
            if (args.Length > 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            ExecutingClient.ClientPlayer.CurrentMap.SetSpawn(ExecutingClient.ClientPlayer.Entity.Location.GetAsBlockCoords(), ExecutingClient.ClientPlayer.Entity.Location.Look,
                ExecutingClient.ClientPlayer.Entity.Location.Rotation);

            SendExecutorMessage("§SSpawn location updated.");
        }
    }
}