using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class ModuleManager : Sprite
    {
        readonly Dictionary<Module.ModuleTypes, Module> modulesOn;
        public ModuleManager(TiledObject data) : base("Assets/square.png", true, false)
        {
            modulesOn = new Dictionary<Module.moduleTypes, Module>
            {
                { Module.moduleTypes.Switch, null },
                { Module.moduleTypes.Dpad, null },
                { Module.moduleTypes.ThreeButtons, null },
                { Module.moduleTypes.OneButton, null }
            };

            alpha = 0f;
            LoadModule(Module.moduleTypes.OneButton);
        }

        void ReplaceModule(int moduleLeftIndex, Module newModule)
        {
            modulesOn[newModule.moduleType] = newModule;
            modulesOn[newModule.moduleType].StartModule();
            modulesOn[newModule.moduleType].End += UpdateModule;
        }

        void UpdateModule(Module.ModuleTypes moduleType)
        {
            //ReplaceModule(index, modulesLeft[index]);
        }

        void LoadModule(Module.ModuleTypes moduleType)
        {
            Pivot pivot = new Pivot();
            var loader = new TiledLoader("Assets/" + moduleType + ".tmx", pivot, addColliders: false, autoInstance: true);

            int index;

            //level objects
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "Object Layer 1", out index))
            {
                loader.rootObject = pivot;
                loader.addColliders = true;
                loader.LoadObjectGroups(index);
            }
        }
    }
}