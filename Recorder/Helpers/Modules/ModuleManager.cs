using System.Collections.Generic;
using System.Linq;

namespace Recorder.Helpers.Modules
{
    public class ModuleManager : Dictionary<string, Module>
    {
        private static ModuleManager _manager;

        private ModuleManager()
        {
        }

        public static ModuleManager GetManager()
        {
            if (_manager == null)
                _manager = new ModuleManager();
            return _manager;
        }

        public void StartModules()
        {
            Values.ToList().ForEach(module => module.Start());
        }

        public void StopModules()
        {
            Values.ToList().ForEach(module => module.Stop());
        }
    }
}