using System.Collections.Generic;
using ZBase.Common;

namespace ZBase.World {
    public class Teleporter {
        public string Name { get; set; }
        public MinecraftLocation OriginStart { get; set; }
        public MinecraftLocation OriginEnd { get; set; }
        public MinecraftLocation Destination { get; set; }
        public string DestinationMap { get; set; }

        public static Teleporter Matches(MinecraftLocation location, IEnumerable<Teleporter> portals) {
            foreach (Teleporter teleporter in portals) {
                if (teleporter.InRange(location))
                    return teleporter;
            }

            return null;
        }

        public bool InRange(MinecraftLocation location)
        {
            var currentBlock = location.GetAsBlockCoords();
            var startBlock = OriginStart.GetAsBlockCoords();
            var endBlock = OriginEnd.GetAsBlockCoords();

            if (currentBlock.X >= startBlock.X && currentBlock.X <= endBlock.X) {
                if (currentBlock.Y >= startBlock.Y && currentBlock.Y <= endBlock.Y) {
                    if (currentBlock.Z >= startBlock.Z && currentBlock.Z <= endBlock.Z)
                        return true;
                }
            }

            return false;
        }
        
        public Teleporter() {

        }

        public Teleporter(MinecraftLocation start, MinecraftLocation end, MinecraftLocation dest, string name, string map) {
            OriginStart = start;
            OriginEnd = end;
            Destination = dest;
            Name = name;
            DestinationMap = map;
        }
    }
}
