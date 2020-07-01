using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    public class Candy : Mapfill {
        private int _mapScale = 6;
        private int _area = 4;
        private int _priority = 0;

        public Candy() {
            Name = "Candy";
        }


        public override void Execute(HcMap map, string[] args) {
            Stopwatch sw = Stopwatch.StartNew();

            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;

            var data = new byte[mapSize.X*mapSize.Y*mapSize.Z];

            double hmapSizeX = Math.Ceiling(mapSize.X / (double)_mapScale);
            double hmapSizeY = Math.Ceiling(mapSize.Y / (double)_mapScale);
            const int fields = 1;
            var mBuildings = new Dictionary<double, int>();
            var rng = new Random();

            for (var x = 0; x <= hmapSizeX; x++) {
                for (var y = 0; y <= hmapSizeY; y++) {
                    double number = (x + y * hmapSizeX) * fields;
                    mBuildings[number] = rng.Next(0, mapSize.Z);
                }
            }

            for (var x = 0; x <= hmapSizeX; x++) {

                for (var y = 0; y <= hmapSizeY; y++)
                {
                    double number = (x + y * hmapSizeX) * fields;
                    int oheight = mBuildings[number];
                    int height = oheight * _priority;

                    var count = 0;
                    for (int ax = -_area; ax <= _area; ax++) {
                        for (int ay = -_area; ay <= _area; ay++) {
                            if (x + ax < 0 || x + ax >= hmapSizeX || y + ay < 0 || y + ay >= hmapSizeY)
                                continue;

                            double num = x + ax + (y + ay) * hmapSizeX * fields;
                            int bheight = mBuildings[num];
                            height = height + bheight;
                            count++;
                        }
                    }

                    height = (int) Math.Floor(((double)height / (count + _priority)));

                    int bx = x * _mapScale;
                    int by = y * _mapScale;
                    byte mat = BlockManager.GetBlock("Red Cloth").Id; //21; // -- material

                    for (var iz = 0; iz <= height; iz++) {
                        if (mat > 33)
                            mat = BlockManager.GetBlock("Red Cloth").Id;

                        for (var ix = 0; ix < _mapScale; ix++) {
                            for (var iy = 0; iy < _mapScale; iy++) {

                                //if (bx + ix > mapSize.X || by+iy > mapSize.Y)
                                //    continue;

                                int xCoord = bx + ix;
                                int yCoord = by + iy;

                                data[GetBlockCoords(xCoord, yCoord, iz)] = mat;
                            }
                        }

                        mat++;
                    }
                }
            }

            map.SetMap(data);

            sw.Stop();
            Chat.SendMapChat("&2Contructed \"Candy\" in &a" + sw.Elapsed.TotalSeconds + "&2s.", 0, map);
            map.Resend();
        }
    }
}