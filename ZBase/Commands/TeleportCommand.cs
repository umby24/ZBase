// -- Created by umby24

using System;
using System.Linq;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Commands {
	public class TeleportCommand : Command {
		public TeleportCommand () {
			CommandString = "tp";
			CommandAliases = new string[0];
			Group = "General";
			MinRank = 0;
		}

		public override void Execute(string[] args) {
			if (args.Length == 0 || args.Length > 1) {
				SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
				return;
			}

			var toTp = Entity.AllEntities.Where(a => String.Equals(a.Name, args [0], StringComparison.CurrentCultureIgnoreCase)).ToArray();

			if (toTp.Length == 0) {
				SendExecutorMessage($"§EUnable to find a player called {args[0]}");
				return;
			}

			ExecutingClient.ClientPlayer.Entity.Location = toTp [0].Location;
			ExecutingClient.ClientPlayer.Entity.SendOwn = true;
			ExecutingClient.ClientPlayer.Entity.HandleMove ();

			SendExecutorMessage("§STeleported.");
		}
	}
}

