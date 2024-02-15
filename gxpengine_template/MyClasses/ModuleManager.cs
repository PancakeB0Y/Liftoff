using GXPEngine;
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
        List<Module> modulesLeft;

        Module[] modulesOn = new Module[]
        {
            null, null, null, null
        };

        public ModuleManager()
        {
            modulesLeft = new List<Module>();
        }

        public void AddModule(Module module)
        {
            modulesLeft.Add(module);
        }

        public List<Module> GetModules()
        {
            return modulesLeft;
        }

        public void UpdateModulesOn()
        {
            for (int i = modulesLeft.Count - 1; i >= 0; i--)
            {
                switch (modulesLeft[i].modulePos)
                {
                    case Module.modulePosition.Left:
                        modulesOn[0] = modulesLeft[i];
                        modulesLeft[i].StartTimer();
                        modulesLeft.RemoveAt(0);
                        break;
                    case Module.modulePosition.Right:
                        modulesOn[1] = modulesLeft[i];
                        modulesLeft[i].StartTimer();
                        modulesLeft.RemoveAt(0);
                        break;
                    case Module.modulePosition.Top:
                        modulesOn[2] = modulesLeft[i];
                        modulesLeft[i].StartTimer();
                        modulesLeft.RemoveAt(0);
                        break;
                    case Module.modulePosition.Bottom:
                        modulesOn[3] = modulesLeft[i];
                        modulesLeft[i].StartTimer();
                        modulesLeft.RemoveAt(0);
                        break;
                }
            }
        }

        void Update()
        {

        }
    }
}
