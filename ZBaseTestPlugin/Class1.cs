using ZBase.Commands;
using ZBase.Common;

namespace ZBaseTestPlugin {
    public class TestPlugin : ZBasePlugin {
        public TestPlugin() {
            PluginAuthor = "umby24";
            PluginName = "Test Plugin";
            PluginVersion = 1;
            ApiVersion = 1;
        }
        
        public override void PluginInit() {
            Logger.Log(LogType.Debug, "Initializing Test Plugin...");
            CommandHandler.RegisterCommand(new DestroyEntities());
            CommandHandler.RegisterCommand(new CreateEntity());
            CommandHandler.RegisterCommand(new PerfCommand());
            Logger.Log(LogType.Debug, "Test Plugin Loaded :)");
        }

        public override void PluginUnload() {
        }
    }
}