using ZBase.Common;
using ZBase.World;

namespace ZBase.Fills {
    public class Blank : Mapfill {
        public Blank() {
            Name = "Blank";
        }

        public override void Execute(HcMap map, string[] args) {
            Vector3S mapSize = map.GetSize();
            MapSize = mapSize;
            var data = new byte[mapSize.X * mapSize.Y * mapSize.Z];
            map.SetMap(data);
            map.Resend();
        }
    }
}
