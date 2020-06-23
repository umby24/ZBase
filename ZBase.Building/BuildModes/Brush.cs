using System;
using System.Collections.Generic;
using System.Linq;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Building.BuildModes {
    public class Brush : BuildMode {
        public Brush() {
            Name = Constants.BrushBuildModeName;
        }
        public override void Invoke(Vector3S location, byte mode, Block block) {
            var brushSize = PlayerState.GetInt(0);
            var myRandom = new Random();
            // -- In D3, you first build a 3d array of relative coordinates.
            // -- for example if your brush size is 3, it will iterate from -5 to +5. so you have x-5 to x+5, y-5 to y+5, etc.
            
            // -- Next, you iterate size-1 to size+1 in each coordinate direction, add that to the location of the current block hit, and check to see if it is air.
            // -- if not, build 'entf' (Math.Sqrt(ix*ix + iy*iy + iz*iz)) where ix,iy,iz are the iterators.
            // -- if entf <= BrushSize, then for -1 to 1 in each direction, add those to our offsets, then in our 3d array, set it to the existing value plus a random number (Math.Random(100) / 250)
            
            // -- Finally, you iterate just the brush size in each direction. If the block at that offset is air, and the value in the 3d array is >1, we place a block there.
            // -- because the extranous things arn't used and are purely an entropy introducer... im gonna do this to try and simply it.
            
            HcMap currentMap = ExecutingClient.ClientPlayer.Entity.CurrentMap;
            var myArray = Build3dArray(brushSize);

            var buildArray = BuildVectorArray(brushSize + 1);

            foreach (Vector3S position in buildArray) {
                var blockCoord = new Vector3S(location.X + position.X, location.Y + position.Y, location.Z + position.Z);
                var blockType = currentMap.GetBlockId(blockCoord);

                if ((mode != 1 || blockType <= 0) && (mode != 0 || blockType != 0)) 
                    continue;
                
                var entf = MathF.Sqrt((position.X * position.X) + (position.Y * position.Y) + (position.Z * position.Z));

                if (!(entf <= brushSize)) 
                    continue;

                var innerArray = BuildVectorArray(2);
                
                foreach (Vector3S pos in innerArray) {
                    var arrayCoord =new Vector3S(pos.X+position.X, pos.Y+position.Y, pos.Z+position.Z);
                    myArray[arrayCoord] = myArray[arrayCoord] + myRandom.Next(100) / 250f;
                }
            }


            var placeArray = BuildVectorArray(brushSize);
            
            foreach (Vector3S position in placeArray) {
                if (!(myArray[position] > 1)) 
                    continue;
                
                var blockLocation = new Vector3S(location.X + position.X, location.Y+position.Y, location.Z+position.Z);
                var blockType = currentMap.GetBlockId(blockLocation);
                
                if (mode == 1 && blockType == 0) {
                    currentMap.SetBlockId(blockLocation, block.Id);
                } else if (mode == 0 && (block.Id == blockType || block.Id == 3 && blockType == 2)) {
                    currentMap.SetBlockId(blockLocation, 0);
                }
            }
        }

        public static Vector3S[] BuildVectorArray(int bounds) {
            var result = new List<Vector3S>();
            
            for (var ix = -bounds; ix <= bounds; ix++) {
                for (var iy = -bounds; iy <= bounds; iy++) {
                    for (var iz = -bounds; iz <= bounds; iz++) {
                        result.Add(new Vector3S(ix, iy, iz));
                    }
                }    
            }

            return result.ToArray();
        }
        
        public static Dictionary<Vector3S, float> Build3dArray(int brushSize) {
            var vectorArray = BuildVectorArray(brushSize + 2);

            return vectorArray.ToDictionary(item => item, item => 0.0f);
        }
    }
}