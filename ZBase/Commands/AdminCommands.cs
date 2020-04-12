using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.Persistence;
using ZBase.World;

namespace ZBase.Commands {
    public class KickCommand : Command {
        public KickCommand() {
            CommandString = "kick";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) { // -- Kick [name] [message]
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            Client[] toKick = GetOnlineClient(args[0]);

            if (toKick.Length == 0) {
                SendExecutorMessage($"§EUnable to find someone called {args[0]}");
                return;
            }

            string reason = string.Join(" ", args.Skip(1).ToArray());
            var executorRank = ExecutingClient.ClientPlayer.CurrentRank.Value;
            
            foreach (Client client in toKick) {
                if (client.ClientPlayer.CurrentRank.Value >= executorRank) {
                    SendExecutorMessage("§EYou don't have permission to kick this person.");
                    continue;
                }

                client.Kick(reason);
                Chat.SendGlobalChat(
                    $"§S{client.ClientPlayer.PrettyName} was kicked by {ExecutingClient.ClientPlayer.PrettyName}. ({reason})",
                    0, true);
            }
        }
    }

    public class BanCommand : Command {
        public BanCommand() {
            CommandString = "ban";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }

            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            Rank playerRank = Rank.GetRank(dt.Rank);

            if (playerRank.Value >= ExecutingClient.ClientPlayer.CurrentRank.Value) {
                SendExecutorMessage("§EYou don't have permission to ban this person.");
                return;
            }

            Client[] onlineClient = GetOnlineClient(args[0]);

            dt.Banned = true;
            dt.BannedBy = ExecutingClient.ClientPlayer.Name;
            dt.BanMessage = "You are banned!";
            Player.Database.UpdatePlayer(dt);
            
            Chat.SendGlobalChat($"§SPlayer '{args[0]}' has been banned.", 0);

            foreach (var c in onlineClient) {
                c.Kick("You have been banned!");
            }
        }
    }

    public class UnbanCommand : Command {
        public UnbanCommand() {
            CommandString = "unban";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }
            
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            dt.Banned = false;
            dt.BannedUntil = 0;
            Player.Database.UpdatePlayer(dt);

            SendExecutorMessage($"§SPlayer '{args[0]}' has been unbanned.");
        }
    }

    public class StopCommand : Command {
        public StopCommand() {
            CommandString = "stop";
            CommandAliases = new[] {"freeze"};
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }

            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            Rank playerRank = Rank.GetRank(dt.Rank);

            if (playerRank.Value >= ExecutingClient.ClientPlayer.CurrentRank.Value) {
                SendExecutorMessage("§EYou don't have permission to stop this person.");
                return;
            }
            
            dt.Stopped = true;
            Player.Database.UpdatePlayer(dt);
            
            Client[] onlineClient = GetOnlineClient(args[0]);
            foreach (var c in onlineClient) {
                Chat.SendClientChat("§SYou have been stopped.", 0, c);
                c.ClientPlayer.Stopped = true;
            }

            SendExecutorMessage($"§SPlayer '{args[0]}' has been stopped.");
        }
    }

    public class UnstopCommand : Command {
        public UnstopCommand() {
            CommandString = "unstop";
            CommandAliases = new[] {"unfreeze"};
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            dt.Stopped = false;
            Player.Database.UpdatePlayer(dt);

            Client[] onlineClient = GetOnlineClient(args[0]);
            
            foreach (var c in onlineClient) {
                Chat.SendClientChat("§SYou have been un-stopped.", 0, c);
                c.ClientPlayer.Stopped = false;
            }

            SendExecutorMessage($"§SPlayer '{args[0]}' has been un-stopped.");
        }
    }

    public class MuteCommand : Command {
        public MuteCommand() {
            CommandString = "mute";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length < 2) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }

            double intTime;

            if (!double.TryParse(args[1], out intTime)) {
                SendExecutorMessage($"§EInvalid time: '{args[1]}'.");
                return;
            }

            TimeSpan ts = TimeSpan.FromMinutes(intTime);
            DateTime mutedUntil = DateTime.UtcNow + ts; // -- DB: Time_Muted
            double unixTime = (mutedUntil - Utils.GetUnixEpoch()).TotalSeconds;
            
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            dt.TimeMuted = unixTime;
            Player.Database.UpdatePlayer(dt);

            SendExecutorMessage(
                "§SPlayer muted until " + mutedUntil.ToShortTimeString() + " on " + mutedUntil.ToShortDateString());

            Client[] onlineClient = GetOnlineClient(args[0]);

            foreach (Client c in onlineClient) {
                Chat.SendClientChat("§SYou have been muted!", 0, c);
                c.ClientPlayer.MutedUntil = mutedUntil;
            }
        }
    }

    public class UnmuteCommand : Command {
        public UnmuteCommand() {
            CommandString = "unmute";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length < 1) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            dt.TimeMuted = 0;
            Player.Database.UpdatePlayer(dt);

            SendExecutorMessage("§SPlayer unmuted.");

            Client[] onlineClient = GetOnlineClient(args[0]);

            foreach (Client c in onlineClient) {
                Chat.SendClientChat("§SYou may speak again.", 0, c);
                c.ClientPlayer.MutedUntil = Utils.GetUnixEpoch();
            }
        }
    }

    public class TempbanCommand : Command {
        public TempbanCommand() {
            CommandString = "tempban";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length < 2) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }

            double intTime;

            if (!double.TryParse(args[1], out intTime)) {
                SendExecutorMessage($"§EInvalid time: '{args[1]}'.");
                return;
            }

            TimeSpan ts = TimeSpan.FromMinutes(intTime);
            DateTime mutedUntil = DateTime.UtcNow + ts;
            double unixTime = (mutedUntil - Utils.GetUnixEpoch()).TotalSeconds;
            
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            dt.BannedUntil = unixTime;
            Player.Database.UpdatePlayer(dt);

            SendExecutorMessage(
                "§SPlayer banned until " + mutedUntil.ToShortTimeString() + " on " + mutedUntil.ToShortDateString());

            Client[] onlineClient = GetOnlineClient(args[0]);

            foreach (Client c in onlineClient) {
                c.Kick("You have been temporarily banned!");
            }
        }
    }

    public class IpBanCommand : Command {
        public IpBanCommand() {
            CommandString = "ipban";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                SendExecutorMessage($"§ECould not find a player called {args[0]}.");
                return;
            }
            PlayerModel dt = Player.Database.GetPlayerModel(args[0]);
            
            var newBan = new IpBanModel {
                BannedBy = ExecutingClient.ClientPlayer.Name,
                Ip = dt.Ip,
                Reason = Constants.DefaultBanMessage
            };
            
            
            Player.Database.IpBan(newBan);

            foreach (var c in Server.RoClients) {
                if (c.Ip != dt.Ip)
                    continue;

                c.Kick("You have been banned!");
            }

            SendExecutorMessage($"§SPlayers with the ip {dt.Ip} have been banned.");
        }
    }
}