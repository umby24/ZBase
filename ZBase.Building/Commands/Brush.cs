using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ZBase.BuildModes;
using ZBase.Common;

namespace ZBase.Building.Commands {
    public class Brush : Command {
        public Brush() {
            CommandString = "brush";
            MinRank = 50;
            Group = "Building";
            Description = "Usage: /brush [size]<br>" +
                          "§SMakes your clicks create random blobs of blocks.<br>" +
                          "§SContains an adjustable size, 1-15.";
        }
        public override void Execute(string[] args) {
            if (args.Length != 1) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!int.TryParse(args[0], out int myNum)) {
                SendExecutorMessage("Invalid brush size. Must be a number between 1 and 15.");
                return;
            }

            if (myNum < 1 || myNum > 15) {
                SendExecutorMessage("Invalid brush size. Must be a number between 1 and 15.");
                return;
            }

            var bm = BuildModeManager.Instance.GetBuildmode(Constants.BrushBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            ExecutingClient.ClientPlayer.CurrentState.Set(myNum, 0);
            
            SendExecutorMessage("§SMBuild Mode Brush started. Place or delete a block to brush. /cancel to stop.");
        }
    }
}