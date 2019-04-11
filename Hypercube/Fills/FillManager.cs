using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZBase.Common;

namespace ZBase.Fills {
    public class FillManager {
        public static Dictionary<string, Mapfill> Fills = new Dictionary<string, Mapfill>(StringComparer.InvariantCultureIgnoreCase);

        public static void LoadFills() {
            Type[] types = Assembly.GetAssembly(typeof(Mapfill)).GetTypes();
            types = types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Mapfill))).ToArray();

            foreach (Type type in types) {
                var cmd = (Mapfill) Activator.CreateInstance(type);
                RegisterFill(cmd);
            }
        }

        private static void RegisterFill(Mapfill fill) {
            Mapfill existing;

            if (Fills.TryGetValue(fill.Name, out existing)) {
                Logger.Log(LogType.Warning, $"Attempted to register existing fill: {fill.Name}");
                return;
            }

            Fills.Add(fill.Name, fill);
            Logger.Log(LogType.Debug, $"Registered fill {fill.Name}");
        }
    }
}
