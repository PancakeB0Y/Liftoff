using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiledMapParser;
using static gxpengine_template.MyClasses.Module;

namespace gxpengine_template.MyClasses.Modules
{
    public class ModuleManager : Sprite
    {
        readonly Dictionary<Module.ModuleTypes, Module> modulesOn;
        readonly Dictionary<Module.ModuleTypes, List<Module>> prefabsByType;

        readonly List<GameObject> modulePrefabs;
        public ModuleManager(TiledObject data) : base("Assets/square.png", true, false)
        {
            modulesOn = new Dictionary<Module.ModuleTypes, Module>
            {
                { Module.ModuleTypes.Switch, null },
                { Module.ModuleTypes.Dpad, null },
                { Module.ModuleTypes.ThreeButtons, null },
                { Module.ModuleTypes.OneButton, null }
            };

            prefabsByType = new Dictionary<Module.ModuleTypes, List<Module>>
            {
                { Module.ModuleTypes.Switch, new List<Module>() },
                { Module.ModuleTypes.Dpad, new List<Module>() },
                { Module.ModuleTypes.ThreeButtons, new List<Module>() },
                { Module.ModuleTypes.OneButton, new List<Module>() }
            };

            alpha = 0f;
            LoadModules(out modulePrefabs);
            prefabsByType = OrganizeByType(modulePrefabs, prefabsByType);

            AddChild(new Coroutine(LoadStartingModules()));
        }

        IEnumerator LoadStartingModules()
        {
            yield return null;

            ReplaceModule(Module.ModuleTypes.Dpad);
            ReplaceModule(Module.ModuleTypes.ThreeButtons);
            ReplaceModule(Module.ModuleTypes.OneButton);
            ReplaceModule(Module.ModuleTypes.Switch);
        }

        void ReplaceModule(Module.ModuleTypes moduleType)
        {
            Module newModule = GetRandomModule(moduleType, 1);
            modulesOn[newModule.moduleType] = newModule;
            modulesOn[newModule.moduleType].StartModule();
            modulesOn[newModule.moduleType].End += ReplaceModule;
            MyUtils.MyGame.CurrentScene.AddChild(newModule);

            switch (moduleType)
            {
                case ModuleTypes.Dpad:
                    newModule.SetXY(newModule.width, game.height / 2);
                    break;
                case ModuleTypes.ThreeButtons:
                    newModule.SetXY(game.width - newModule.width, game.height / 2);
                    break;
                case ModuleTypes.OneButton:
                    newModule.SetXY(game.width / 2, newModule.height);
                    break;
                case ModuleTypes.Switch:
                    newModule.SetXY(game.width / 2, game.height - (newModule.height * 2));
                    break;
            }

        }

        Module GetRandomModule(Module.ModuleTypes moduleType, int Difficulty)
        {
            Random rnd = new Random();

            int r = rnd.Next(prefabsByType[moduleType].Count);

            Module module = prefabsByType[moduleType][r];

            int i = 0;

            while (module.Difficulty != Difficulty && i < 10)
            {
                r = rnd.Next(prefabsByType[moduleType].Count);
                module = prefabsByType[moduleType][r];
                i++;
            }

            return (Module)module.Clone();
        }

        void LoadModules(out List<GameObject> modulePrefabs)
        {
            Pivot pivot = new Pivot();
            var loader = new TiledLoader("Assets/DifficultyPrefabs.tmx", pivot, addColliders: false, autoInstance: true);

            int index;

            //level objects
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "Object Layer 1", out index))
            {
                loader.rootObject = pivot;
                loader.addColliders = false;
                loader.LoadObjectGroups(index);
            }

            modulePrefabs = pivot.GetChildren();
        }

        Dictionary<Module.ModuleTypes, List<Module>> OrganizeByType(List<GameObject> modules, Dictionary<Module.ModuleTypes, List<Module>> prefabsByType)
        {
            foreach (Module module in modules)
            {
                prefabsByType[module.moduleType].Add(module);
            }

            return prefabsByType;
        }


    }
}