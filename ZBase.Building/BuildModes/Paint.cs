using ZBase.Common;

namespace ZBase.Building.BuildModes {
    public class Paint : BuildMode {
        public Paint() {
            Name = Constants.PaintBuildModeName;
        }
        public override void Invoke(Vector3S location, byte mode, Block block) {
            ExecutingClient.ClientPlayer.Entity.CurrentMap.SetBlockId(location, block.Id);
        }
    }
}