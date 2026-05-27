using System;
using ZBase.Common;

namespace ZBase.CTF;

public class GameTimers : TaskItem
{
    public GameTimers()
    {
        Interval = new TimeSpan(0, 0, 1);
        ZBase.Common.TaskScheduler.RegisterTask("CTF Game Timer", this);
    }

    public override void Setup() {}
    public override void Main()
    {
        
    }

    public override void Teardown() {}
}
