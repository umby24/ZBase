using System;
using System.Collections.Generic;
using System.Linq;
using ZBase.Common;
using ZBase.Fills;
using ZBase.Network;

namespace ZBase.Commands {
    public class FillCommand : Command {
        public FillCommand() {
            CommandString = "fill";
            CommandAliases = new[] {"mapfill"};
            Group = "Map";
            MinRank = 100;
        }

        public override void Execute(Client executingClient, string[] args) {
            switch (args.Length) {
                case 0:
                    PrintFills(executingClient);
                    break;
                default:
                    FillMap(executingClient, args[0], args);
                    break;
            }
        }

        private void PrintFills(Client c) {
            string mapFillString = FillManager.Fills.Aggregate("§D", (current, value) => current + (" §S" + value.Key + " §D"));

            Chat.SendClientChat("§SMapfills:", 0, c);
            Chat.SendClientChat(mapFillString, 0, c);
        }

        private void FillMap(Client c, string name, string[] args) {
            List<KeyValuePair<string, Mapfill>> fills =
                FillManager.Fills.Where(a => a.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (fills.Count == 0) {
                Chat.SendClientChat($"§EThere is no fill called '{name}'", 0, c);
                return;
            }

            Mapfill myFill = fills.FirstOrDefault().Value;
            myFill.Execute(c.ClientPlayer.CurrentMap, args);
            Chat.SendClientChat("§SFill complete.", 0, c);
        }
    }
}