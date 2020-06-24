using System;
using System.Collections.Generic;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    struct Vector2S {
        public bool Equals(Vector2S other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2S && Equals((Vector2S)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public short X { get; set; }
        public short Y { get; set; }

        public Vector2S(short x, short y) => (X, Y) = (x, y);

        public Vector2S(int x, int y)
        {
            X = (short)x;
            Y = (short)y;
        }
        public static bool operator ==(Vector2S item1, Vector2S item2) => (item1.X, item1.Y) == (item2.X, item2.Y);

        public static bool operator !=(Vector2S item1, Vector2S item2) => (item1.X, item1.Y) != (item2.X, item2.Y);

        public override string ToString() {
            return $"[Vector: {X}, {Y}]";
        }
    }
    /// <summary>
    /// Ported from https://github.com/umby24/D3classic/blob/master/Lua/Map_Fill/UmbyFlyingIslands.lua
    /// </summary>
    public class FlyingIslands : Mapfill {
        private Block _waterBlock;
        private const int ChunkSize = 16;
            
        public FlyingIslands() {
            Name = "flying";
            _waterBlock = BlockManager.GetBlock("still water");
        }
        
        public override void Execute(HcMap map, string[] args) {
            Vector3S mapSize = map.GetSize();
            int chunkX = mapSize.X / 16;
            int chunkY = mapSize.Y / 16;
            
            var randomGenerator = new Random();
            var firstSeed = randomGenerator.Next(-500, 500) / 2;
            var secondSeed = randomGenerator.Next(-500, 500) / 2;
            
            
            var data = new byte[mapSize.X*mapSize.Y*mapSize.Z];
            data = AddWater(mapSize, data, 2);

            for (var x = 0; x < chunkX; x++) {
                for (var y = 0; y < chunkY; y++) {
                    data = createSkyIslands(data, x, y, secondSeed, 1, mapSize.Z);
                }
            }
            
            map.SetMap(data);
            Chat.SendMapChat("Sky Islands done!", 0, map);
            map.Resend();
        }

        private byte[] AddWater(Vector3S mapSize, byte[] input, int waterLevel) {
            for (var ix = 0; ix < mapSize.X; ix++) {
                for (var iy = 0; iy < mapSize.Y; iy++) {
                    for (var iz = 0; iz < waterLevel; iz++) {
                        input[GetBlockCoords(ix, iy, iz)] = _waterBlock.Id;
                    }
                }
            }

            return input;
        }
        
        private float Random(float x, float y, float seed) {
            double result = x + (y * 1.2345f) + (seed * 5.6789);
            result += (result - x);
            result += (result + y);
            result += (result + x * 12.3);
            result += (result - y * 45.6);
            result += Math.Sin(x * 78.9012) + y + Math.Cos(seed * 78.9012);
            result += Math.Cos(y * 12.3456) - x + Math.Sin(seed * result + result + x);
            result += Math.Sin(y * 45.6789) + x + Math.Cos(seed * result + result - y);
            return (float)(result - Math.Floor(result));
        }

        private float[] Quantize(float x, float y, float factor) {
            return new[] {MathF.Floor(x / factor) * factor, MathF.Floor(y / factor)* factor};
        }

        private float[,] GenerateRandomMap(int chunkX, int chunkY, int chunks, int resultSize, float randomness,
            float seed) {

            int mapDivider = chunks; // -- in 1
            float[] mapPos = Quantize(chunkX, chunkY, mapDivider); // -- in chunks
            float mapPosXm = mapPos[0] * ChunkSize; // -- in meters
            float mapPosYm = mapPos[1] * ChunkSize;
            float offsetX = chunkX - mapPos[0];
            float offsetY = chunkY - mapPos[1];
            
            var result = new Dictionary<Vector2S, float>();
            
            int size = 1;
            
            for (var ix = 0; ix <= size; ix++) {
                for (var iy = 0; iy <= size; iy++) {
                    var x = mapPosXm + ix * ChunkSize * chunks;
                    var y = mapPosYm + iy * ChunkSize * chunks;
                    var randomVal = Random(x, y, seed);
                    
                    result.Add(new Vector2S(ix, iy), randomVal);
                }
            }
            
            // -- Iterations
            while (size < chunks * resultSize) {
                for (var ix = size; ix > 0; ix--) { // -- expand the array out a bit..
                    for (var iy = size; iy > 0; iy--) {
                        result.Add(new Vector2S(ix*2, iy*2), result[new Vector2S(ix, iy)]);
                    }
                }

                size *= 2;
                int sizeFactor = chunks * ChunkSize / size;
                
                // -- Diamond step
                for (var ix = 1; ix <= size; ix += 2) {
                    for (var iy = 1; iy <= size; iy += 2) {
                        var currentPoint = new Vector2S(ix, iy);
                        var firstPoint = result[new Vector2S(ix + 1, iy + 1)];
                        var secondPoint = result[new Vector2S(ix - 1, iy + 1)];
                        var thirdPoint = result[new Vector2S(ix + 1, iy - 1)];
                        var fourthPoint = result[new Vector2S(ix - 1, iy - 1)];

                        var tmpAvg = (firstPoint + secondPoint+ thirdPoint +
                                      fourthPoint) / 4;
                        
                        var firstMax = MathF.Max(Math.Abs(tmpAvg - firstPoint),
                            Math.Abs(tmpAvg - secondPoint));
                        var secondMax = MathF.Max(Math.Abs(tmpAvg - thirdPoint),
                            Math.Abs(tmpAvg - fourthPoint));
                        
                        var tempMax = MathF.Max(firstMax, secondMax);
                        var randomX = mapPosXm + ix * sizeFactor;
                        var randomY = mapPosYm + iy * sizeFactor;
                        result[currentPoint] =
                            tmpAvg + ((Random(randomX, randomY, seed) * 2 - 1) *
                                      tempMax * randomness);
                    }
                }
                
                // -- Square Step
                for (var ix = 0; ix <= size; ix += 2) {
                    for (var iy = 1; iy <= size; iy += 2) {
                        var currentPoint = new Vector2S(ix, iy);
                        var firstPoint = result[new Vector2S(ix, iy - 1)];
                        var secondPoint = result[new Vector2S(ix, iy + 1)];

                        float tmpAvg = (firstPoint + secondPoint) / 2;
                        float tmpMax = MathF.Max(Math.Abs(tmpAvg - firstPoint), Math.Abs(tmpAvg - secondPoint));
                        var randomX = mapPosXm + ix * sizeFactor;
                        var randomY = mapPosYm + iy * sizeFactor;
                        result[currentPoint] =
                            tmpAvg + ((Random(randomX, randomY, seed) * 2 - 1) * tmpMax * randomness);
                    }
                }

                for (var ix = 1; ix <= size; ix += 2) {
                    for (var iy = 0; iy <= size; iy += 2) {
                        var currentPoint = new Vector2S(ix, iy);
                        var firstPoint = result[new Vector2S(ix-1, iy)];
                        var secondPoint = result[new Vector2S(ix+1, iy)];

                        float tmpAvg = (firstPoint + secondPoint) / 2;
                        float tmpMax = MathF.Max(Math.Abs(tmpAvg - firstPoint), Math.Abs(tmpAvg - secondPoint));
                        var randomX = mapPosXm + ix * sizeFactor;
                        var randomY = mapPosYm + iy * sizeFactor;
                        result[currentPoint] =
                            tmpAvg + ((Random(randomX, randomY, seed) * 2 - 1) * tmpMax * randomness);
                    }
                }
            }
            
            // -- Final return build.
            float[,] final = new float[resultSize,resultSize];
            for (var ix = 0; ix < resultSize; ix++) {
                for (var iy = 0; iy < resultSize; iy++) {
                    var point = new Vector2S((short)(offsetX * resultSize + ix), (short)(offsetY*resultSize+iy));
                    final[ix, iy] = result[point];
                }
            }

            return final;
        }

        private Dictionary<Vector2S, float> HeightmapFractal(int chunkX, int chunkY, int Quant, int iterPerChunk, int randomness, int seed) {
            var heightMap = GenerateRandomMap(chunkX, chunkY, Quant, iterPerChunk, randomness, seed);
            int size = iterPerChunk;
            
            var map = new Dictionary<Vector2S, float>();
            for (var ix = 0; ix < iterPerChunk; ix++) {
                for (var iy = 0; iy < iterPerChunk; iy++) {
                    map.Add(new Vector2S(ix, iy), heightMap[ix, iy]);
                }
            }
            
            while (size < ChunkSize) {
                // -- Array Resizing..
                for (var ix = size; ix > 0; ix--) {
                    for (var iy = size; iy > 0; iy--) {
                        var point1 = new Vector2S(ix*2, iy*2);
                        map.Add(point1, heightMap[ix, iy]);
                    }
                }

                size *= 2;
                // -- Diamond step
                for (var ix = 1; ix <= size; ix += 2) {
                    for (var iy = 1; iy <= size; iy += 2) {
                        var point1 = map[new Vector2S(ix + 1, iy + 1)];
                        var point2 = map[new Vector2S(ix - 1, iy + 1)];
                        var point3 = map[new Vector2S(ix + 1, iy - 1)];
                        var point4 = map[new Vector2S(ix - 1, iy - 1)];

                        map[new Vector2S(ix, iy)] = (point1 + point2 + point3 + point4) / 4;
                    }
                }
                // -- square step
                for (var ix = 0; ix <= size; ix += 2) {
                    for (var iy = 1; iy <= size; iy += 2) {
                        var currentPoint = new Vector2S(ix, iy);
                        var firstPoint = map[new Vector2S(ix, iy - 1)];
                        var secondPoint = map[new Vector2S(ix, iy + 1)];
                        map[currentPoint] = (firstPoint + secondPoint) / 2;
                    }
                }

                for (var ix = 1; ix <= size; ix += 2) {
                    for (var iy = 0; iy <= size; iy += 2) {
                        var currentPoint = new Vector2S(ix, iy);
                        var firstPoint = map[new Vector2S(ix-1, iy)];
                        var secondPoint = map[new Vector2S(ix+1, iy)];
                        map[currentPoint] = (firstPoint + secondPoint) / 2;
                    }
                }
            }

            return map;
        }
        
        private byte[] createSkyIslands(byte[] data, int chunkX, int chunkY, int seed, int generationState, int mapZ) {
            int offsetX = chunkX * ChunkSize;
            int offsetY = chunkY * ChunkSize;
            
            var HeightMap0 = HeightmapFractal(chunkX, chunkY, 1, 1, 1, seed);
            var heightmap1 = HeightmapFractal(chunkX, chunkY, 1, 1, 0, seed);
            var totalHeight = HeightmapFractal(chunkX, chunkY, 4, 1, 0, seed);
            var treeTypeMap = HeightmapFractal(chunkX, chunkY, 8, 1, 0, seed + 3);

            if (generationState == 1) {
                // -- Build the chunk
                for (var ix = 0; ix < ChunkSize; ix++) {
                    for (var iy = 0; iy < ChunkSize; iy++) {
                        var currentPoint = new Vector2S(ix, iy);
                        var height = 20 + totalHeight[currentPoint] * mapZ;
                        var height0 = Math.Floor(height + HeightMap0[currentPoint] * 50);
                        var height1 = Math.Floor((mapZ / 2) + heightmap1[currentPoint] * 15);

                        for (int iz = (int)height0; iz <= height1; iz++) {
                            byte blockType = 3;
                            if (iz == height1) {
                                blockType = 2;
                            }
                            
                            data[GetBlockCoords(offsetX + ix, offsetY + iy, (int)iz)] = blockType;
                        }
                    }
                }

                return data;
            }
            
            // -- Build the trees.
            return data;
        }
        
    }
}