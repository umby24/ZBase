using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ZBase.Common;
using TaskScheduler = ZBase.Common.TaskScheduler;

namespace ZBase.World {
    public class HcMapActions : TaskItem {
        public ConcurrentQueue<Action> ActionQueue { get; set; }

        public HcMapActions() {
            ActionQueue = new ConcurrentQueue<Action>();
            Interval = TimeSpan.FromSeconds(0.33);
            TaskScheduler.RegisterTask("HcMapActions" + new Random().Next(2035, 193876957), this);
        }
        
        public override void Setup() {

        }

        public override void Main() {
            if (ActionQueue.TryDequeue(out Action toPerform)) {
                Task.Run(toPerform);
            }
        }

        public override void Teardown() {
            ActionQueue = null;
        }
    }
}