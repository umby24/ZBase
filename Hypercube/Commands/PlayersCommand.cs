// -- Created by umby24

using System;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
	public class PlayersCommand : Command {
		public PlayersCommand () {
			CommandString = "players";
			CommandAliases = new[] { "who" };
			Group = "General";
			MinRank = 0;
		}

		public override void Execute(Client executingPlayer, string[] args) {
			string onlineString = "§SOnline Players: " + Server.OnlinePlayers + "<br>";

		    onlineString = Entity.AllEntities.Aggregate(onlineString, (current, c) => current + (c.PrettyName + " §D&f "));

		    Chat.SendClientChat (onlineString, 0, executingPlayer);
		}
	}

    public class PlayerInfoCommand : Command {
        public PlayerInfoCommand() {
            CommandString = "pinfo";
            CommandAliases = new string[0];
            MinRank = 0;
            Group = "General";
            Description = "Usage: /pinfo [player]<br>" +
                          "Displays information about a player from the PlayerDB.";
        }

        public override void Execute(Client c, string[] args) {
            if (args.Length != 1) {
                Chat.SendClientChat("§EIncorrect number of arguments.", 0, c);
                return;
            }

            if (!Player.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat("§EPlayer not found.", 0, c);
                return;
            }

            var dbEntry = Player.Database.GetDataTable($"SELECT * FROM PlayerDB WHERE Name='{args[0]}' LIMIT 1");
            var currentRank = Rank.GetRank(Convert.ToInt32(dbEntry.Rows[0]["Rank"]));
            var prettyName = currentRank.Prefix + args[0] + currentRank.Suffix;
            var stopped = Convert.ToBoolean(dbEntry.Rows[0]["Stopped"]);
            var banned = Convert.ToBoolean(dbEntry.Rows[0]["Banned"]);

            long bannedUntil = (long) dbEntry.Rows[0]["BannedUntil"];
            var mutedUntil = (long) dbEntry.Rows[0]["Time_Muted"];

            DateTime muteTime =
                (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(mutedUntil));
            DateTime banTime =
                (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(bannedUntil));


            Chat.SendClientChat("§SPlayer Info: " + prettyName, 0, c);
            Chat.SendClientChat("§Current Rank: " + currentRank, 0, c);

            if (stopped)
                Chat.SendClientChat("§SPlayer is Stopped.", 0, c);

            if (banTime > DateTime.UtcNow)
                Chat.SendClientChat("§SPlayer is temp-banned until " + banTime.ToShortDateString() + ".", 0, c);

            if (banned)
                Chat.SendClientChat("§SPlayer is banned.", 0, c);

            if (muteTime > DateTime.UtcNow)
                Chat.SendClientChat("§SPlayer is muted.", 0, c);
        }
    }
}

