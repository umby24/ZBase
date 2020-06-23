using System;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Building.BuildModes {
    public struct SphereOptions {
        public bool Hollow { get; set; }
        public Vector3S Coord { get; set; }
        public Client ExecutingClient { get; set; }
        public Block Material { get; set; }
        public Block ReplaceMaterial { get; set; }
        public double Radius { get; set; }
    }
    
    public class Sphere : BuildMode {
        public Sphere() {
            Name = Constants.SphereBuildModeName;
        }

        public override void Invoke(Vector3S location, byte mode, Block block) {
            if (mode == 0)
                return;

            var currentState = PlayerState.GetInt(0);
            if (currentState == 0) {
                var newLocation = new MinecraftLocation();
                newLocation.SetAsBlockCoords(location);
                PlayerState.SetCoord(newLocation, 0);
                PlayerState.Set(1, 0);
                return;
            }

            Vector3S firstLocation = PlayerState.GetCoord(0).GetAsBlockCoords();
            var radius = Math.Sqrt(Math.Pow(location.X - firstLocation.X, 2) +
                                   Math.Pow(location.Y - firstLocation.Y, 2) +
                                   Math.Pow(location.Z - firstLocation.Z, 2));
            var replaceMaterial = BlockManager.GetBlock(PlayerState.GetString(0));
            var isHollow = PlayerState.GetInt(1) == 1;

            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = null;
            ExecutingClient.ClientPlayer.CurrentState.ResendBlocks(ExecutingClient);
            
            if (radius < 50) {
                ExecutingClient.ClientPlayer.Entity.CurrentMap.MapActions.ActionQueue.Enqueue(() => {
                    var opts = new SphereOptions {
                        Coord = location, 
                        Radius = radius,
                        ReplaceMaterial = replaceMaterial,
                        ExecutingClient = ExecutingClient,
                        Hollow = isHollow,
                        Material = block
                    };
                    
                    BuildSphere(opts);
                }); 
                SendExecutorMessage("§SSphere queued.");
            }
            else {
                SendExecutorMessage("§ESphere is too large.");
            }
        }


        private void BuildSphere(SphereOptions options) {
            var rounded = ((int)Math.Round(options.Radius, 1) + 1);
            var power = (float)Math.Pow(options.Radius, 2);

            for (var ix = -rounded; ix < rounded; ix++) {
                for (var iy = -rounded; iy < rounded; iy++) {
                    for (var iz = -rounded; iz < rounded; iz++) {
                        var squareDistance = (int)(Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2));

                        if (!(squareDistance <= power)) 
                            continue;

                        var allowed = false;

                        if (options.Hollow) {
                            if (Math.Pow(ix + 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > power)
                                allowed = true;

                            if (Math.Pow(ix - 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > power)
                                allowed = true;

                            if (Math.Pow(ix, 2) + Math.Pow(iy + 1, 2) + Math.Pow(iz, 2) > power)
                                allowed = true;

                            if (Math.Pow(ix, 2) + Math.Pow(iy - 1, 2) + Math.Pow(iz, 2) > power)
                                allowed = true;

                            if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz + 1, 2) > power)
                                allowed = true;

                            if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz - 1, 2) > power)
                                allowed = true;
                        } else {
                            allowed = true;
                        }

                        if (!allowed) 
                            continue;
                        var blockLocation = new Vector3S((short)(options.Coord.X + ix), (short)(options.Coord.Y + iy), (short)(options.Coord.Z + iz));
                        var currentBlock =
                            options.ExecutingClient.ClientPlayer.Entity.CurrentMap.GetBlockId(blockLocation);
                        
                        if (options.ReplaceMaterial == null || options.ReplaceMaterial.Id == currentBlock)
                            options.ExecutingClient.ClientPlayer.HandleBlockPlace(blockLocation, options.Material.Id, 1);
                    }
                }
            }
        }
    }
}