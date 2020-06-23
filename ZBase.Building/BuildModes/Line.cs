using System;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Building.BuildModes {
    public struct LineOptions {
        public Vector3S StartLocation { get; set; }
        public Vector3S EndLocation { get; set; }
        public Client ExecutingClient { get; set; }
        public Block Material { get; set; }
        
    }
    
    public class Line : BuildMode {
        public Line() {
            Name = Constants.LineBuildModeName;
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
            
            var firstCoord = PlayerState.GetCoord(0);
            
            PlayerState.CurrentMode = null;
            PlayerState.ResendBlocks(ExecutingClient);
            
            ExecutingClient.ClientPlayer.Entity.CurrentMap.MapActions.ActionQueue.Enqueue(() => {
                var lineOptions = new LineOptions {
                    StartLocation = firstCoord.GetAsBlockCoords(),
                    EndLocation = location,
                    ExecutingClient = ExecutingClient,
                    Material = block
                };
                
                BuildLine(lineOptions);
            });
            
            SendExecutorMessage("§SLine Queued.");
        }

        private void BuildLine(LineOptions options) {
            var dx = options.EndLocation.X - options.StartLocation.X;
            var dy = options.EndLocation.Y - options.StartLocation.Y;
            var dz = options.EndLocation.Z - options.StartLocation.Z;

            var blocks = 1;

            if (blocks < Math.Abs(dx))
                blocks = Math.Abs(dx);

            if (blocks < Math.Abs(dy))
                blocks = Math.Abs(dy);

            if (blocks < Math.Abs(dz))
                blocks = Math.Abs(dz);

            var mx = dx / (float) blocks;
            var my = dy / (float)blocks;
            var mz = dz / (float)blocks;

            for (var i = 0; i < blocks + 1; i++) {
                var blockLocation = new Vector3S((short)(options.StartLocation.X + mx * i),
                    (short)(options.StartLocation.Y + my * i),
                    (short)(options.StartLocation.Z + mz * i));

                options.ExecutingClient.ClientPlayer.HandleBlockPlace(blockLocation, options.Material.Id, 1);
            }
            
            Chat.SendClientChat("§SLine completed.", 0, options.ExecutingClient);
        }
    }
}