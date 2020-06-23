using ZBase.BuildModes;
using ZBase.Common;

namespace ZBase.Building.Commands {
    public class HBox : Command {
        public HBox() {
            CommandString = "hbox";
            MinRank = 50;
            Group = "Building";
            Description = "Usage: /hbox [material]<br>" +
                          "§SFill in an area with blocks and a hollow center<br>" +
                          "§SOptionally replace only a material";
        }
        
        public override void Execute(string[] args) {
            if (args.Length > 1) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }
            
            
            var bm = BuildModeManager.Instance.GetBuildmode(Constants.BoxBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            
            if (args.Length == 1) {
                var material = BlockManager.GetBlock(args[0]);
                if (material == null) {
                    SendExecutorMessage(Constants.InvalidBlockType);
                    return;
                }
                ExecutingClient.ClientPlayer.CurrentState.Set(args[0], 0);
            }
            
            ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
            ExecutingClient.ClientPlayer.CurrentState.Set(1, 1);
            SendExecutorMessage("§SBuildMode: Hollow Box started.<br>§SPlace two blocks to fill an area.");
        }
    }
    
    public class Box : Command {
        public Box() {
            CommandString = "box";
            MinRank = 50;
            Group = "Build";
            Description = "Usage: /box [material]<br>" +
                          "Fill in an area with blocks<br>" +
                          "Optionally replace only a material";
        }
        
        public override void Execute(string[] args) {
            if (args.Length > 1) {
                SendExecutorMessage(Common.Constants.InvalidNumArgumentsMessage);
                return;
            }
            
            
            var bm = BuildModeManager.Instance.GetBuildmode(Constants.BoxBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            
            if (args.Length == 1) {
                var material = BlockManager.GetBlock(args[0]);
                if (material == null) {
                    SendExecutorMessage("§EInvalid block type.");
                    return;
                }
                ExecutingClient.ClientPlayer.CurrentState.Set(args[0], 0);
            }
            
            ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
            ExecutingClient.ClientPlayer.CurrentState.Set(0, 1);
            SendExecutorMessage("§SBuildMode: Box started.<br>Place two blocks to fill an area.");
        }
    }
}