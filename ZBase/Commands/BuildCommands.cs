using System.Linq;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Commands {
    public class PlaceCommand : Command {
        public PlaceCommand() {
            CommandString = "place";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 50;
        }

        public override void Execute(string[] args) {
            if (args.Length > 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            var blockPos = ExecutingClient.ClientPlayer.Entity.GetBlockCoords();
            blockPos.Z -= 1;

            if (!ExecutingClient.ClientPlayer.Entity.CurrentMap.BlockInBounds(blockPos.X, blockPos.Y, blockPos.Z))
                return;

            ExecutingClient.ClientPlayer.Entity.CurrentMap.SetBlockId(blockPos.X, blockPos.Y, blockPos.Z,
                ExecutingClient.ClientPlayer.LastMaterial.OnClient);

            SendExecutorMessage("§SBlock placed.");
        }
    }

    public class MaterialCommand : Command {
        public MaterialCommand() {
            CommandString = "material";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 0;
        }

        public override void Execute(string[] args) {
            if (args.Length != 1) {
                SendExecutorMessage("§SBuild material reset.");
                ExecutingClient.ClientPlayer.Material = null;
                return;
            }

            Block block = BlockManager.GetBlock(args[0]);

            if (block == null) {
                SendExecutorMessage($"§ECould not find block '{args[0]}'.");
                return;
            }

            ExecutingClient.ClientPlayer.Material = block;
            SendExecutorMessage("§SBuild material set.");
        }
    }

    public class MaterialsCommand : Command {
        public MaterialsCommand() {
            CommandString = "materials";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 0;
        }

        public override void Execute(string[] args) {
            if (args.Length > 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            //Chat.SendClientChat("§SMaterials:<br>" + joined, 0, c);
        }
    }

    //public class UndoCommand : Command {
    //    public UndoCommand() {
    //        CommandString = "undo";
    //        CommandAliases = new string[0];
    //        MinRank = 0;
    //        Group = "Build";
    //        Description = "§SUndoes changes you have made.<br>§SUsage: /undo [steps <optional>]";
    //    }

    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length == 0) {
    //            c.ClientPlayer.Undo(30000);
    //            Chat.SendClientChat("§SDone.", 0, c);
    //            return;
    //        }

    //        int myInt;

    //        if (!int.TryParse(args[0], out myInt)) {
    //            Chat.SendClientChat("§EUndo steps should be an number.", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.Undo(myInt);
    //        Chat.SendClientChat("§SDone.", 0, c);
    //    }
    //}
    //public class RedoCommand : Command {
    //    public RedoCommand() {
    //        CommandString = "redo";
    //        CommandAliases = new string[0];
    //        MinRank = 0;
    //        Group = "Build";
    //        Description = "§Redoes changes you have undone.<br>§SUsage: /redo [steps <optional>]";
    //    }

    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length == 0) {
    //            c.ClientPlayer.Redo(30000);
    //            Chat.SendClientChat("§SDone.", 0, c);
    //            return;
    //        }

    //        int myInt;

    //        if (!int.TryParse(args[0], out myInt)) {
    //            Chat.SendClientChat("§Redo steps should be an number.", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.Redo(myInt);
    //        Chat.SendClientChat("§SDone.", 0, c);
    //    }
    //}
}
