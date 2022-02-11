// -- Created by umby24

using System;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.Persistence;
using ZBase.World;

namespace ZBase.Commands {
    public class RankCommand : Command {
        private const string InvalidRankMessage = "§ERank must be a number between -65535 and 65535.";
        public int MinSetRank { get; set; }
        
        public RankCommand() {
            CommandString = "rank";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 0;
            MinSetRank = 150;
        }

        public override void Execute(string[] args) {
            if (args.Length == 0 || args.Length > 2) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            switch (args.Length) {
                case 1:
                    DisplayRank(args[0]);
                    break;
                case 2: {
                    if (AdditionalRank())
                        SetRank( args[0], args[1]);
                    break;
                }
            }
        }


        
        /// <summary>
        /// Handles messaging for parts of the command that are rank restricted.
        /// </summary>
	    private bool AdditionalRank() {
            if (ExecutingClient.ClientPlayer.CurrentRank.Value <= MinSetRank) {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return false;
            }

            return true;
        }

        private PlayerModel GetPlayerModel(string username) {
            if (!ClassicubePlayer.Database.ContainsPlayer(username)) {
                SendExecutorMessage($"§ECould not find a player called {username}");
                return null;
            }

            PlayerModel playerModel = ClassicubePlayer.Database.GetPlayerModel(username);
            return playerModel;
        }
        
        private void DisplayRank(string username) {
            PlayerModel playerEntry = GetPlayerModel(username);

            if (playerEntry == null)
                return;
            
            var rnk = Rank.GetRank(playerEntry.Rank);
            SendExecutorMessage($"§SUser {username} is {rnk.Name}({rnk.Value})");
        }

        private bool SetSanityCheck(string name, Rank currentRank, Rank rnk) {
            var executersRank = ExecutingClient.ClientPlayer.CurrentRank.Value;
            
            // -- Don't allow setting someone to a rank higher than your own
            if (rnk.Value >= executersRank) {
                SendExecutorMessage("§EYou cannot set someone's rank equal to or higher than your own.");
                return false;
            }
            // -- Don't allow changing your own rank
            if (name == ExecutingClient.ClientPlayer.Name) {
                SendExecutorMessage("§You cannot modify your own rank.");
                return false;
            }
            
            // -- Don't allow modifying someone with higher rank.   
            if (currentRank.Value >= executersRank) {
                SendExecutorMessage("§EYou cannot change someone's rank if they're higher than you.");
                return false;
            }

            return true;
        }

        private static Rank GetPlayerRank(string name) {
            PlayerModel playerEntry = ClassicubePlayer.Database.GetPlayerModel(name);
            return Rank.GetRank(playerEntry.Rank);
        }

        private void SetRank(string username, string rank) {
            PlayerModel playerEntry = GetPlayerModel(username);

            if (playerEntry == null)
                return;
            
            if (!int.TryParse(rank, out var rnkNumber)) {
                SendExecutorMessage(InvalidRankMessage);
                return;
            }

            if (rnkNumber > 65535 || rnkNumber < -65535) {
                SendExecutorMessage(InvalidRankMessage);
                return;
            }

            var rnk = Rank.GetRank(rnkNumber);

            if (!SetSanityCheck(username, GetPlayerRank(username), rnk)) {
                return;
            }
            
            playerEntry.Rank = (short)rnkNumber;
            ClassicubePlayer.Database.UpdatePlayer(playerEntry);


            SendExecutorMessage($"§SPlayer {username} is now ranked {rnk.Name}({rnkNumber})");

            // -- Check if this player is online.
            Client thisPlayer = Server.RoClients.FirstOrDefault(a => String.Equals(a.ClientPlayer.Name, username, StringComparison.CurrentCultureIgnoreCase));
            thisPlayer?.ClientPlayer.LoadDbInfo();

            if (thisPlayer != null)
                Chat.SendClientChat($"§SYour rank was changed to {rnk.Name} ({rnkNumber}) by {ExecutingClient.ClientPlayer.Entity.PrettyName}.", 0, thisPlayer);
        }
    }
}

