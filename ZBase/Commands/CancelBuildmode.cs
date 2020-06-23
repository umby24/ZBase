using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class CancelBuildmode : Command {
        public CancelBuildmode() {
            CommandString = "cancel";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 0;
            Description = "Usage: /cancel<br>" +
                          "Cancels any active buildmodes.";
        }

        public override void Execute(string[] args) {
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = null;
            ExecutingClient.ClientPlayer.CurrentState.ResendBlocks(ExecutingClient);
            SendExecutorMessage("§SBuildmode cancelled.");
        }
    }
}
