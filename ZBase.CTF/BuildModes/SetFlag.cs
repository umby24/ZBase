using System;
using ZBase.Common;

namespace ZBase.CTF.BuildModes;

public class SetFlag : BuildMode
{
    public SetFlag()
    {
        Name = "SetFlag";
    }
    public override void Invoke(Vector3S location, byte mode, Block block)
    {
        if (mode == 0) // -- Ignore block deletes
            return;

        var whichFlag = PlayerState.GetInt(0);
        if (whichFlag == 0)
        {
            GameState.Instance.BlueFlag = new MinecraftLocation();
            GameState.Instance.BlueFlag.SetAsBlockCoords(location);
            SendExecutorMessage("§SBlue flag set.");
        } else
        {
            GameState.Instance.RedFlag = new MinecraftLocation();
            GameState.Instance.RedFlag.SetAsBlockCoords(location);
            SendExecutorMessage("§SRed flag set.");
        }
        
        PlayerState.CurrentMode = null;
        PlayerState.ResendBlocks(ExecutingClient);
    }
}
