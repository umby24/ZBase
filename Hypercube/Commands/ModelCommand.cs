namespace ZBase.Commands {
    //public class ModelCommand : Command {
    //    private readonly string[] _playerModels = { 
    //        "chicken",
    //        "creeper",
    //        "croc",
    //        "humanoid",
    //        "pig",
    //        "printer",
    //        "sheep",
    //        "skeleton",
    //        "spider",
    //        "zombie",
    //        "head",
    //        "sitting",
    //        "chibi"
    //    };

    //    public ModelCommand() {
    //        CommandString = "model";
    //        CommandAliases = new string[0];
    //        Group = "General";
    //        MinRank = 0;
    //    }

    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length == 0 || args.Length > 1) {
    //            Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
    //            return;
    //        }

    //        if (!IsValidModel(args[0])) {
    //            Chat.SendClientChat("§EInvalid model type.", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.Entity.Model = args[0];
    //        Chat.SendClientChat("§SModel changed.", 0, c);
    //    }

    //    private bool IsValidModel(string model) {
    //        int blockId = 0;

    //        if (!_playerModels.Contains(model) && !int.TryParse(model, out blockId))
    //            return false;

    //        if (_playerModels.Contains(model))
    //            return true;

    //        return blockId <= 65 && blockId != 0;
    //    }
    //}
}