using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TiledMapParser;
using static gxpengine_template.MyClasses.Module;

namespace gxpengine_template.MyClasses.Modules
{
    public class ModuleManager : Sprite
    {
        readonly Dictionary<Module.ModuleTypes, Module> modulesOn;
        readonly Dictionary<Module.ModuleTypes, List<Module>> prefabsByType;

        readonly List<GameObject> modulePrefabs;

        List<AnimationSprite> transitionsClose;
        List<AnimationSprite> transitionsOpen;
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

            transitionsClose = new List<AnimationSprite>();
            transitionsOpen = new List<AnimationSprite>();

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

        IEnumerator ReplaceModuleCR(ModuleTypes moduleType)
        {
            var anim = PlayCloseAnimation(moduleType);

            while (anim.currentFrame < anim.frameCount)
            {
                yield return null;
            }

            if (modulesOn[moduleType] != null)
            {
                modulesOn[moduleType].End -= ReplaceModule;
                modulesOn[moduleType].Destroy();
            }

            PlayOpenAnimation(moduleType);

            Module newModule = GetRandomModule(moduleType, 1);
            modulesOn[moduleType] = newModule;
            modulesOn[moduleType].StartModule();
            modulesOn[moduleType].End += ReplaceModule;
            MyUtils.MyGame.CurrentScene.AddChild(newModule);

            switch (moduleType)
            {
                case ModuleTypes.Dpad:
                    newModule.SetXY(newModule.width / 2 + 80, game.height / 2);
                    break;
                case ModuleTypes.ThreeButtons:
                    newModule.SetXY(game.width - newModule.width + 70, game.height / 2);
                    break;
                case ModuleTypes.OneButton:
                    newModule.SetXY(game.width / 2, 140);
                    break;
                case ModuleTypes.Switch:
                    newModule.SetXY(game.width / 2 - newModule.width - 50, game.height - 220);
                    break;
            }
        }
        void ReplaceModule(Module.ModuleTypes moduleType)
        {
            AddChild(new Coroutine(ReplaceModuleCR(moduleType)));
        }

        Module GetRandomModule(Module.ModuleTypes moduleType, int Difficulty)
        {
            List<Module> modulesByDifficulty = new List<Module>();

            foreach (Module curModule in prefabsByType[moduleType])
            {
                if (curModule.Difficulty == Difficulty)
                {
                    modulesByDifficulty.Add(curModule);
                }
            }

            if (modulesByDifficulty.Count >= 0)
            {
                int r = Utils.Random(0, modulesByDifficulty.Count);

                Module module = modulesByDifficulty[r];

                return (Module)module.Clone();
            }

            return null;
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

        AnimationSprite PlayCloseAnimation(Module.ModuleTypes moduleType)
        {
            AnimationSprite closeModuleAnimation = new AnimationSprite("Assets/Transition_MicroGames.png", 8, 7, 52, true, false);
            closeModuleAnimation.SetCycle(1, 26, 1);

            closeModuleAnimation.width = 460;
            closeModuleAnimation.height = 310;

            switch (moduleType)
            {
                case ModuleTypes.Dpad:
                    closeModuleAnimation.SetXY(-20, game.height / 2 - 63);
                    break;
                case ModuleTypes.ThreeButtons:
                    closeModuleAnimation.SetXY(game.width - 404, game.height / 2 - 55);
                    break;
                case ModuleTypes.OneButton:
                    closeModuleAnimation.SetXY(game.width / 2 - 205, -10);
                    break;
                case ModuleTypes.Switch:
                    closeModuleAnimation.SetXY(game.width / 2 - 200, game.height - 113);
                    break;
            }

            AddChild(closeModuleAnimation);
            transitionsClose.Add(closeModuleAnimation);
            return closeModuleAnimation;
        }

        void PlayOpenAnimation(Module.ModuleTypes moduleType)
        {
            AnimationSprite openModuleAnimation = new AnimationSprite("Assets/Transition_MicroGames.png", 8, 7, 52, true, false);
            openModuleAnimation.SetCycle(26, 52, 1);

            openModuleAnimation.width = 460;
            openModuleAnimation.height = 310;

            switch (moduleType)
            {
                case ModuleTypes.Dpad:
                    openModuleAnimation.SetXY(-20, game.height / 2 - 63);
                    break;
                case ModuleTypes.ThreeButtons:
                    openModuleAnimation.SetXY(game.width - 404, game.height / 2 - 55);
                    break;
                case ModuleTypes.OneButton:
                    openModuleAnimation.SetXY(game.width / 2 - 205, -10);
                    break;
                case ModuleTypes.Switch:
                    openModuleAnimation.SetXY(game.width / 2 - 200, game.height - 113);
                    break;
            }

            AddChild(openModuleAnimation);
            transitionsOpen.Add(openModuleAnimation);
        }

        void AnimateTransitions()
        {
            foreach (AnimationSprite transition in transitionsClose)
            {
                transition.AnimateFixed();
                if (transition.currentFrame >= transition.frameCount)
                {
                    transition.Destroy();
                }
            }

            foreach (AnimationSprite transition in transitionsOpen)
            {
                transition.AnimateFixed();
                if (transition.currentFrame >= 52)
                {
                    transition.Destroy();
                }
            }
        }

        void Update()
        {
            AnimateTransitions();
        }
    }
}