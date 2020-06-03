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

        public override void Execute(string[] args) {
            switch (args.Length) {
                case 0:
                    PrintFills();
                    break;
                default:
                    FillMap(args[0], args);
                    break;
            }
        }

        private void PrintFills() {
            string mapFillString = FillManager.Fills.Aggregate("§D", (current, value) => current + (" §S" + value.Key + " §D"));

            SendExecutorMessage("§SMapfills:");
            SendExecutorMessage(mapFillString);
        }

        private void FillMap(string name, string[] args) {
            List<KeyValuePair<string, Mapfill>> fills =
                FillManager.Fills.Where(a => a.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (fills.Count == 0) {
                SendExecutorMessage($"§EThere is no fill called '{name}'");
                return;
            }

            Mapfill myFill = fills.FirstOrDefault().Value;
            myFill.Execute(ExecutingClient.ClientPlayer.Entity.CurrentMap, args);
            SendExecutorMessage("§SFill complete.");
        }
    }
}