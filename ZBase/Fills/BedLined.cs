using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    public class BedLined : Mapfill {
        public BedLined() {
            Name = "Bedlined";
        }
        
        public override void Execute(HcMap map, string[] args) {
            var sw = new Stopwatch();
            sw.Start();
            
            byte bedrockBlock = BlockManager.GetBlock("solid").Id;
            
            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;
            
            var data = new byte[mapSize.X * mapSize.Y * mapSize.Z];

            for (short ix = 0; ix < mapSize.X; ix++) {
                for (short iy = 0; iy < mapSize.Y; iy++)
                    data[GetBlockCoords(ix, iy, 0)] = bedrockBlock;
            }

            for (short ix = 0; ix < mapSize.X; ix++) {
                for (short iz = 0; iz < mapSize.Z / 2; iz++) {
                    data[GetBlockCoords(ix, 0, iz)] = bedrockBlock;
                    data[GetBlockCoords(ix, mapSize.Y - 1, iz)] = bedrockBlock;
                }
            }

            for (short iy = 0; iy < mapSize.Y; iy++) {
                for (short iz = 0; iz < mapSize.Z / 2; iz++) {
                    data[GetBlockCoords(0, iy, iz)] = bedrockBlock;
                    data[GetBlockCoords(mapSize.X - 1, iy, iz)] = bedrockBlock;
                }
            }

            map.SetMap(data);
            sw.Stop();
            Chat.SendMapChat($"&cMap created in {sw.Elapsed.TotalSeconds}s.", 0, map);
            map.Resend();
        }
    }
}
