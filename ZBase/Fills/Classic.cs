using System.Diagnostics;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;
using ZBase.Fills.Common;
using ZBase.Fills.Foliage;
using System;

namespace ZBase.Fills {
    public sealed class JavaRandom {
        
        long seed;
        const long value = 0x5DEECE66DL;
        const long mask = (1L << 48) - 1;
        
        public JavaRandom(int seed) { SetSeed(seed); }
        public void SetSeed(int seed) {
            this.seed = (seed ^ value) & mask;
        }
        
        public int Next(int min, int max) { return min + Next(max - min); }
        
        public int Next(int n) {
            if ((n & -n) == n) { // i.e., n is a power of 2
                seed = (seed * value + 0xBL) & mask;
                long raw = (long)((ulong)seed >> (48 - 31));
                return (int)((n * raw) >> 31);
            }

            int bits, val;
            do {
                seed = (seed * value + 0xBL) & mask;
                bits = (int)((ulong)seed >> (48 - 31));
                val = bits % n;
            } while (bits - val + (n - 1) < 0);
            return val;
        }
        
        public float NextFloat() {
            seed = (seed * value + 0xBL) & mask;
            int raw = (int)((ulong)seed >> (48 - 24));
            return raw / ((float)(1 << 24));
        }
    }

    public class Classic : Mapfill
    {
        int waterLevel, oneY, Width, Length, Height;
        byte[] blocks;
        short[] heightmap;
        JavaRandom rnd;
        int minHeight;
        string CurrentState;

        public Classic() {
            Name = "Classic";
        }
        
        public override void Execute(HcMap map, string[] args)
        {
            var mapSize = map.GetSize();
            blocks = new byte[mapSize.X * mapSize.Y * mapSize.Z];
            Width = mapSize.X; Length = mapSize.Z; Height = mapSize.Y;
            rnd = new JavaRandom(new Random().Next());
            oneY = Width * Length;
            waterLevel = Height / 2;
            minHeight = Height;

            var sw = Stopwatch.StartNew();

            CreateHeightmap();
            CreateStrata();
            CarveCaves();
            CarveOreVeins(0.02f, "Coal Ore", BlockManager.GetBlock("Coal").Id);
            CarveOreVeins(0.01f, "Iron Ore", BlockManager.GetBlock("Iron Ore").Id);
            CarveOreVeins(0.005f, "Gold Ore", BlockManager.GetBlock("Gold Ore").Id);
            FloodFillWaterBorders();
            FloodFillWater();
            FloodFillLava();
            CreateSurfaceLayer();
            PlantFlowers();
            PlantMushrooms();
            PlantTrees();

            map.SetMap(blocks);
            sw.Stop();
            Chat.SendMapChat("Finished generating map in " + sw.Elapsed.TotalSeconds.ToString("0.00") + " seconds", 0, map);
            map.Resend();

        }

        static int Floor(float value) {
            int valueI = (int)value;
            return value < valueI ? valueI - 1 : valueI;
        }
        
        void FillOblateSpheroid(int x, int y, int z, float radius, byte block) {
            int xBeg = Floor(Math.Max(x - radius, 0));
            int xEnd = Floor(Math.Min(x + radius, Width - 1));
            int yBeg = Floor(Math.Max(y - radius, 0));
            int yEnd = Floor(Math.Min(y + radius, Height - 1));
            int zBeg = Floor(Math.Max(z - radius, 0));
            int zEnd = Floor(Math.Min(z + radius, Length - 1));
            float radiusSq = radius * radius;
            
            for (int yy = yBeg; yy <= yEnd; yy++)
                for (int zz = zBeg; zz <= zEnd; zz++)
                    for (int xx = xBeg; xx <= xEnd; xx++)
            {
                int dx = xx - x, dy = yy - y, dz = zz - z;
                if ((dx * dx + 2 * dy * dy + dz * dz) < radiusSq) {
                    int index = (yy * Length + zz) * Width + xx;
                    if (blocks[index] == BlockManager.GetBlock("Stone").Id)
                        blocks[index] = block;
                }
            }
        }
        
        void FloodFill(int startIndex, byte block) {
            if (startIndex < 0) return; // y below map, immediately ignore
            FastIntStack stack = new FastIntStack(4);
            stack.Push(startIndex);    
            
            while (stack.Size > 0) {
                int index = stack.Pop();
                if (blocks[index] != BlockManager.GetBlock(0).Id) continue;
                blocks[index] = block;
                
                int x = index % Width;
                int y = index / oneY;
                int z = (index / Width) % Length;
                
                if (x > 0) stack.Push(index - 1);
                if (x < Width - 1) stack.Push(index + 1);
                if (z > 0) stack.Push(index - Width);
                if (z < Length - 1) stack.Push(index + Width);
                if (y > 0) stack.Push(index - oneY);
            }
        }
        
        sealed class FastIntStack {
            public int[] Values;
            public int Size;
            
            public FastIntStack(int capacity) {
                Values = new int[capacity];
                Size = 0;
            }
            
            public int Pop() {
                return Values[--Size];
            }
            
            public void Push(int item) {
                if (Size == Values.Length) {
                    int[] array = new int[Values.Length * 2];
                    Buffer.BlockCopy(Values, 0, array, 0, Size * sizeof(int));
                    Values = array;
                }
                Values[Size++] = item;
            }
        }

        private void CreateHeightmap()
        {
             CombinedNoise n1 = new CombinedNoise(
                new OctaveNoise(8, rnd), new OctaveNoise(8, rnd));
            CombinedNoise n2 = new CombinedNoise(
                new OctaveNoise(8, rnd), new OctaveNoise(8, rnd));
            OctaveNoise n3 = new OctaveNoise(6, rnd);
            int index = 0;
            short[] hMap = new short[Width * Length];
            
            for (int z = 0; z < Length; z++)
                for (int x = 0; x < Width; x++) 
            {
                double hLow = n1.Compute(x * 1.3f, z * 1.3f) / 12 - 2, height = hLow;
                
                if (n3.Compute(x, z) <= 0) {
                    double hHigh = n2.Compute(x * 1.3f, z * 1.3f) / 10 + 3;
                    height = Math.Max(hLow, hHigh);
                }
                if (height < 0) height *= 0.8f;
                
                int adjHeight = (int)(height + waterLevel);
                minHeight = adjHeight < minHeight ? adjHeight : minHeight;
                hMap[index++] = (short)adjHeight;
            }
            heightmap = hMap;
        }

        void CreateStrata() {
            OctaveNoise n = new OctaveNoise(8, rnd);
            CurrentState = "Creating strata";            
            int hMapIndex = 0, maxY = Height - 1, mapIndex = 0;
            // Try to bulk fill bottom of the map if possible
            int minStoneY = CreateStrataFast();
            byte ground = BlockManager.GetBlock("Dirt").Id;
            byte cliff = BlockManager.GetBlock("Stone").Id;

            for (int z = 0; z < Length; z++)
                for (int x = 0; x < Width; x++) 
            {
                int dirtThickness = (int)(n.Compute(x, z) / 24 - 4);
                int dirtHeight    = heightmap[hMapIndex++];
                int stoneHeight   = dirtHeight + dirtThickness;    
                
                stoneHeight = Math.Min(stoneHeight, maxY);
                dirtHeight  = Math.Min(dirtHeight,  maxY);
                
                mapIndex = minStoneY * oneY + z * Width + x;
                for (int y = minStoneY; y <= stoneHeight; y++) 
                {
                    blocks[mapIndex] = cliff; mapIndex += oneY;
                }
                
                stoneHeight = Math.Max(stoneHeight, 0);
                mapIndex = (stoneHeight + 1) * oneY + z * Width + x;
                for (int y = stoneHeight + 1; y <= dirtHeight; y++) 
                {
                    blocks[mapIndex] = ground; mapIndex += oneY;
                }
            }
        }
        
        int CreateStrataFast() {
            int count, mapIndex = 0;
            
            // Make lava layer at bottom
            count = Length * Width;
            for (int i = 0; i < count; i++)
            {
                blocks[mapIndex++] = BlockManager.GetBlock("Lava").Id;
            }
            
            // Invariant: the lowest value dirtThickness can possible be is -14
            int stoneHeight = minHeight - 14;
            if (stoneHeight <= 0) return 1; // no layer is fully stone
            byte cliff = BlockManager.GetBlock("Stone").Id;
            
            // We can quickly fill in bottom solid layers
            count = stoneHeight * Length * Width;
            for (int i = 0; i < count; i++)
            {
                blocks[mapIndex++] = cliff;
            }
            return stoneHeight;
        }
        
        void CarveCaves() {
            int cavesCount = blocks.Length / 8192;
            CurrentState = "Carving caves";
            
            for (int i = 0; i < cavesCount; i++) 
            {
                double caveX = rnd.Next(Width);
                double caveY = rnd.Next(Height);
                double caveZ = rnd.Next(Length);
                
                int caveLen  = (int)(rnd.NextFloat() * rnd.NextFloat() * 200);
                double theta = rnd.NextFloat() * 2 * Math.PI, deltaTheta = 0;
                double phi   = rnd.NextFloat() * 2 * Math.PI, deltaPhi = 0;
                double caveRadius = rnd.NextFloat() * rnd.NextFloat();
                
                for (int j = 0; j < caveLen; j++) 
                {
                    caveX += Math.Sin(theta) * Math.Cos(phi);
                    caveZ += Math.Cos(theta) * Math.Cos(phi);
                    caveY += Math.Sin(phi);
                    
                    theta = theta + deltaTheta * 0.2;
                    deltaTheta = deltaTheta * 0.9 + rnd.NextFloat() - rnd.NextFloat();
                    phi = phi / 2 + deltaPhi / 4;
                    deltaPhi = deltaPhi * 0.75 + rnd.NextFloat() - rnd.NextFloat();
                    if (rnd.NextFloat() < 0.25) continue;
                    
                    int cenX = (int)(caveX + (rnd.Next(4) - 2) * 0.2);
                    int cenY = (int)(caveY + (rnd.Next(4) - 2) * 0.2);
                    int cenZ = (int)(caveZ + (rnd.Next(4) - 2) * 0.2);
                    
                    double radius = (Height - cenY) / (double)Height;
                    radius = 1.2 + (radius * 3.5 + 1) * caveRadius;
                    radius = radius * Math.Sin(j * Math.PI / caveLen);
                    FillOblateSpheroid(cenX, cenY, cenZ, (float)radius, BlockManager.GetBlock("Air").Id);
                }
            }
        }
        
        void CarveOreVeins(float abundance, string blockName, byte block) {
            int numVeins = (int)(blocks.Length * abundance / 16384);
            CurrentState = "Carving " + blockName;
            
            for (int i = 0; i < numVeins; i++) 
            {
                double veinX = rnd.Next(Width);
                double veinY = rnd.Next(Height);
                double veinZ = rnd.Next(Length);
                
                int veinLen = (int)(rnd.NextFloat() * rnd.NextFloat() * 75 * abundance);
                double theta = rnd.NextFloat() * 2 * Math.PI, deltaTheta = 0;
                double phi = rnd.NextFloat() * 2 * Math.PI, deltaPhi = 0;
                
                for (int j = 0; j < veinLen; j++) 
                {
                    veinX += Math.Sin(theta) * Math.Cos(phi);
                    veinZ += Math.Cos(theta) * Math.Cos(phi);
                    veinY += Math.Sin(phi);
                    
                    theta = deltaTheta * 0.2;
                    deltaTheta = deltaTheta * 0.9 + rnd.NextFloat() - rnd.NextFloat();
                    phi = phi / 2 + deltaPhi / 4;
                    deltaPhi = deltaPhi * 0.9 + rnd.NextFloat() - rnd.NextFloat();
                    
                    float radius = abundance * (float)Math.Sin(j * Math.PI / veinLen) + 1;
                    FillOblateSpheroid((int)veinX, (int)veinY, (int)veinZ, radius, block);
                }
            }
        }
        
        void FloodFillWaterBorders() {
            int waterY = waterLevel - 1;
            int index1 = (waterY * Length + 0) * Width + 0;
            int index2 = (waterY * Length + (Length - 1)) * Width + 0;
            
            CurrentState = "Flooding edge water";
            byte water = BlockManager.GetBlock("Still Water").Id;
            if (water == BlockManager.GetBlock("Air").Id) return;
            
            for (int x = 0; x < Width; x++) 
            {
                FloodFill(index1, water);
                FloodFill(index2, water);
                index1++; index2++;
            }
            
            index1 = (waterY * Length + 0) * Width + 0;
            index2 = (waterY * Length + 0) * Width + (Width - 1);
            for (int z = 0; z < Length; z++) 
            {
                FloodFill(index1, water);
                FloodFill(index2, water);
                index1 += Width; index2 += Width;
            }
        }
        
        void FloodFillWater() {
            int numSources = Width * Length / 800;
            
            CurrentState = "Flooding water";
            byte water = BlockManager.GetBlock("Water").Id;
            if (water == BlockManager.GetBlock("Air").Id) return;
            
            for (int i = 0; i < numSources; i++) 
            {
                int x = rnd.Next(Width), z = rnd.Next(Length);
                int y = waterLevel - rnd.Next(1, 3);
                FloodFill((y * Length + z) * Width + x, water);
            }
        }
        
        void FloodFillLava() {
            int numSources = Width * Length / 20000;
            CurrentState = "Flooding lava";
            
            for (int i = 0; i < numSources; i++) 
            {
                int x = rnd.Next(Width), z = rnd.Next(Length);
                int y = (int)((waterLevel - 3) * rnd.NextFloat() * rnd.NextFloat());
                FloodFill((y * Length + z) * Width + x, BlockManager.GetBlock("Lava").Id);
            }
        }
        
        void CreateSurfaceLayer() {
            OctaveNoise n1 = new OctaveNoise(8, rnd), n2 = new OctaveNoise(8, rnd);
            CurrentState = "Creating surface";
            // TODO: update heightmap
            byte surface = BlockManager.GetBlock("Grass").Id;
            byte sandy   = BlockManager.GetBlock("Sand").Id;
            byte rocky   = BlockManager.GetBlock("Gravel").Id;
            byte water   = BlockManager.GetBlock("Still Water").Id;
            
            int hMapIndex = 0;
            for (int z = 0; z < Length; z++)
                for (int x = 0; x < Width; x++) 
            {
                int y = heightmap[hMapIndex++];
                if (y < 0 || y >= Height) continue;
                
                int index = (y * Length + z) * Width + x;
                byte blockAbove = y >= (Height - 1) ? BlockManager.GetBlock("Air").Id : blocks[index + oneY];
                if (blockAbove == water && (n2.Compute(x, z) > 12)) {
                    blocks[index] = rocky;
                } else if (blockAbove == BlockManager.GetBlock("Air").Id) {
                    blocks[index] = (y <= waterLevel && (n1.Compute(x, z) > 8)) ? sandy : surface;
                }
            }
        }
        
        void PlantFlowers() {
            int numPatches = Width * Length / 3000;
            CurrentState = "Planting flowers";
            byte surface = BlockManager.GetBlock("Grass").Id;
            
            for (int i = 0; i < numPatches; i++) 
            {
                byte type  = (byte)(BlockManager.GetBlock("Yellow Flower").Id + rnd.Next(2));
                int patchX = rnd.Next(Width), patchZ = rnd.Next(Length);
                for (int j = 0; j < 10; j++) 
                {
                    int flowerX = patchX, flowerZ = patchZ;
                    for (int k = 0; k < 5; k++) 
                    {
                        flowerX += rnd.Next(6) - rnd.Next(6);
                        flowerZ += rnd.Next(6) - rnd.Next(6);
                        if (flowerX < 0 || flowerZ < 0 || flowerX >= Width || flowerZ >= Length)
                            continue;
                        
                        int flowerY = heightmap[flowerZ * Width + flowerX] + 1;
                        if (flowerY <= 0 || flowerY >= Height) continue;
                        
                        int index = (flowerY * Length + flowerZ) * Width + flowerX;
                        if (blocks[index] == BlockManager.GetBlock("Air").Id && blocks[index - oneY] == surface)
                            blocks[index] = type;
                    }
                }
            }
        }
        
        void PlantMushrooms() {
            int numPatches = blocks.Length / 2000;
            CurrentState = "Planting mushrooms";
            byte cliff = BlockManager.GetBlock("Stone").Id;
            
            for (int i = 0; i < numPatches; i++) 
            {
                byte type  = (byte)(BlockManager.GetBlock("Brown Mushroom").Id + rnd.Next(2));
                int patchX = rnd.Next(Width);
                int patchY = rnd.Next(Height);
                int patchZ = rnd.Next(Length);
                
                for (int j = 0; j < 20; j++) 
                {
                    int mushX = patchX, mushY = patchY, mushZ = patchZ;
                    for (int k = 0; k < 5; k++) 
                    {
                        mushX += rnd.Next(6) - rnd.Next(6);
                        mushZ += rnd.Next(6) - rnd.Next(6);
                        if (mushX < 0 || mushZ < 0 || mushX >= Width || mushZ >= Length)
                            continue;
                        int solidHeight = heightmap[mushZ * Width + mushX];
                        if (mushY >= (solidHeight - 1))
                            continue;
                        
                        int index = (mushY * Length + mushZ) * Width + mushX;
                        if (blocks[index] == BlockManager.GetBlock("Air").Id && blocks[index - oneY] == cliff)
                            blocks[index] = type;
                    }
                }
            }
        }
        
        void PlantTrees() {
            int numPatches = Width * Length / 4000;
            CurrentState = "Planting trees";
            byte surface = BlockManager.GetBlock("Grass").Id;
            
            Tree tree = GetTreeGen();
            if (tree == null) return;
            Random R = new Random();
            
            for (int i = 0; i < numPatches; i++) 
            {
                int patchX = rnd.Next(Width), patchZ = rnd.Next(Length);
                
                for (int j = 0; j < 20; j++) 
                {
                    int treeX = patchX, treeZ = patchZ;
                    for (int k = 0; k < 20; k++) 
                    {
                        treeX += rnd.Next(6) - rnd.Next(6);
                        treeZ += rnd.Next(6) - rnd.Next(6);
                        if (treeX < 0 || treeZ < 0 || treeX >= Width ||
                            treeZ >= Length || rnd.NextFloat() >= 0.25)
                            continue;
                        
                        int treeY = heightmap[treeZ * Width + treeX] + 1;
                        if (treeY >= Height) continue;
                        int treeHeight = tree.DefaultSize(R);
                        
                        int index = (treeY * Length + treeZ) * Width + treeX;
                        byte blockUnder = treeY > 0 ? blocks[index - oneY] : BlockManager.GetBlock("Air").Id;
                        
                        if (blockUnder == surface && CanGrowTree(treeX, treeY, treeZ, treeHeight)) {
                            tree.SetData(R, treeHeight);
                            
                            tree.Generate((ushort)treeX, (ushort)treeY, (ushort)treeZ, (xT, yT, zT, bT) =>
                                  {
                                      int idx = (yT * Length + zT) * Width + xT;
                                      // don't place leafs over trunk
                                      if (bT == BlockManager.GetBlock("Leaves").Id && blocks[idx] == BlockManager.GetBlock("Log").Id) return;
                                      blocks[idx] = (byte)bT;
                                  });
                        }
                    }
                }
            }
        }
        
        Tree GetTreeGen() {
            return new ClassicTree() { rng = rnd };
        }
        
        bool CanGrowTree(int treeX, int treeY, int treeZ, int treeHeight) {
            // check tree bounds
            if (treeY < 0     || (treeY + treeHeight - 1) >= Height) return false;
            if (treeX - 2 < 0 || treeX + 2 >= Width)  return false;
            if (treeZ - 2 < 0 || treeZ + 2 >= Length) return false;
            
            // check tree base            
            int baseHeight = treeHeight - 4;
            for (int y = treeY; y < treeY + baseHeight; y++)
                for (int z = treeZ - 1; z <= treeZ + 1; z++)
                    for (int x = treeX - 1; x <= treeX + 1; x++)
            {
                int index = (y * Length + z) * Width + x;
                if (blocks[index] != 0) return false;
            }
            
            // and also check canopy
            for (int y = treeY + baseHeight; y < treeY + treeHeight; y++)
                for (int z = treeZ - 2; z <= treeZ + 2; z++)
                    for (int x = treeX - 2; x <= treeX + 2; x++)
            {
                int index = (y * Length + z) * Width + x;
                if (blocks[index] != 0) return false;
            }
            return true;
        }
    }
}