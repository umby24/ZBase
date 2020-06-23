using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ZBase.BuildModes;
using ZBase.Common;

namespace ZBase.Building.Commands {
    public class Line : Command {
        public Line() {
            CommandString = "line";
            MinRank = 50;
            Group = "Build";
            Description = "Usage: /line<br>" +
                          "Build a line between two points";
        }
        public override void Execute(string[] args) {
            if (args.Length != 0) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }

            var bm = BuildModeManager.Instance.GetBuildmode(Constants.LineBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            
            ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
            SendExecutorMessage("§SBuildmode: Line started. Place two blocks to build a line.");
        }
    }
}