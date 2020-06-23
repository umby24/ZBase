using System;
using System.Collections.Generic;
using ZBase.Common;

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
            
            var currentMap = ExecutingClient.ClientPlayer.Entity.CurrentMap;
            var myArray = Build3dArray(brushSize);
            
            for (var ix = -brushSize-2; ix < brushSize +2; ix++) {
                for (var iy = -brushSize-2; iy < brushSize +2; iy++) {
                    for (var iz = -brushSize-2; iz < brushSize +2; iz++) {
                        var blockCoord = new Vector3S(location.X + ix, location.Y + iy, location.Z + iz);
                        var blockType = currentMap.GetBlockId(blockCoord);
                        
                        if ((mode == 1 && blockType > 0) || (mode == 0 && blockType == 0)) {
                            var entf = MathF.Sqrt((ix * ix) + (iy * iy) + (iz * iz));
                            if (entf <= brushSize) {
                                for (var iix = -2; iix < 2; iix++) {
                                    for (var iiy = -2; iiy < 2; iiy++) {
                                        for (var iiz = -2; iiz < 2; iiz++) {
                                            var myCoord = new Vector3S(ix+iix, iy+iiy, iz+iiz);
                                            myArray[myCoord] = myArray[myCoord] + myRandom.Next(100) / 250f;
                                        }
                                    }
                                }        
                            }
                        } 
                    }
                }
            }

            for (var ix = -brushSize-1; ix < brushSize+1; ix++) {
                for (var iy = -brushSize-1; iy < brushSize+1; iy++) {
                    for (var iz = -brushSize-1; iz < brushSize+1; iz++) {
                        var myCoord = new Vector3S(ix, iy, iz);
                        if (myArray[myCoord] > 1) {
                            var blockLocation = new Vector3S(location.X + ix, location.Y+iy, location.Z+iz);
                            var blockType = currentMap.GetBlockId(blockLocation);
                            if (mode == 1 && blockType == 0) {
                                currentMap.SetBlockId(blockLocation, block.Id);
                            } else if (mode == 0 && (block.Id == blockType || block.Id == 3 && blockType == 2)) {
                                currentMap.SetBlockId(blockLocation, 0);
                            }
                        }
                    }
                }
            }
        }

        static Dictionary<Vector3S, float> Build3dArray(int brushSize) {
            var result = new Dictionary<Vector3S, float>();
            
            for (var ix = -brushSize - 3; ix < brushSize + 3; ix++) {
                for (var iy = -brushSize - 3; iy < brushSize + 3; iy++) {
                    for (var iz = -brushSize - 3; iz < brushSize + 3; iz++) {
                        result.Add(new Vector3S(ix, iy, iz), 0.0f);
                    }
                }
            }

            return result;
        }
    }
}