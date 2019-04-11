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

        public override void Execute(Client c, string[] args) {
            if (args.Length > 0) {
                Chat.SendClientChat("§EThis command does not take any parameters.", 0, c);
                return;
            }

            var blockPos = c.ClientPlayer.Entity.GetBlockCoords();
            blockPos.Z -= 1;

            if (!c.ClientPlayer.CurrentMap.BlockInBounds(blockPos.X, blockPos.Y, blockPos.Z))
                return;

            c.ClientPlayer.CurrentMap.SetBlockId(blockPos.X, blockPos.Y, blockPos.Z,
                c.ClientPlayer.LastMaterial.OnClient);

            Chat.SendClientChat("§SBlock placed.", 0, c);
        }
    }

    public class MaterialCommand : Command {
        public MaterialCommand() {
            CommandString = "material";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 0;
        }

        public override void Execute(Client c, string[] args) {
            if (args.Length != 1) {
                Chat.SendClientChat("§SBuild material reset.", 0, c);
                c.ClientPlayer.Material = null;
                return;
            }

            Block block = BlockManager.GetBlock(args[0]);

            if (block == null) {
                Chat.SendClientChat($"§ECould not find block '{args[0]}'.", 0, c);
                return;
            }

            c.ClientPlayer.Material = block;
        }
    }

    public class MaterialsCommand : Command {
        public MaterialsCommand() {
            CommandString = "materials";
            CommandAliases = new string[0];
            Group = "Build";
            MinRank = 0;
        }

        public override void Execute(Client c, string[] args) {
            if (args.Length > 0) {
                Chat.SendClientChat($"§EInvalid number of arguments.", 0, c);
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
