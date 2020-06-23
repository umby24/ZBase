using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ZBase.BuildModes;
using ZBase.Common;

namespace ZBase.Building.Commands {
    public class Paint : Command {
        public Paint() {
            CommandString = "paint";
            MinRank = 50;
            Group = "Build";
            Description = "Usage: /paint<br>" +
                          "§SBreaking blocks replaces them with what you're holding";
        }
        
        public override void Execute(string[] args) {
            if (args.Length > 0) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }
            var bm = BuildModeManager.Instance.GetBuildmode(Constants.PaintBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            SendExecutorMessage("§SBuildmode: Paint started. Break a block to replace it with what you're holding.<br>§SType /cancel to stop.");
        }
    }
}