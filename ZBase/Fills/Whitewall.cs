using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    public class Whitewall : Mapfill {
        private byte _whiteCloth;
        public Whitewall() {
            Name = "Whitewall";
            _whiteCloth = BlockManager.GetBlock("White Cloth").Id;

        }
        public override void Execute(HcMap map, string[] args) {
            var sw = new Stopwatch();
            sw.Start();

            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;
            var data = new byte[mapSize.X * mapSize.Y * mapSize.Z];

            for (short ix = 0; ix < mapSize.X; ix++) {
                for (short iy = 0; iy < mapSize.Y; iy++)
                    data[GetBlockCoords(ix, iy, 0)] = _whiteCloth;
                //map.BlockChange(-1, (short) ix, (short) iy, 0, whiteBlock, airBlock, false, false, false, 1);
            }

            for (short ix = 0; ix < mapSize.X; ix++) {
                for (short iz = 0; iz < mapSize.Z / 2; iz++) {
                    data[GetBlockCoords(ix, 0, iz)] = _whiteCloth;
                    data[GetBlockCoords(ix, mapSize.Y - 1, iz)] = _whiteCloth;
              //      map.BlockChange(-1, (short) ix, 0, (short) iz, whiteBlock, airBlock, false, false, false, 1);
              //      map.BlockChange(-1, (short) ix, (short) (map.CWMap.SizeY - 1), (short) iz, whiteBlock, airBlock,
              //          false, false, false, 1);
                }
            }

            for (short iy = 0; iy < mapSize.Y; iy++) {
                for (short iz = 0; iz < mapSize.Z/ 2; iz++) {
                    data[GetBlockCoords(0, iy, iz)] = _whiteCloth;
                    data[GetBlockCoords(mapSize.X - 1, iy, iz)] = _whiteCloth;
                    //     map.BlockChange(-1, 0, (short) iy, (short) iz, whiteBlock, airBlock, false, false, false, 1);
                    //     map.BlockChange(-1, (short) (map.CWMap.SizeX - 1), (short) iy, (short) iz, whiteBlock, airBlock,
                    //         false, false, false, 1);
                }
            }

            map.SetMap(data);
            sw.Stop();
            Chat.SendMapChat($"&cMap created in {sw.Elapsed.TotalSeconds}s.", 0, map);
            map.Resend();
        }
    }
}