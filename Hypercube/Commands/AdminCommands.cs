using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
    public class KickCommand : Command {
        public KickCommand() {
            CommandString = "kick";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client executingClient, string[] args) { // -- Kick [name] [message]
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            Client[] toKick = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower()).ToArray();

            if (toKick.Length == 0) {
                Chat.SendClientChat("§EUnable to find someone called " + args[0], 0, executingClient);
                return;
            }

            string reason = string.Join(" ", args.Skip(1).ToArray());

            foreach (Client client in toKick) {
                if (client.ClientPlayer.CurrentRank.Value >= executingClient.ClientPlayer.CurrentRank.Value) {
                    Chat.SendClientChat("§EYou don't have permission to kick this person.", 0, executingClient);
                    continue;
                }

                client.Kick(reason);
                Chat.SendGlobalChat(
                    $"§S{client.ClientPlayer.PrettyName} was kicked by {executingClient.ClientPlayer.PrettyName}. ({reason})",
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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            DataTable dt = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{args[0]}' LIMIT 1");
            Rank playerRank = Rank.GetRank((int) dt.Rows[0]["Rank"]);

            if (playerRank.Value >= executingClient.ClientPlayer.CurrentRank.Value) {
                Chat.SendClientChat("§EYou don't have permission to ban this person.", 0, executingClient);
                return;
            }

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();

            Player.Database.SetDatabase(args[0], "PlayerDB", "Banned", 1);
            Player.Database.SetDatabase(args[0], "PlayerDB", "BannedBy", executingClient.ClientPlayer.Name);
            Player.Database.SetDatabase(args[0], "PlayerDB", "BanMessage", "You are banned!");

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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            Player.Database.SetDatabase(args[0], "PlayerDB", "Banned", 0);
            Player.Database.SetDatabase(args[0], "PlayerDB", "BannedUntil", 0);

            Chat.SendClientChat($"§SPlayer '{args[0]}' has been unbanned.", 0, executingClient);
        }
    }

    public class StopCommand : Command {
        public StopCommand() {
            CommandString = "stop";
            CommandAliases = new[] {"freeze"};
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            DataTable dt = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{args[0]}' LIMIT 1");
            Rank playerRank = Rank.GetRank((int) dt.Rows[0]["Rank"]);

            if (playerRank.Value >= executingClient.ClientPlayer.CurrentRank.Value) {
                Chat.SendClientChat("§EYou don't have permission to stop this person.", 0, executingClient);
                return;
            }

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();
            Player.Database.SetDatabase(args[0], "PlayerDB", "Stopped", 1);
            Player.Database.SetDatabase(args[0], "PlayerDB", "StoppedBy", executingClient.ClientPlayer.Name);
            Player.Database.SetDatabase(args[0], "PlayerDB", "StopMessage", "You have been stopped.");

            Chat.SendClientChat($"§SPlayer '{args[0]}' has been stopped.", 0, executingClient);

            foreach (var c in onlineClient) {
                Chat.SendClientChat("§SYou have been stopped.", 0, c);
                c.ClientPlayer.Stopped = true;
            }
        }
    }

    public class UnstopCommand : Command {
        public UnstopCommand() {
            CommandString = "unstop";
            CommandAliases = new[] {"unfreeze"};
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();
            Player.Database.SetDatabase(args[0], "PlayerDB", "Stopped", 0);

            Chat.SendClientChat($"§SPlayer '{args[0]}' has been un-stopped.", 0, executingClient);

            foreach (var c in onlineClient) {
                Chat.SendClientChat("§SYou have been un-stopped.", 0, c);
                c.ClientPlayer.Stopped = false;
            }
        }
    }

    public class MuteCommand : Command {
        public MuteCommand() {
            CommandString = "mute";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length < 2) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            double intTime;

            if (!double.TryParse(args[1], out intTime)) {
                Chat.SendClientChat($"§EInvalid time: '{args[1]}'.", 0, executingClient);
                return;
            }

            TimeSpan ts = TimeSpan.FromMinutes(intTime);
            DateTime mutedUntil = DateTime.UtcNow + ts; // -- DB: Time_Muted
            double unixTime = (mutedUntil - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            Player.Database.SetDatabase(args[0], "PlayerDB", "Time_Muted", (int) unixTime);
            Chat.SendClientChat(
                "§SPlayer muted until " + mutedUntil.ToShortTimeString() + " on " + mutedUntil.ToShortDateString(), 0,
                executingClient);

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();

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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length < 1) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            Player.Database.SetDatabase(args[0], "PlayerDB", "Time_Muted", 0);
            Chat.SendClientChat("§SPlayer unmuted.", 0, executingClient);

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();

            foreach (Client c in onlineClient) {
                Chat.SendClientChat("§SYou may speak again.", 0, c);
                c.ClientPlayer.MutedUntil = new DateTime(1970, 1, 1);
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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length < 2) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            double intTime;

            if (!double.TryParse(args[1], out intTime)) {
                Chat.SendClientChat($"§EInvalid time: '{args[1]}'.", 0, executingClient);
                return;
            }

            TimeSpan ts = TimeSpan.FromMinutes(intTime);
            DateTime mutedUntil = DateTime.UtcNow + ts;
            double unixTime = (mutedUntil - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            Player.Database.SetDatabase(args[0], "PlayerDB", "BannedUntil", (int) unixTime);
            Chat.SendClientChat(
                "§SPlayer banned until " + mutedUntil.ToShortTimeString() + " on " + mutedUntil.ToShortDateString(), 0,
                executingClient);

            Client[] onlineClient = Server.RoClients.Where(a => a.ClientPlayer.Name.ToLower() == args[0].ToLower())
                .ToArray();

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

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, executingClient);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat($"§ECould not find a player called {args[0]}.", 0, executingClient);
                return;
            }

            DataTable dt = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{args[0]}' LIMIT 1");
            string ipToBan = (string) dt.Rows[0]["IP"];
            Player.Database.IpBan(ipToBan);

            foreach (var c in Server.RoClients) {
                if (c.Ip != ipToBan)
                    continue;

                c.Kick("You have been banned!");
            }

            Chat.SendClientChat($"§SPlayers with the ip {ipToBan} have been banned.", 0, executingClient);
        }
    }
}