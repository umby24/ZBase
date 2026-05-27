namespace ZBase.CTF;
using ZBase;
using ZBase.Common;
using ZBase.Commands;
using ZBase.BuildModes;

public class Main : ZBasePlugin
{
    public Main()
    {
        this.PluginName = "CTF";
        this.PluginVersion = 1;
        this.PluginAuthor = "umby24";    
    }

    public override void PluginInit()
    {
        var buildModes = new BuildMode[]
        {
            new BuildModes.SetFlag()
        };

        var commands = new Command[] {
            new Commands.CTFCtrl()
        };

        foreach (Command command in commands)
        {
            CommandHandler.RegisterCommand(command);
        }
        foreach(var bm in buildModes)
        {
            BuildModeManager.Instance.RegisterBuildMode(bm);
        }
    }

    public override void PluginUnload()
    {
        throw new NotImplementedException();
    }
}
