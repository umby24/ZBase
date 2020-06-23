using System;
using System.Threading;
using System.Threading.Tasks;
using ZBase.Building.BuildModes;
using ZBase.BuildModes;
using ZBase.Commands;
using ZBase.Common;

namespace ZBase.Building {
    public class Main : ZBasePlugin {
        public Main() {
            PluginName = "ZBase.Building";
            PluginAuthor = "umby24";
            PluginVersion = 1;
            ApiVersion = 1;
        }
        
        public override void PluginInit() {
            Task.Run(() => {
                Thread.Sleep(1000);
                BuildModeManager.Instance.RegisterBuildMode(new Line());
                BuildModeManager.Instance.RegisterBuildMode(new Box());
                BuildModeManager.Instance.RegisterBuildMode(new Sphere());
                CommandHandler.RegisterCommand(new Commands.Box());
                CommandHandler.RegisterCommand(new Commands.HBox());
                CommandHandler.RegisterCommand(new Commands.Line());
                CommandHandler.RegisterCommand(new Commands.Sphere());
                CommandHandler.RegisterCommand(new Commands.HSphere());
            });
        }

        public override void PluginUnload() {
        }
    }
}