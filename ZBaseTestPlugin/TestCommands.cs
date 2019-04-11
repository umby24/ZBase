using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBaseTestPlugin {
    public class PerfCommand : Command {
        public PerfCommand() {
            CommandString = "perf";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client executingClient, string[] args) {
            if (args.Length == 0 || args[0] == "block") {
                Perf_BlockTest(executingClient);
                return;
            }
        }

        private void Perf_BlockTest(Client c) {
            Stopwatch sw = Stopwatch.StartNew();
            for (var x = 0; x < 127; x++) {
                for (var y = 0; y < 127; y++) {
                        c.ClientPlayer.HandleBlockPlace(new Vector3S(x, y, 100), 15, 1);
                    
                }
            }
            
            sw.Stop();
            Chat.SendGlobalChat("&cDone in " + sw.Elapsed.TotalSeconds + " seconds.", 0);
        }
    }

    public class DestroyEntities : Command {
        public DestroyEntities() {
            CommandString = "dtest";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }

        public override void Execute(Client c, string[] args) {
            Entity[] copy = Entity.AllEntities.ToArray();
            
            foreach (Entity e in copy) {
                if (e.Name == "Num1" || e.Name == "Num2") {
                    e.Despawn();
                }
            }
            
            Chat.SendClientChat("&cEntities destroyed.", 0, c);
        }
    }
    
    public class CreateEntity : Command {
        public CreateEntity() {
            CommandString = "etest";
            CommandAliases = new string[0];
            Group = "Op";
            MinRank = 150;
        }
        
        public override void Execute(Client c, string[] args) {
            Test_CreateEntity();
            Chat.SendClientChat("&cEntities created.", 0, c);
        }
        
        private void Test_CreateEntity() {
            Entity[] copy = Entity.AllEntities.ToArray();
            
            foreach (Entity e in copy) {
                if (e.Name == "Num1" || e.Name == "Num2") {
                    e.Despawn();
                }
            }

            var location1 = new MinecraftLocation();
            location1.SetAsBlockCoords(new Vector3S(40, 40, 20));

            var entity1 = new Entity {
                Name = "Num1",
                PrettyName = "&cNum1",
                CurrentMap = HcMap.DefaultMap,
                ClientId = (sbyte) HcMap.DefaultMap.GetEntityId(),
                Location = location1,
            };

            var entity2 = new Entity {
                Name = "Num2",
                PrettyName = "&cNum2",
                CurrentMap = HcMap.DefaultMap,
                ClientId = (sbyte) HcMap.DefaultMap.GetEntityId(),
                Location = HcMap.DefaultMap.GetSpawn(),
            };
            
            entity1.Spawn();
            entity2.Spawn();
            entity1.HandleMove();
            entity2.HandleMove();
            
        }
        
    }
}