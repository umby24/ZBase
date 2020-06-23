using System.Threading;
using System.Threading.Tasks;
using ZBase.Building.Commands;
using ZBase.BuildModes;
using ZBase.Commands;
using ZBase.Common;
using Box = ZBase.Building.BuildModes.Box;
using Brush = ZBase.Building.BuildModes.Brush;
using Line = ZBase.Building.BuildModes.Line;
using Paint = ZBase.Building.BuildModes.Paint;
using Sphere = ZBase.Building.BuildModes.Sphere;

namespace ZBase.Building {
    public class Main : ZBasePlugin {
        public Main() {
            PluginName = "ZBase.Building";
            PluginAuthor = "umby24";
            PluginVersion = 1;
            ApiVersion = 1;
        }
        
        public override void PluginInit() {
            var buildModes = new BuildMode[] {
                new Line(),
                new Box(),
                new Sphere(),
                new Brush(),
                new Paint()
            };

            var commands = new Command[] {
                new Commands.Box(),
                new HBox(),
                new Commands.Sphere(),
                new Commands.Line(),
                new Commands.Sphere(),
                new HSphere(),
                new Commands.Brush(),
                new Commands.Paint(),
            };
            
            Task.Run(() => {
                Thread.Sleep(1000);
                foreach (BuildMode mode in buildModes) {
                    BuildModeManager.Instance.RegisterBuildMode(mode);
                }

                foreach (Command command in commands) {
                    CommandHandler.RegisterCommand(command);
                }
            });
        }

        public override void PluginUnload() {
        }
    }
}