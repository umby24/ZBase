namespace ZBase.Commands {
    //public class SetMapHacks : Command {
    //    public SetMapHacks() {
    //        CommandString = "sethack";
    //        CommandAliases = new string[0];
    //        MinRank = 200;
    //        Group = "Map";
    //        Description = "Usage: /sethack [hack] [value]<br>" +
    //                      "Enables/Disabled certain hacks on this map.<br>" +
    //                      "Valid hacks: fly, noclip, speed, spawn, thirdperson, jump.<br>" +
    //                      "Note that jump takes a numerical value (1-65535).";
    //    }
    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length != 2) {
    //            Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
    //            return;
    //        }

    //        HackPermissions currentPerms = c.ClientPlayer.CurrentMap.Permissions;

    //        switch (args[0].ToLower()) {
    //            case "fly":
    //                currentPerms.Fly = ParseBoolResponse(args[1]);
    //                break;
    //            case "noclip":
    //            case "clip":
    //                currentPerms.NoClip = ParseBoolResponse(args[1]);
    //                break;
    //            case "speed":
    //                currentPerms.Speed = ParseBoolResponse(args[1]);
    //                break;
    //            case "spawn":
    //                currentPerms.Spawn = ParseBoolResponse(args[1]);
    //                break;
    //            case "third":
    //            case "thirdperson":
    //                currentPerms.ThirdPerson = ParseBoolResponse(args[1]);
    //                break;
    //            case "jump":
    //            case "height":
    //                if (!IsInt(args[1])) {
    //                    Chat.SendClientChat($"§EInvalid jump height '{args[1]}'.", 0, c);
    //                    return;
    //                }
    //                currentPerms.JumpHeight = short.Parse(args[1]);
    //                break;
    //            default:
    //                Chat.SendClientChat($"§EInvalid hack type '{args[0]}'.", 0, c);
    //                return;
    //        }

    //        c.ClientPlayer.CurrentMap.SetHackPermissions(currentPerms);
    //        Chat.SendClientChat("§SHack permissions set.", 0, c);
    //    }

    //    private bool ParseBoolResponse(string input) {
    //        if (input.ToLower().Trim() == "on" || input.ToLower().Trim() == "true" || input.ToLower().Trim() == "1")
    //            return true;

    //        return false;
    //    }
    //    private bool IsInt(string input) {
    //        int output;

    //        if (!int.TryParse(input, out output))
    //            return false;

    //        if (output > short.MaxValue || output < short.MinValue)
    //            return false;

    //        return true;
    //    }
    //}

    //public class SetTextureUrl : Command {
    //    public SetTextureUrl() {
    //        CommandString = "texture";
    //        CommandAliases = new string[0];
    //        Group = "Map";
    //        MinRank = 200;
    //        Description = "Usage: /texture [Url]<br>" +
    //                      "Sets the texture pack for this map. If blank, the texture pack will be removed.";
    //    }

    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length != 1) {
    //            c.ClientPlayer.CurrentMap.MapTexturePack = " ";
    //            Chat.SendClientChat("§STexture reset.", 0, c);
    //            return;
    //        }

    //        if (!args[0].StartsWith("http")) {
    //            Chat.SendClientChat("§ETexture URL must start with http", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.CurrentMap.MapTexturePack = args[0];
    //        c.ClientPlayer.CurrentMap.Resend();
    //        Chat.SendClientChat("§STexture set.", 0, c);
    //    }
    //}

    //public class SetMapWeather : Command {
    //    public SetMapWeather() {
    //        CommandString = "weather";
    //        CommandAliases = new string[0];
    //        Group = "Map";
    //        MinRank = 200;
    //        Description = "Usage: /weather [type]<br>" +
    //                      "Valid types are sun, snow, and rain." +
    //                      "If no type is provided, sun is the default.";
    //    }

    //    public override void Execute(Client executingClient, string[] args) {
    //        if (args.Length != 1) {
    //            executingClient.ClientPlayer.CurrentMap.SetWeather(WeatherType.Sunny);
    //            Chat.SendClientChat("§SWeather reset.", 0, executingClient);
    //            return;
    //        }

    //        switch (args[0].ToLower()) {
    //            case "sun":
    //            case "sunny":
    //                executingClient.ClientPlayer.CurrentMap.SetWeather(WeatherType.Sunny);
    //                break;
    //            case "snow":
    //                executingClient.ClientPlayer.CurrentMap.SetWeather(WeatherType.Snowing);
    //                break;
    //            case "rain":
    //                executingClient.ClientPlayer.CurrentMap.SetWeather(WeatherType.Raining);
    //                break;
    //            default:
    //                Chat.SendClientChat($"§EUnknown weather type: {args[0]}", 0, executingClient);
    //                return;
    //        }

    //        Chat.SendClientChat("§SWeather set.", 0, executingClient);
    //    }
    //}

    //public class SetMapColors : Command {
    //    public SetMapColors() {
    //        CommandString = "setcolors";
    //        CommandAliases = new string[0];
    //        Group = "Map";
    //        MinRank = 200;
    //        Description = "Usage: /setcolors [type] [R] [G] [B]<br>" +
    //                      "Types 0-4: sky,cloud, fog, sun, diffuse.<br>" +
    //                      "RGB, valid 0-255.<br>" +
    //                      "Sets the enviroment colors for this map.";
    //    }
    //    public override void Execute(Client executingClient, string[] args) {
    //        if (args.Length != 4) {
    //            var testCuboid = new SelectionCuboid {
    //                SelectionId = 1,
    //                Color = new Vector3S(100, 100, 100),
    //                Start = new Vector3S(50, 50, 50),
    //                End = new Vector3S(75, 75, 75),
    //                Label = "Test",
    //                Opacity = 255
    //            };

    //            executingClient.ClientPlayer.CreateSelection(testCuboid);

    //            Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
    //            return;
    //        }

    //        int type, red, green, blue;
    //        if (!int.TryParse(args[0], out type)) {
    //            Chat.SendClientChat("§EType must be a number.", 0, executingClient);
    //            return;
    //        }
    //        if (!int.TryParse(args[1], out red)) {
    //            Chat.SendClientChat("§ERed must be a number", 0, executingClient);
    //            return;
    //        }
    //        if (!int.TryParse(args[2], out green)) {
    //            Chat.SendClientChat("§EGreen must be a number", 0, executingClient);
    //            return;
    //        }
    //        if (!int.TryParse(args[3], out blue)) {
    //            Chat.SendClientChat("§EBlue must be a number", 0, executingClient);
    //            return;
    //        }

    //        if (type > 4 || type < 0) {
    //            Chat.SendClientChat("§EType must have a value between 0 and 4.", 0, executingClient);
    //            return;
    //        }

    //        if ((red > 255 || red < 0) || (green > 255 || green < 0) || (blue > 255 || blue < 0)) {
    //            Chat.SendClientChat("§ERed, green, and blue must be between 0-255.", 0, executingClient);
    //            return;
    //        }

    //        if (executingClient.ClientPlayer.CurrentMap.EnvColors == null) {
    //            executingClient.ClientPlayer.CurrentMap.ResetEnviromentColors();
    //        }

    //        var colorRef =
    //            executingClient.ClientPlayer.CurrentMap.EnvColors.FirstOrDefault(
    //                a => a.ColorType == (EnviromentColorType) type);

    //        if (colorRef == null) {
    //            Chat.SendClientChat("§EAn error occured.", 0, executingClient);
    //            return;
    //        }

    //        colorRef.Red = (short)red;
    //        colorRef.Green = (short)green;
    //        colorRef.Blue = (short)blue;
    //        executingClient.ClientPlayer.CurrentMap.ResendColors();
    //        Chat.SendClientChat("§SNew colors applied.", 0, executingClient);
    //    }
    //}
    //public class SetProps : Command {
    //    public SetProps() {
    //        CommandString = "setprops";
    //        CommandAliases = new string[0];
    //        Group = "Map";
    //        MinRank = 200;

    //    }

    //    public override void Execute(Client executingClient, string[] args) {
            
    //    }
    //}
}
