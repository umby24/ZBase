// -- Created by umby24

using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Commands {
	public class BringCommand : Command {
		public BringCommand () {
			CommandString = "bring";
			CommandAliases = new string[0];
			Group = "General";
			MinRank = 0;
		}

		public override void Execute(string[] args) {
			if (args.Length == 0 || args.Length > 1) {
				SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
				return;
			}

			Client[] toTp = GetOnlineClient(args[0]);

			if (toTp.Length == 0) {
				SendExecutorMessage($"§EUnable to find a player called {args[0]}");
				return;
			}

			foreach (Client client in toTp) {
				client.ClientPlayer.Entity.Location = ExecutingClient.ClientPlayer.Entity.Location;
				client.ClientPlayer.Entity.SendOwn = true;
				client.ClientPlayer.Entity.HandleMove();
				Chat.SendClientChat($"§STeleported by {ExecutingClient.ClientPlayer.Name}.", 0, client);
			}

			Chat.SendClientChat ("§STeleported.", 0, ExecutingClient);
		}
	}
}

