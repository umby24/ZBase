using System;
using System.IO;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
    public class AddMapCommand : Command {
        public AddMapCommand() {
            CommandString = "mapadd";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(string[] args) {
            if (args.Length > 1 || args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (HcMap.Maps.ContainsKey(args[0])) { // -- Check if the map already exists.
                ExecutingClient.ClientPlayer.ChangeMap(HcMap.Maps[args[0]]);
                return;
            }

            var mapPath = Path.Combine(ClassicWorldMapProvider.MapDirectory, args[0] + ".cw");
            var newMap = new HcMap(mapPath, args[0], new Vector3S(128, 128, 128));
            HcMap.Maps.Add(newMap.MapProvider.MapName, newMap);

            ExecutingClient.ClientPlayer.ChangeMap(newMap);
            SendExecutorMessage("§SMap created.");
        }
    }

    public class DeleteMapCommand : Command {
        public DeleteMapCommand() {
            CommandString = "mapdelete";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(string[] args) {
            if (args.Length != 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (ExecutingClient.ClientPlayer.Entity.CurrentMap == HcMap.DefaultMap) {
                SendExecutorMessage("§EYou cannot delete the default map!");
                return;
            }

            HcMap toDelete = ExecutingClient.ClientPlayer.Entity.CurrentMap;
            toDelete.Delete();
            SendExecutorMessage("§SMap Deleted.");
        }
    }

    public class ResizeMapCommand : Command {
        public ResizeMapCommand() {
            CommandString = "mapresize";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(string[] args) {
            if (args.Length != 3) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            ExecutingClient.ClientPlayer.Entity.CurrentMap.Resize(new Vector3S(Convert.ToInt16(args[0]), Convert.ToInt16(args[1]), Convert.ToInt16(args[2])));
            SendExecutorMessage("§SMap Resized.");
        }
    }


    public class SaveMapCommand : Command {
        public SaveMapCommand() {
            CommandString = "savemap";
            CommandAliases = new [] {"mapsave"};
            MinRank = 200;
            Group = "Map";
            Description = "Usage: /savemap<br>" +
                          "Forces the current map to save to disk.";
        }

        public override void Execute(string[] args) {
            ExecutingClient.ClientPlayer.Entity.CurrentMap.Save();
            SendExecutorMessage("§SMap saved.");
        }
    }

    public class ResendMapCommand : Command {
        public ResendMapCommand() {
            CommandString = "mapresend";
            CommandAliases = new[] {"resend"};
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(string[] args) {
            ExecutingClient.ClientPlayer.Entity.CurrentMap.Resend();    
        }
    }

    public class MapInfoCommand : Command {
        public MapInfoCommand() {
            CommandString = "minfo";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = -1;
        }
        public override void Execute(string[] args) {
            var yourMap = ExecutingClient.ClientPlayer.Entity.CurrentMap;
            SendExecutorMessage($"§SMap Name: {yourMap.ToString()}");
            SendExecutorMessage($"§SBuild Rank: {Rank.GetRank(yourMap.BuildRank)}");
            SendExecutorMessage($"§Show Rank: {Rank.GetRank(yourMap.Showrank)}");
            SendExecutorMessage($"§SJoin Rank: {Rank.GetRank(yourMap.Joinrank)}");

            //Chat.SendClientChat($"§SIt is currently {yourMap.Weather}", 0, c);
            //Chat.SendClientChat($"§SFly: {yourMap.Permissions.Fly} Clip: {yourMap.Permissions.NoClip} Rewspawn: {yourMap.Permissions.Spawn} Speedhax: {yourMap.Permissions.Speed} Third person: {yourMap.Permissions.ThirdPerson} Jump Height:{yourMap.Permissions.JumpHeight}", 0, c);
            //Chat.SendClientChat($"§SSky: {EnvColorToHex(yourMap.EnvColors[0])} Clouds: {EnvColorToHex(yourMap.EnvColors[1])} Fog: {EnvColorToHex(yourMap.EnvColors[2])} Ambient: {EnvColorToHex(yourMap.EnvColors[3])} Diffuse: {EnvColorToHex(yourMap.EnvColors[4])}", 0, c);
        }

        private string EnvColorToHex(EnviromentColors colors) {
            return "#" + colors.Red.ToString("X2") + colors.Green.ToString("X2") + colors.Blue.ToString("X2");
        }
    }

    public class PermsCommand : Command {
        public PermsCommand() {
            CommandString = "perms";
            Group = "Map";
            MinRank = 200;
            CommandAliases = new string[0];
        }

        public override void Execute(string[] args) {
            if (args.Length != 2) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            int inputVal;

            if (!int.TryParse(args[1], out inputVal)) {
                SendExecutorMessage("§ERank number must be  between -65535 and 65535.");
                return;
            }

            switch (args[0].ToLower()) {
                case "build":
                    ExecutingClient.ClientPlayer.Entity.CurrentMap.BuildRank = (short)inputVal;
                    break;
                case "show":
                    ExecutingClient.ClientPlayer.Entity.CurrentMap.Showrank = (short)inputVal;
                    break;
                case "join":
                    ExecutingClient.ClientPlayer.Entity.CurrentMap.Joinrank = (short)inputVal;
                    break;
                default:
                    SendExecutorMessage("§EUnknown permission '" + args[1] + "'.");
                    return;
            }

            SendExecutorMessage("§SPermission saved.");
        }
    }
}