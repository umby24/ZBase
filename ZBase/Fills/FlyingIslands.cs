using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.XPath;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Fills {
    public struct Vector2S {
        public bool Equals(Vector2S other) {
            return this == other;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2S && Equals((Vector2S)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public short X { get; set; }
        public short Y { get; set; }

        public Vector2S(short x, short y) => (X, Y) = (x, y);

        public Vector2S(int x, int y) {
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
        private byte _waterBlock;
        private byte _dirtBlock;
        private byte _grassBlock;

        private const int ChunkSize = 16;
        public FlyingIslands() {
            Name = "flying";
            _waterBlock = BlockManager.GetBlock("still water").Id;
            _dirtBlock = BlockManager.GetBlock("dirt").Id;
            _grassBlock = BlockManager.GetBlock("grass").Id;
        }

        public override void Execute(HcMap map, string[] args) {
            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;
            int chunkX = mapSize.X / 16;
            int chunkY = mapSize.Y / 16;

            var randomGenerator = new Random();
            float firstSeed = randomGenerator.Next(-500, 500) / 2f;
            float secondSeed = randomGenerator.Next(-500, 500) / 2f;
            float newSeed = randomGenerator.Next(-500, 500) / 2f;

            Chat.SendMapChat($"&2Generation Seed: &a{firstSeed}", 0, map);
            Chat.SendMapChat($"&2Generation Seed: &a{secondSeed}", 0, map);

            var data = new byte[mapSize.X * mapSize.Y * mapSize.Z];
            data = GenerateWater(mapSize, data, 2);

            for (var x = 0; x < chunkX; x++) {
                for (var y = 0; y < chunkY; y++) {
                    data = CreateSkyIslands(data, x, y, secondSeed, 1, mapSize.Z);
                    data = CreateSkyIslands(data, x, y, secondSeed, 2, mapSize.Z);

                    if (mapSize.Z > 64) data = CreateSkyIslands(data, x, y, firstSeed, 1, mapSize.Z, 3, true);

                    if (mapSize.Z > 256) data = CreateSkyIslands(data, x, y, newSeed, 1, mapSize.Z, 10, true);
                }
            }

            map.SetMap(data);
            Chat.SendMapChat("&aSky Islands done!", 0, map);
            map.Resend();
        }

        /// <summary>
        /// Adds a water layer to the bottom of the map
        /// </summary>
        /// <param name="mapSize">Size of the map as a vector</param>
        /// <param name="input">Map data to populate with water</param>
        /// <param name="waterLevel">How high the water should be from the bottom</param>
        /// <returns>Map data array filled with water.</returns>
        private byte[] GenerateWater(Vector3S mapSize, byte[] input, int waterLevel) {
            for (var ix = 0; ix < mapSize.X; ix++) {
                for (var iy = 0; iy < mapSize.Y; iy++) {
                    for (var iz = 0; iz < waterLevel; iz++) {
                        input[GetBlockCoords(ix, iy, iz)] = _waterBlock;
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Another Psuedo-Random number generator, based on x,y coordinate of a map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static double Random(float x, float y, float seed) {
            double result = x + y * 1.2345d + seed * 5.6789d;
            result += result - x;
            result += result + y;
            result += result + x * 12.3d;
            result += result - y * 45.6d;

            result += Math.Sin(x * 78.9012d) + y + Math.Cos(seed * 78.9012d);
            result += Math.Cos(y * 12.3456d) - x + Math.Sin(seed * result + result + x);
            result += Math.Sin(y * 45.6789d) + x + Math.Cos(seed * result + result - y);
            return result - Math.Floor(result);
        }

        public static int[] Quantize(float x, float y, float factor) {
            return new[] { (int)(MathF.Floor(x / factor) * factor), (int)(MathF.Floor(y / factor) * factor) };
        }

        public static Dictionary<Vector2S, double> GenerateRandomMap(int mapPosXm, int mapPosYm, int chunks, float seed) {
            var result = new Dictionary<Vector2S, double>();
            List<Vector2S> tdpa = Generate2dPointArray(0, 1, true);
            foreach (var pt in tdpa) {
                int x = mapPosXm + pt.X * ChunkSize * chunks;
                int y = mapPosYm + pt.Y * ChunkSize * chunks;
                double randomVal = Random(x, y, seed);
                result.Add(pt, randomVal);
            }

            return result;
        }

        private Dictionary<Vector2S, double> GenerateRandomMap(int chunkX, int chunkY, int chunks, int resultSize, float randomness,
            float seed) {

            int mapDivider = chunks; // -- in 1
            int[] mapPos = Quantize(chunkX, chunkY, mapDivider); // -- in chunks
            int mapPosXm = mapPos[0] * ChunkSize; // -- in meters
            int mapPosYm = mapPos[1] * ChunkSize;
            int offsetX = chunkX - mapPos[0];
            int offsetY = chunkY - mapPos[1];

            var size = 1;

            Dictionary<Vector2S, double> result = GenerateRandomMap(mapPosXm, mapPosYm, chunks, seed);
            // -- Iterations
            while (size < chunks * resultSize) { // -- this is basically.. for(var size = size; size < chunks*resultsize; size*=2) {}

                // -- Expand the array out.
                result = DoubleHeightMap(result, size);

                size *= 2;
                int sizeFactor = chunks * ChunkSize / size;
                result = DiamondRandomMap(result, size, sizeFactor, mapPosXm, mapPosYm, seed, randomness);
                result = SquareRandomMap(result, size, sizeFactor, mapPosXm, mapPosYm, seed, randomness);
            }

            // -- Final return build.
            var final = new Dictionary<Vector2S, double>();
            List<Vector2S> tdpa = Generate2dPointArray(0, resultSize, true);

            foreach (Vector2S pt in tdpa) {
                var valuePoint = new Vector2S(offsetX * resultSize + pt.X, offsetY * resultSize + pt.Y);
                final.Add(pt, result[valuePoint]);
            }

            return final;
        }

        public static List<Vector2S> Generate2dPointArray(int start, int end, bool inclusive) {
            var result = new List<Vector2S>();

            for (int ix = start; inclusive ? ix <= end : ix < end; ix++) {
                for (int iy = start; inclusive ? iy <= end : iy < end; iy++) {
                    result.Add(new Vector2S(ix, iy));
                }
            }

            return result;
        }

        public static Dictionary<Vector2S, double> DiamondRandomMap(Dictionary<Vector2S, double> map, int size, int sizeFactor, int mapPosXm, int mapPosYm, float seed, float randomness) {
            for (var ix = 1; ix <= size; ix += 2) {
                for (var iy = 1; iy <= size; iy += 2) {
                    var currentPoint = new Vector2S(ix, iy);
                    var firstPoint = map[new Vector2S(ix + 1, iy + 1)];
                    var secondPoint = map[new Vector2S(ix - 1, iy + 1)];
                    var thirdPoint = map[new Vector2S(ix + 1, iy - 1)];
                    var fourthPoint = map[new Vector2S(ix - 1, iy - 1)];

                    var tmpAvg = (firstPoint + secondPoint + thirdPoint +
                                  fourthPoint) / 4;

                    var firstMax = Math.Max(Math.Abs(tmpAvg - firstPoint),
                        Math.Abs(tmpAvg - secondPoint));
                    var secondMax = Math.Max(Math.Abs(tmpAvg - thirdPoint),
                        Math.Abs(tmpAvg - fourthPoint));

                    var tempMax = Math.Max(firstMax, secondMax);
                    var randomX = mapPosXm + ix * sizeFactor;
                    var randomY = mapPosYm + iy * sizeFactor;
                    var randomVal =Random(randomX, randomY, seed);
                    var timzed = randomVal * 2 - 1;
                    var finalVal = tmpAvg + timzed * tempMax * randomness;

                    map[currentPoint] = finalVal;
                }
            }

            return map;
        }

        public static Dictionary<Vector2S, double> SquareRandomMap(Dictionary<Vector2S, double> map, int size,
            int sizeFactor, int mapPosXm, int mapPosYm, float seed, float randomness) {
            

            for (var ix = 0; ix <= size; ix += 2) {
                for (var iy = 1; iy <= size; iy += 2) {
                    var currentPoint = new Vector2S(ix, iy);
                    var firstPoint = map[new Vector2S(ix, iy - 1)];
                    var secondPoint = map[new Vector2S(ix, iy + 1)];

                    double tmpAvg = (firstPoint + secondPoint) / 2;
                    double tmpMax = Math.Max(Math.Abs(tmpAvg - firstPoint), Math.Abs(tmpAvg - secondPoint));
                    var randomX = mapPosXm + ix * sizeFactor;
                    var randomY = mapPosYm + iy * sizeFactor;

                    map[currentPoint] =
                        tmpAvg + ((Random(randomX, randomY, seed) * 2 - 1) * tmpMax * randomness);
                }
            }

            for (var ix = 1; ix <= size; ix += 2) {
                for (var iy = 0; iy <= size; iy += 2) {
                    var currentPoint = new Vector2S(ix, iy);
                    var firstPoint = map[new Vector2S(ix - 1, iy)];
                    var secondPoint = map[new Vector2S(ix + 1, iy)];

                    double tmpAvg = (firstPoint + secondPoint) / 2;
                    double tmpMax = Math.Max(Math.Abs(tmpAvg - firstPoint), Math.Abs(tmpAvg - secondPoint));
                    var randomX = mapPosXm + ix * sizeFactor;
                    var randomY = mapPosYm + iy * sizeFactor;
                    map[currentPoint] =
                        tmpAvg + (((Random(randomX, randomY, seed) * 2) - 1) * tmpMax * randomness);
                }
            }

            return map;
        }

        /// <summary>
        /// Sets the value of each block to the average of all 4 adjacent blocks
        /// </summary>
        /// <param name="map"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Dictionary<Vector2S, double> DiamondHeightMap(Dictionary<Vector2S, double> map, int size) {
            // xyx
            // ycy
            // xyx
            // -- c = current, y are the blocks we get the average of. Notice its a "diamond" shape, thus diamond average step.
            for (var ix = 1; ix <= size; ix += 2) {
                for (var iy = 1; iy <= size; iy += 2) {
                    double point1 = map[new Vector2S(ix + 1, iy + 1)];
                    double point2 = map[new Vector2S(ix - 1, iy + 1)];
                    double point3 = map[new Vector2S(ix + 1, iy - 1)];
                    double point4 = map[new Vector2S(ix - 1, iy - 1)];
                    var cp = new Vector2S(ix, iy);
                    double value = (point1 + point2 + point3 + point4) / 4;
                    if (map.ContainsKey(cp))
                        map[cp] = value;
                    else
                        map.Add(cp, value);
                }
            }

            return map;
        }

        public static Dictionary<Vector2S, double> SquareHeightMap(Dictionary<Vector2S, double> map, int size) {
            for (var ix = 0; ix <= size; ix += 2) {
                for (var iy = 1; iy <= size; iy += 2) {
                    var currentPoint = new Vector2S(ix, iy);
                    double firstPoint = map[new Vector2S(ix, iy - 1)];
                    double secondPoint = map[new Vector2S(ix, iy + 1)];
                    double value = (firstPoint + secondPoint) / 2;

                    if (map.ContainsKey(currentPoint))
                        map[currentPoint] = value;
                    else
                        map.Add(currentPoint, value);
                }
            }

            for (var ix = 1; ix <= size; ix += 2) {
                for (var iy = 0; iy <= size; iy += 2) {
                    var currentPoint = new Vector2S(ix, iy);
                    double firstPoint = map[new Vector2S(ix - 1, iy)];
                    double secondPoint = map[new Vector2S(ix + 1, iy)];
                    double value = (firstPoint + secondPoint) / 2;

                    if (map.ContainsKey(currentPoint))
                        map[currentPoint] = value;
                    else
                        map.Add(currentPoint, value);
                }
            }

            return map;
        }

        /// <summary>
        /// Expands a point array by a factor of two. Populates values from the non-doubled, pre-existing point.
        /// </summary>
        /// <param name="map">Array to resize</param>
        /// <param name="size">The size to resize this array to, halved.</param>
        /// <returns></returns>
        public static Dictionary<Vector2S, double> DoubleHeightMap(Dictionary<Vector2S, double> map, int size) {
            List<Vector2S> pointArray = Generate2dPointArray(0, size, true);
            pointArray.Reverse();

            foreach (Vector2S pt in pointArray) {
                var doublePoint = new Vector2S(pt.X * 2, pt.Y * 2);
                var newValue = 0d;
                
                if (map.ContainsKey(pt))
                    newValue = map[pt];

                if (!map.ContainsKey(doublePoint))
                    map.Add(doublePoint, newValue);
                else
                    map[doublePoint] = newValue;
            }

            return map;
        }

        private Dictionary<Vector2S, double> HeightmapFractal(int chunkX, int chunkY, int Quant, int iterPerChunk, int randomness, float seed) {
            Dictionary<Vector2S, double> heightMap = GenerateRandomMap(chunkX, chunkY, Quant, iterPerChunk, randomness, seed);

            int size = iterPerChunk;

            while (size < ChunkSize) { 
                heightMap = DoubleHeightMap(heightMap, size); // -- Expand point array
                size *= 2;
                heightMap = DiamondHeightMap(heightMap, size); // -- Perform an average on all adjacent blocks
                heightMap = SquareHeightMap(heightMap, size); // -- Perform a differential average on the adjacent blocks.
            }

            return heightMap;
        }

        private byte[] CreateSkyIslands(byte[] data, int chunkX, int chunkY, float seed, int generationState, int mapZ, int divisionFactor = 1, bool secondLayer = false) {
            int offsetX = chunkX * ChunkSize;
            int offsetY = chunkY * ChunkSize;

            var firstRandom = 1;
            var secondRandom = 0;
            var thirdRandom = 0;

            if (secondLayer) {
                firstRandom = 2;
                secondRandom = 2;
                thirdRandom = 4;
            }

            Dictionary<Vector2S, double> heightMap0 = HeightmapFractal(chunkX, chunkY, 1, 1, firstRandom, seed);
            Dictionary<Vector2S, double> heightMap1 = HeightmapFractal(chunkX, chunkY, 1, 1, secondRandom, seed);
            Dictionary<Vector2S, double> totalHeight = HeightmapFractal(chunkX, chunkY, 4, 1, thirdRandom, seed);
            Dictionary<Vector2S, double> treeTypeMap = HeightmapFractal(chunkX, chunkY, 8, 1, 0, seed + 3);

            List<Vector2S> tdpa = Generate2dPointArray(0, ChunkSize, false);
            var randomGenerator = new Random();

            foreach (Vector2S cp in tdpa) {
                double height = 20 + totalHeight[cp] * mapZ;
                double height0 = Math.Floor(height + heightMap0[cp] * 50);
                double height1 = Math.Floor((mapZ / 2d) + heightMap1[cp] * 15);

                double treeMapValue = treeTypeMap[cp];
                string treeType = GetTreeType(treeMapValue);

                for (var iz = (int)height0; iz <= height1; iz++) {
                    if (iz < 0)
                        continue;

                    bool isHighestValue = iz == height1;
                    int ix = offsetX + cp.X;
                    int iy = offsetY + cp.Y;

                    if (generationState == 1) { // -- Map generation stage
                        byte blockType = 3;
                        if (isHighestValue) {
                            blockType = 2;
                        }

                        data[GetBlockCoords(ix, iy, iz / divisionFactor)] = blockType;
                        continue;
                    }
                    // -- Tree generation stage.
                    if (isHighestValue && randomGenerator.Next(40) == 1) {
                        data = CreateTree(data, ix, iy, iz + 1, 1 + randomGenerator.NextDouble(),
                            treeType, mapZ);
                    }
                }
            }

            return data;
        }

        private string GetTreeType(double value) {
            string result;

            if (value <= 0.2)
                result = "";
            else if (value <= 0.4)
                result = "pine";
            else if (value <= 0.6)
                result = "birch";
            else if (value <= 0.8)
                result = "oak";
            else
                result = "";

            return result;
        }


        private byte[] GenerateOak(byte[] data, Vector3S location, double size) {
            int blockSize = (int)Math.Floor(size * 5);
            if (blockSize > 7) blockSize = 7;
            if (blockSize < 6) blockSize = 6;

            for (var iz = 0; iz < blockSize - 1; iz++) {
                data[GetBlockCoords(location.X, location.Y, location.Z + iz)] = 17;
            }

            var radius = 0.5f;

            for (int iz = blockSize; iz >= (blockSize - 4); iz--) {
                var intRadius = (int) Math.Ceiling(radius);
                List<Vector2S> tdpa = Generate2dPointArray(-intRadius, intRadius, true);

                foreach (Vector2S pt in tdpa) {
                    double dist = Math.Sqrt(Math.Pow(pt.X, 2) + Math.Pow(pt.Y, 2));
                    if (!(dist <= radius)) continue;
                    int currentIndex = GetBlockCoords(location.X + pt.X, location.Y + pt.Y, location.Z + iz);
                    byte currentType = data[currentIndex];
                    if (currentType != 0) continue;
                    data[currentIndex] = 18;
                }

                if (radius < 2)
                    radius += 0.7f;
            }

            return data;
        }

        private byte[] GeneratePine(byte[] data, Vector3S location, double size, int mapZ) {
            var blockSize = (int)Math.Floor(size * 7);
            for (var iz = 0; iz < blockSize - 1; iz++) {
                if ((location.Z + iz) >= mapZ)
                    continue;

                data[GetBlockCoords(location.X, location.Y, location.Z + iz)] = 17;
            }
            
            var radius = 0;
            var step = 0;

            for (int iz = blockSize; iz >= 3; iz--) {
                List<Vector2S> tdpa = Generate2dPointArray(-radius, radius, true);
                foreach (Vector2S pt in tdpa) {
                    if (radius != 0 && (Math.Abs(pt.X) >= radius || Math.Abs(pt.Y) >= radius)) continue;

                    int currentIndex = GetBlockCoords(location.X + pt.X, location.Y + pt.Y, location.Z + iz);
                    byte currentType = data[currentIndex];
                    if (currentType != 0) continue;

                    data[currentIndex] = 18;
                }
                step += 1;
                if (step == 3) {
                    step = 0;
                    radius -= 1;
                }
                else {
                    radius += 1;
                }
                    
            }

            return data;
        }

        private byte[] CreateTree(byte[] data, int x, int y, int z, double random, string treeType, int mapZ) {
            switch (treeType) {
                case "oak":
                case "birch":
                    return GenerateOak(data, new Vector3S(x, y, z), random); // -- In the original, there is no difference in generation logic between these two.
                case "pine":
                    return GeneratePine(data, new Vector3S(x, y, z), random, mapZ);

            }
            return data;
        }
    }
}