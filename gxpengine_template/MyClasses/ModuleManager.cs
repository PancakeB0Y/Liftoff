using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses
{
    public class ModuleManager : GameObject
    {
        readonly Dictionary<Module.moduleTypes, Module> modulesOn;
        readonly List<Module> modulesLeft;



        public ModuleManager()
        {
            modulesLeft = new List<Module>();
            modulesOn = new Dictionary<Module.moduleTypes, Module>();
            modulesOn.Add(Module.moduleTypes.Switch, null);
            modulesOn.Add(Module.moduleTypes.Dpad, null);
            modulesOn.Add(Module.moduleTypes.ThreeButtons, null);
            modulesOn.Add(Module.moduleTypes.OneButton, null);
        }

        public void AddModule(Module module)
        {
            modulesLeft.Add(module);
        }

        public List<Module> GetModules()
        {
            return modulesLeft;
        }

        void ReplaceModule(int moduleLeftIndex, Module newModule)
        {
            modulesOn[newModule.moduleType] = newModule;
            modulesOn[newModule.moduleType].StartModule();
            modulesOn[newModule.moduleType].End += UpdateModule;
            modulesLeft.RemoveAt(moduleLeftIndex);
        }

        void UpdateModule(Module.moduleTypes moduleType)
        {
            int index = GetFirstModuleByType(moduleType);

            if (index == -1)
            {
                Console.WriteLine("No modules left from type " + moduleType);
                return;
            }

            ReplaceModule(index, modulesLeft[index]);
        }

        int GetFirstModuleByType(Module.moduleTypes moduleType)
        {
            for (int i = 0; i < modulesLeft.Count; i++)
            {
                if (modulesLeft[i].moduleType == moduleType)
                {
                    return i;
                }
            }

            return -1;
        }

        public void SetStartingModules()
        {
            for (int i = 0; i < modulesLeft.Count; i++)
            {
                Module curModule = modulesLeft[i];

                if (modulesOn[curModule.moduleType] == null)
                {
                    ReplaceModule(i, curModule);
                    i--;
                }
            }
        }
    }
}
