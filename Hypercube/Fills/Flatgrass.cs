using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    public class Flatgrass : Mapfill {
        private byte _dirtBlock;
        private byte _grassBlock;
        
        public Flatgrass() {
            Name = "Flatgrass";
            _dirtBlock = BlockManager.GetBlock("dirt").Id;
            _grassBlock = BlockManager.GetBlock("grass").Id;
        }

        public override void Execute(HcMap map, string[] args) {
            var sw = new Stopwatch();
            sw.Start();

            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;
            var data = new byte[mapSize.X*mapSize.Y*mapSize.Z];

            for (short x = 0; x < mapSize.X; x++) {
                for (short y = 0; y < mapSize.Y; y++) {
                    for (short z = 0; z < (mapSize.Z/2); z++) {
                        data[GetBlockCoords(x, y, z)] = z == (mapSize.Z/2) - 1 ? _grassBlock : _dirtBlock;
                    }
                }
            }
            
            map.SetMap(data);

            sw.Stop();
            Chat.SendMapChat($"&cMap created in {sw.Elapsed.TotalSeconds}s.", 0, map);
            map.Resend();
        }
    }
}
