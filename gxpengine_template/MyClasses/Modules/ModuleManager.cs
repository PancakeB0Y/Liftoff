using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TiledMapParser;
using static gxpengine_template.MyClasses.Module;

namespace gxpengine_template.MyClasses.Modules
{
    public class ModuleManager : Sprite
    {
        public event Action ScoreUpdate;

        readonly Dictionary<Module.ModuleTypes, Module> modulesOn;
        readonly Dictionary<Module.ModuleTypes, List<Module>> prefabsByType;

        readonly List<GameObject> modulePrefabs;

        List<AnimationSprite> transitionsClose;
        List<AnimationSprite> transitionsOpen;

        //score manager
        public int Score
        {
            get => _currentScore;
            set
            {
                _currentScore = value;
                _scoreTextMesh.Text = value.ToString();
            }
        }
        int _currentScore;
        readonly TextMesh _scoreTextMesh;

        Dictionary<ModuleTypes, bool> _modulesSpawned = new Dictionary<ModuleTypes, bool>
        {
            { Module.ModuleTypes.Dpad, false },
            { Module.ModuleTypes.OneButton, false },
            { Module.ModuleTypes.ThreeButtons, false },
            { Module.ModuleTypes.Switch, false }
        };
        bool _allModulesSpawned = false;

        public int HighScore
        {
            get => _currentHighScore;
            set
            {
                _currentHighScore = value;
                _highScoreTextMesh.Text = value.ToString();
            }
        }
        int _currentHighScore;
        readonly TextMesh _highScoreTextMesh;
        readonly Timer[] _timers;

        public ModuleManager(TiledObject data) : base("Assets/square.png", true, false)
        {
            modulesOn = new Dictionary<Module.ModuleTypes, Module>
            {
                { Module.ModuleTypes.Dpad, null },
                { Module.ModuleTypes.OneButton, null },
                { Module.ModuleTypes.ThreeButtons, null },
                { Module.ModuleTypes.Switch, null }
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

            _scoreTextMesh = new TextMesh("0", 200, 200, MyUtils.MainColor, Color.Transparent, CenterMode.Min, textSize: 30, fontFileName: "Assets/cour.ttf", fontStyle: FontStyle.Bold);
            _highScoreTextMesh = new TextMesh("0", 200, 200, MyUtils.MainColor, Color.Transparent, CenterMode.Min, textSize: 30, fontFileName: "Assets/cour.ttf", fontStyle: FontStyle.Bold);

            _timers = new Timer[4];
            LoadTimers(data);

            AddChild(new Coroutine(Init()));
        }

        void AddScore(Module module)
        {
            Score += DifficultyManager.Instance.GetMultipliedScore(module.SuccesScore);
        }

        IEnumerator Init()
        {
            yield return null;
            MyUtils.MyGame.CurrentScene.LateAddChild(_scoreTextMesh);
            _scoreTextMesh.SetXY(game.width - 145, 69);
            MyUtils.MyGame.CurrentScene.LateAddChild(_highScoreTextMesh);
            _highScoreTextMesh.SetXY(game.width - 145, game.height - 69);
            _highScoreTextMesh.Text = SaveManager.Instance.GetHighScore().ToString();

            LoadEmptyModule();
        }

        void LoadTimers(TiledObject data)
        {
            string filePath = data.GetStringProperty("TimerFilePath");
            for (int i = 0; i < _timers.Length; i++)
            {
                _timers[i] = new Timer(filePath, false, false);
                _timers[i].SetXY(data.GetFloatProperty($"Timer{i}X"), data.GetFloatProperty($"Timer{i}Y"));
                _timers[i].SetAlpha(0);
                MyUtils.MyGame.CurrentScene.LateAddChild(_timers[i]);
            }
        }

        void UpdateTimers()
        {
            foreach (var item in modulesOn)
            {
                if (item.Value == null) continue;
                float persentage = item.Value.CurrTime / item.Value.TotalTime;

                switch (item.Key)
                {
                    case ModuleTypes.Switch:
                        _timers[3].SetPersentage(persentage);
                        break;
                    case ModuleTypes.Dpad:
                        _timers[0].SetPersentage(persentage);
                        break;
                    case ModuleTypes.ThreeButtons:
                        _timers[2].SetPersentage(persentage);
                        break;
                    case ModuleTypes.OneButton:
                        _timers[1].SetPersentage(persentage);
                        break;
                }
            }
        }

        IEnumerator ReplaceModuleCR(ModuleTypes moduleType)
        {
            if (_modulesSpawned[moduleType])
            {
                LoadEmptyModule();
            }
            _modulesSpawned[moduleType] = true;

            var anim = PlayCloseAnimation(moduleType);

            while (anim.currentFrame < anim.frameCount)
            {
                yield return null;
            }

            if (modulesOn[moduleType] != null)
            {
                modulesOn[moduleType].End -= ReplaceModule;
                modulesOn[moduleType].Success -= AddScore;
                modulesOn[moduleType].Destroy();
            }

            PlayOpenAnimation(moduleType);

            Module newModule = GetRandomModule(moduleType, 1);
            modulesOn[moduleType] = newModule;
            modulesOn[moduleType].StartModule();
            modulesOn[moduleType].End += ReplaceModule;
            modulesOn[moduleType].Success += AddScore;

            MyUtils.MyGame.CurrentScene.AddChild(newModule);

            switch (moduleType)
            {
                case ModuleTypes.Dpad:
                    newModule.SetXY(newModule.width / 2 + 80, game.height / 2);
                    break;
                case ModuleTypes.ThreeButtons:
                    newModule.SetXY(game.width / 2, game.height - newModule.height - 60);
                    break;
                case ModuleTypes.OneButton:
                    newModule.SetXY(game.width / 2, 140);
                    break;
                case ModuleTypes.Switch:
                    newModule.SetXY(game.width - 330, game.height / 2 - 75);
                    break;
            }

            Console.WriteLine(newModule.TotalTime);
        }

        void ReplaceModule(Module.ModuleTypes moduleType)
        {
            AddChild(new Coroutine(ReplaceModuleCR(moduleType)));
        }

        void LoadEmptyModule()
        {
            if (_allModulesSpawned) { return; }

            bool areAllModulesSpawned = true;

            List<ModuleTypes> notSpawnedModules = new List<ModuleTypes>();

            foreach (KeyValuePair<ModuleTypes, bool> entry in _modulesSpawned)
            {
                if (!entry.Value)
                {
                    areAllModulesSpawned = false;
                    notSpawnedModules.Add(entry.Key);
                }
            }

            if (areAllModulesSpawned)
            {
                _allModulesSpawned = true;
                return;
            }

            int randModule = Utils.Random(0, notSpawnedModules.Count);
            switch (notSpawnedModules[randModule])
            {
                case ModuleTypes.Dpad:
                    ReplaceModule(ModuleTypes.Dpad);
                    _timers[0].SetAlpha(1);
                    break;
                case ModuleTypes.OneButton:
                    ReplaceModule(ModuleTypes.OneButton);
                    _timers[1].SetAlpha(1);
                    break;
                case ModuleTypes.ThreeButtons:
                    ReplaceModule(ModuleTypes.ThreeButtons);
                    _timers[2].SetAlpha(1);
                    break;
                case ModuleTypes.Switch:
                    ReplaceModule(ModuleTypes.Switch);
                    _timers[3].SetAlpha(1);
                    break;
            }
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
                    closeModuleAnimation.SetXY(game.width / 2 - 200, game.height - 113);
                    break;
                case ModuleTypes.OneButton:
                    closeModuleAnimation.SetXY(game.width / 2 - 205, -10);
                    break;
                case ModuleTypes.Switch:
                    closeModuleAnimation.SetXY(game.width - 404, game.height / 2 - 55);

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
                    openModuleAnimation.SetXY(game.width / 2 - 200, game.height - 113);
                    break;
                case ModuleTypes.OneButton:
                    openModuleAnimation.SetXY(game.width / 2 - 205, -10);
                    break;
                case ModuleTypes.Switch:
                    openModuleAnimation.SetXY(game.width - 404, game.height / 2 - 55);
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
            UpdateTimers();
        }

        protected override void OnDestroy()
        {
            ScoreUpdate = null;
        }
    }
}