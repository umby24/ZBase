// -- Created by umby24

using System;
using System.Data;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
    public class RankCommand : Command {
        public int MinSetRank { get; set; }

        public RankCommand() {
            CommandString = "rank";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 0;
            MinSetRank = 150;
        }

        public override void Execute(Client c, string[] args) {
            if (args.Length == 0 || args.Length > 2) {
                Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
                return;
            }

            if (args.Length == 1) {
                DisplayRank(c, args[0]);
            } else if (args.Length == 2) {
                if (AdditionalRank(c))
                    SetRank(c, args[0], args[1]);
            }
        }

        /// <summary>
        /// Handles messaging for parts of the command that are rank restricted.
        /// </summary>
	    private bool AdditionalRank(Client c) {
            if (c.ClientPlayer.CurrentRank.Value <= MinSetRank) {
                Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
                return false;
            }

            return true;
        }

        private static void DisplayRank(Client c, string username) {
            if (!Player.Database.ContainsPlayer(username)) {
                Chat.SendClientChat($"§ECould not find a player called {username}", 0, c);
                return;
            }

            DataTable dt = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{username}' LIMIT 1");
            Rank rnk = Rank.GetRank(Convert.ToInt32(dt.Rows[0]["Rank"]));

            Chat.SendClientChat($"§SUser {username} is {rnk.Name}({rnk.Value})", 0, c);
        }

        private bool SetSanityCheck(Client c, string name, Rank currentRank, Rank rnk) {
            // -- Don't allow setting someone to a rank higher than your own
            if (rnk.Value >= c.ClientPlayer.CurrentRank.Value) {
                Chat.SendClientChat("§EYou cannot set someone's rank equal to or higher than your own.", 0, c);
                return false;
            }
            // -- Don't allow changing your own rank
            if (name == c.ClientPlayer.Name) {
                Chat.SendClientChat("§You cannot modify your own rank.", 0, c);
                return false;
            }
            // -- Don't allow modifying someone with higher rank.   
            if (currentRank.Value >= c.ClientPlayer.CurrentRank.Value) {
                Chat.SendClientChat("§EYou cannot change someone's rank if they're higher than you.", 0, c);
                return false;
            }

            return true;
        }

        private static Rank GetPlayerRank(string name) {
            DataTable dbEntry = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{name}' LIMIT 1");
            return Rank.GetRank(Convert.ToInt32(dbEntry.Rows[0]["Rank"]));
        }

        private void SetRank(Client c, string username, string rank) {
            if (!Player.Database.ContainsPlayer(username)) {
                Chat.SendClientChat($"§ECould not find a player called {username}", 0, c);
                return;
            }

            int rnkNumber;

            if (!int.TryParse(rank, out rnkNumber)) {
                Chat.SendClientChat("§ERank must be a number between -65535 and 65535.", 0, c);
                return;
            }

            if (rnkNumber > 65535 || rnkNumber < -65535) {
                Chat.SendClientChat("§ERank must be a number between -65535 and 65535.", 0, c);
                return;
            }

            Rank rnk = Rank.GetRank(rnkNumber);

            if (!SetSanityCheck(c, username, GetPlayerRank(username), rnk)) {
                return;
            }

            Player.Database.SetDatabase(username, "PlayerDB", "Rank", rnkNumber);

            Chat.SendClientChat($"§SPlayer {username} is now ranked {rnk.Name}({rnkNumber})", 0, c);

            // -- Check if this player is online.
            Client thisPlayer = Server.RoClients.FirstOrDefault(a => a.ClientPlayer.Name == username);
            thisPlayer?.ClientPlayer.ReloadDb();

            if (thisPlayer != null)
                Chat.SendClientChat($"§SYour rank was changed to {rnk.Name} ({rnkNumber}) by {c.ClientPlayer.PrettyName}.", 0, c);
        }
    }
}

