using System;
using ZBase.Common;
using ZBase.BuildModes;

namespace ZBase.CTF.Commands;

public class CTFCtrl : Command
{
    public CTFCtrl()
    {
        this.CommandString = "ctf";
        this.CommandAliases = new[] { "ctfsetup" };
        this.Description = "Usage: /ctf [command] [args]<br>" +
                            "§SCommands: set, start, stop";
    }
    public override void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            SendExecutorMessage("§EIncorrect number of arguments.");
            SendExecutorMessage(this.Description);
            return;
        }

        switch (args[0].ToLower())
        {
            case "addmap": // -- add a map to the rotation
            case "delmap": // -- remove a map from rotation
            case "set": // -- Multimodal..
                CtfSet(args);
                break;
            case "start":
                Ctfstart(args);
                break;
            case "stop":
                CtfStop(args);
                break;
            default:
                SendExecutorMessage($"Invalid command '{args[0]}'. Valid are set, start, stop.");
                break;
        }
    }

    public void CtfSet(string[] args)
    {
        if (args.Length < 2)
        {
            SendExecutorMessage(TextColors.Error + "Not enough arguments.");
            return;
        }
        if (this.ExecutingClient.ClientPlayer.CurrentRank.Value <= 250)
        {
            SendExecutorMessage("You lack enough privledges to use this command.");
            return;
        }
        switch (args[1].ToLower())
        {
            case "redflag":
                var bm = BuildModeManager.Instance.GetBuildmode(Common.SetFlagBuildMode, ExecutingClient);
                ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
                ExecutingClient.ClientPlayer.CurrentState.Set(1, 0);
                SendExecutorMessage($"{TextColors.Red}Red {TextColors.System}flag set mode enabled. Place a block to set the red flag location.");
                break;
            case "blueflag":
                var bmb = BuildModeManager.Instance.GetBuildmode(Common.SetFlagBuildMode, ExecutingClient);
                ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bmb;
                ExecutingClient.ClientPlayer.CurrentState.Set(0, 0);
                SendExecutorMessage($"{TextColors.Blue}Blue {TextColors.System}flag set mode enabled. Place a block to set the blue flag location.");
                break;
            case "redspawn":
                GameState.Instance.RedSpawn = this.ExecutingClient.ClientPlayer.Entity.Location;
                SendExecutorMessage($"{TextColors.Red}Red {TextColors.System}team spawn updated.");
                break;
            case "bluespawn":
                GameState.Instance.BlueSpawn = this.ExecutingClient.ClientPlayer.Entity.Location;
                SendExecutorMessage($"{TextColors.Blue}Blue {TextColors.System}team spawn updated.");
                break;
            case "defaultspawn":
                GameState.Instance.DefaultSpawn = this.ExecutingClient.ClientPlayer.Entity.Location;
                SendExecutorMessage($"{TextColors.System}Default spawn updated.");
                break;
            case "gamemap":
                GameState.Instance.GameMap = this.ExecutingClient.ClientPlayer.Entity.CurrentMap;
                SendExecutorMessage($"{TextColors.System}Gamemap updated to the current map.");
                break;
            default:
                SendExecutorMessage($"{TextColors.Error}Unknown command '{args[1]}'.");
                break;
        }
    }
    
    public void Ctfstart(string[] args)
    {
        if (GameState.Instance.Running)
        {
            SendExecutorMessage("§EThe game is already running.");
            return;
        }

    }

    public void CtfStop(string[] args)
    {
        if (!GameState.Instance.Running)
        {
            SendExecutorMessage("§EThe game is not running.");
            return;
        }

    }
}
