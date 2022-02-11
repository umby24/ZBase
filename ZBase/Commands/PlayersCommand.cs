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

		public override void Execute(string[] args) {
			string onlineString = "§SOnline Players: " + Server.OnlinePlayers + "<br>";

		    onlineString = Entity.AllEntities.Aggregate(onlineString, (current, c) => current + (c.PrettyName + " §D&f "));

            SendExecutorMessage(onlineString);
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

        public override void Execute(string[] args) {
            if (args.Length != 1) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            if (!ClassicubePlayer.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat("§EPlayer not found.", 0, ExecutingClient);
                return;
            }

            var dbEntry = ClassicubePlayer.Database.GetPlayerModel(args[0]);

            var currentRank = Rank.GetRank(dbEntry.Rank);
            var prettyName = currentRank.Prefix + args[0] + currentRank.Suffix;

            var bannedUntil = dbEntry.BannedUntil;
            var mutedUntil = dbEntry.TimeMuted;

            DateTime muteTime = Utils.GetUnixEpoch().AddSeconds(mutedUntil);
            DateTime banTime = Utils.GetUnixEpoch().AddSeconds(bannedUntil);


            SendExecutorMessage("§SPlayer Info: " + prettyName);
            SendExecutorMessage("§Current Rank: " + currentRank);

            if (dbEntry.Stopped)
                SendExecutorMessage("§SPlayer is Stopped.");

            if (banTime > DateTime.UtcNow)
                SendExecutorMessage("§SPlayer is temp-banned until " + banTime.ToShortDateString() + ".");

            if (dbEntry.Banned)
                SendExecutorMessage("§SPlayer is banned.");

            if (muteTime > DateTime.UtcNow)
                SendExecutorMessage("§SPlayer is muted.");
        }
    }
}

