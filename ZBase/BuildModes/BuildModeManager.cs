using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.BuildModes {
    public class BuildModeManager : TaskItem
    {
        public static BuildModeManager Instance { get; private set; }
        private Dictionary<string, BuildMode> _buildModes;

        public override void Setup()
        {
            Instance = this;
            _buildModes = new Dictionary<string, BuildMode>();
            ReflectiveLoad();
        }

        public override void Main()
        {
           
        }

        public override void Teardown()
        {
           
        }

        public BuildMode GetBuildmode(string name, Client executingClient)
        {
            if (!_buildModes.ContainsKey(name.ToLower()))
            {
                Logger.Log(LogType.Warning, $"Tried to access a non-existing build mode ({name})");
                return null;
            }

            var bm = _buildModes[name.ToLower()];
            var newBm = (BuildMode)Activator.CreateInstance(bm.GetType());
            newBm.ExecutingClient = executingClient;
            return newBm;
        }

        public void RegisterBuildMode(BuildMode mode)
        {
            if (_buildModes.ContainsKey(mode.Name.ToLower()))
            {
                Logger.Log(LogType.Warning, $"Cannot load buildmode {mode.Name}, another mode exists with that name.");
                return;
            }

            _buildModes.Add(mode.Name.ToLower(), mode);
            Logger.Log(LogType.Debug, $"Registered buildmode {mode.Name}");
        }

        private void ReflectiveLoad()
        {
            Type[] types = Assembly.GetAssembly(typeof(BuildMode)).GetTypes();
            types = types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BuildMode))).ToArray();

            foreach (Type type in types)
            {
                var buildMode = (BuildMode)Activator.CreateInstance(type);
                RegisterBuildMode(buildMode);
            }
        }
    }
}
