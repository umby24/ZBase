// -- Created by umby24

using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
	public class TeleportCommand : Command {
		public TeleportCommand () {
			CommandString = "tp";
			CommandAliases = new string[0];
			Group = "General";
			MinRank = 0;
		}

		public override void Execute(Client executingPlayer, string[] args) {
			if (args.Length == 0 || args.Length > 1) {
				Chat.SendClientChat ("§EIncorrect number of arguments.", 0, executingPlayer);
				return;
			}

			Entity[] toTp = executingPlayer.ClientPlayer.Entities.Where (a => a.Name.ToLower () == args [0].ToLower ()).ToArray();

			if (toTp.Length == 0) {
				Chat.SendClientChat ($"§EUnable to find a player called {args[0]}", 0, executingPlayer);
				return;
			}

			executingPlayer.ClientPlayer.Entity.Location = toTp [0].Location;
			executingPlayer.ClientPlayer.Entity.SendOwn = true;
			executingPlayer.ClientPlayer.Entity.HandleMove ();

			Chat.SendClientChat ("§STeleported.", 0, executingPlayer);
		}
	}
}

