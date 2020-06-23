using ZBase.BuildModes;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Building.Commands {
    public class HSphere : Command {
        public HSphere() {
            CommandString = "hsphere";
            MinRank = 50;
            Group = "Building";
            Description = "Usage: /hsphere [material]<br>" +
                          "§SCreates a hollow sphere in the specified area.<br>" +
                          "§SOptionally replace only a given material";
        }
        public override void Execute(string[] args) {
            if (args.Length > 1) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }
            
            if (args.Length == 1) {
                var replaceMaterial = BlockManager.GetBlock(args[0]);
                if (replaceMaterial == null) {
                    SendExecutorMessage(Constants.InvalidBlockType);
                    return;
                }
                ExecutingClient.ClientPlayer.CurrentState.Set(args[0], 0);
            }

            var bm = BuildModeManager.Instance.GetBuildmode(Constants.SphereBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;

            ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
            ExecutingClient.ClientPlayer.CurrentState.Set(1, 1);
            SendExecutorMessage("§SBuildMode: Hollow Sphere started. First block is center, second defines radius.");
        }
    }
    
    public class Sphere : Command {
        public Sphere() {
            CommandString = "sphere";
            MinRank = 50;
            Group = "Building";
            Description = "Usage: /sphere [material]<br>" +
                          "§SCreates a sphere in the specified area.<br>" +
                          "§SOptionally replace only a given material";
        }
        public override void Execute(string[] args) {
            if (args.Length > 1) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }
            
            if (args.Length == 1) {
                var replaceMaterial = BlockManager.GetBlock(args[0]);
                if (replaceMaterial == null) {
                    SendExecutorMessage(Constants.InvalidBlockType);
                    return;
                }
                ExecutingClient.ClientPlayer.CurrentState.Set(args[0], 0);
            }

            var bm = BuildModeManager.Instance.GetBuildmode(Constants.SphereBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;

            ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
            ExecutingClient.ClientPlayer.CurrentState.Set(0, 1);
            SendExecutorMessage("§SBuildMode: Sphere started. First block is center, second defines radius.");
        }
    }
}