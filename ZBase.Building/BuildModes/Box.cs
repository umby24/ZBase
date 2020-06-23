using System;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Building.BuildModes {
    public struct BoxOptions {
        public bool Hollow { get; set; }
        public string ReplaceBlock { get; set; }
        public Vector3S StartCoord { get; set; }
        public Vector3S EndCoord { get; set; }
        public Client ExecutingClient { get; set; }
        public Block Material { get; set; }
    }
    public class Box : BuildMode {
        public Box() {
            Name = Constants.BoxBuildModeName;
        }
        
        public override void Invoke(Vector3S location, byte mode, Block block) {
            if (mode == 0)
                return;

            var state = PlayerState.GetInt(0);
            if (state == 0) {
                var newLocation = new MinecraftLocation();
                newLocation.SetAsBlockCoords(location);
                
                PlayerState.SetCoord(newLocation, 1);
                PlayerState.Set(1, 0);
                return;
            }
            
            // -- State is 1
            Vector3S firstCoord = PlayerState.GetCoord(1).GetAsBlockCoords();
            var endCoord = location;

            var numBlocks = Math.Abs(firstCoord.X - endCoord.X) * Math.Abs(firstCoord.Y - endCoord.Y) *
                            Math.Abs(firstCoord.Z - endCoord.Z);
            var replaceBlock = PlayerState.GetString(0);
            var isHollow = PlayerState.GetInt(1) == 1;
            
            PlayerState.CurrentMode = null;
            PlayerState.ResendBlocks(ExecutingClient);
            
            if (numBlocks < 50000) {
                ExecutingClient.ClientPlayer.Entity.CurrentMap.MapActions.ActionQueue.Enqueue(() => {
                    var opts = new BoxOptions {
                        Hollow = isHollow,
                        EndCoord = endCoord,
                        StartCoord = firstCoord,
                        ReplaceBlock = replaceBlock,
                        Material = block,
                        ExecutingClient = ExecutingClient
                    };
                    
                    BuildBox(opts);
                });
                SendExecutorMessage("§SBox queued.");
            }
            else {
                SendExecutorMessage("§EBox too large.");
            }


        }

        private static void BuildBox(BoxOptions options) {
            var startX = Math.Min(options.StartCoord.X, options.EndCoord.X);
            var startY = Math.Min(options.StartCoord.Y, options.EndCoord.Y);
            var startZ = Math.Min(options.StartCoord.Z, options.EndCoord.Z);
            var endX = Math.Max(options.StartCoord.X, options.EndCoord.X);
            var endY = Math.Max(options.StartCoord.Y, options.EndCoord.Y);
            var endZ = Math.Max(options.StartCoord.Z, options.EndCoord.Z);
            
            Block replaceMaterial = BlockManager.GetBlock(options.ReplaceBlock);
            
            for (var ix = startX; ix < endX + 1; ix++) {
                for (var iy = startY; iy < endY + 1; iy++) {
                    for (var iz = startZ; iz < endZ + 1; iz++) {
                        var blockLocation = new Vector3S(ix, iy, iz);
                        if (options.ReplaceBlock != null) {
                            var current =
                                options.ExecutingClient.ClientPlayer.Entity.CurrentMap.GetBlockId(blockLocation);
                            if (current != replaceMaterial.Id)
                                continue;
                            
                        }
                        if (!options.Hollow || (ix == startX || ix == endX || iy == startY || iy == endY || iz == startZ || iz == endZ)) 
                            options.ExecutingClient.ClientPlayer.HandleBlockPlace(blockLocation, options.Material.Id, 1);
                    }
                }
            }
            Chat.SendClientChat("§SBox completed.", 0, options.ExecutingClient);
        }
    }
}