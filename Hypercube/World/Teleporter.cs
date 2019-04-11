using System.Collections.Generic;
using ZBase.Common;

namespace ZBase.World {
    public class Teleporter {
        public string Name { get; set; }
        public Vector3S OriginStart { get; set; }
        public Vector3S OriginEnd { get; set; }
        public Vector3S Destination { get; set; }
        public byte DestinationLook { get; set; }
        public byte DestinationRot { get; set; }
        public string DestinationMap { get; set; }

        public static Teleporter Matches(Vector3S location, IEnumerable<Teleporter> portals) {
            foreach (Teleporter teleporter in portals) {
                if (teleporter.InRange(location))
                    return teleporter;
            }

            return null;
        }

        public bool InRange(Vector3S location) {
            if (location.X >= OriginStart.X && location.X <= OriginEnd.X) {
                if (location.Y >= OriginStart.Y && location.Y <= OriginEnd.Y) {
                    if (location.Z >= OriginStart.Z && location.Z <= OriginEnd.Z)
                        return true;
                }
            }

            return false;
        }
        
        public Teleporter() {

        }

        public Teleporter(Vector3S start, Vector3S end, Vector3S dest, string name, byte look, byte rot, string map) {
            OriginStart = start;
            OriginEnd = end;
            Destination = dest;
            Name = name;
            DestinationLook = look;
            DestinationRot = rot;
            DestinationMap = map;
        }
    }
}
