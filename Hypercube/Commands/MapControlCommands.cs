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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length > 1 || args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (HcMap.Maps.ContainsKey(args[0])) { // -- Check if the map already exists.
                executingClient.ClientPlayer.ChangeMap(HcMap.Maps[args[0]]);
                return;
            }

            var newMap = new HcMap(Path.Combine("Maps", args[0] + ".cw"), args[0], new Vector3S(128, 128, 128));
            HcMap.Maps.Add(newMap.MapProvider.MapName, newMap);

            executingClient.ClientPlayer.ChangeMap(newMap);
            Chat.SendClientChat("§SMap created.", 0, executingClient);
        }
    }

    public class DeleteMapCommand : Command {
        public DeleteMapCommand() {
            CommandString = "mapdelete";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length != 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (executingClient.ClientPlayer.CurrentMap == HcMap.DefaultMap) {
                Chat.SendClientChat("§EYou cannot delete the default map!", 0, executingClient);
                return;
            }

            HcMap toDelete = executingClient.ClientPlayer.CurrentMap;
            toDelete.Delete();
            Chat.SendClientChat("§SMap Deleted.", 0, executingClient);
        }
    }

    public class ResizeMapCommand : Command {
        public ResizeMapCommand() {
            CommandString = "mapresize";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length != 3) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            executingClient.ClientPlayer.CurrentMap.Resize(new Vector3S(Convert.ToInt16(args[0]), Convert.ToInt16(args[1]), Convert.ToInt16(args[2])));
            Chat.SendClientChat("§SMap Resized.", 0, executingClient);
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

        public override void Execute(Client c, string[] args) {
            c.ClientPlayer.CurrentMap.Save();
            Chat.SendClientChat("§SMap saved.", 0, c);
        }
    }

    public class ResendMapCommand : Command {
        public ResendMapCommand() {
            CommandString = "mapresend";
            CommandAliases = new[] {"resend"};
            Group = "Map";
            MinRank = 200;
        }

        public override void Execute(Client executingClient, string[] args) {
            executingClient.ClientPlayer.CurrentMap.Resend();    
        }
    }

    public class MapInfoCommand : Command {
        public MapInfoCommand() {
            CommandString = "minfo";
            CommandAliases = new string[0];
            Group = "Map";
            MinRank = -1;
        }
        public override void Execute(Client c, string[] args) {
            var yourMap = c.ClientPlayer.CurrentMap;
            Chat.SendClientChat($"§SMap Name: {yourMap.ToString()}", 0, c);
            Chat.SendClientChat($"§SBuild Rank: {Rank.GetRank(yourMap.BuildRank)}", 0, c);
            Chat.SendClientChat($"§Show Rank: {Rank.GetRank(yourMap.Showrank)}", 0, c);
            Chat.SendClientChat($"§SJoin Rank: {Rank.GetRank(yourMap.Joinrank)}", 0, c);

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

        public override void Execute(Client c, string[] args) {
            if (args.Length != 2) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, c);
                return;
            }

            int inputVal;

            if (!int.TryParse(args[1], out inputVal)) {
                Chat.SendClientChat("§ERank number must be  between -65535 and 65535.", 0, c);
                return;
            }

            switch (args[0].ToLower()) {
                case "build":
                    c.ClientPlayer.CurrentMap.BuildRank = (short)inputVal;
                    break;
                case "show":
                    c.ClientPlayer.CurrentMap.Showrank = (short)inputVal;
                    break;
                case "join":
                    c.ClientPlayer.CurrentMap.Joinrank = (short)inputVal;
                    break;
                default:
                    Chat.SendClientChat("§EUnknown permission '" + args[1] + "'.", 0, c);
                    return;
            }

            Chat.SendClientChat("§SPermission saved.", 0, c);
        }
    }
}