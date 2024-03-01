using GXPEngine;
using System;
using System.Collections.Generic;
using TiledMapParser;
using static gxpengine_template.MyClasses.Module;

namespace gxpengine_template.MyClasses
{

    public class MyGame : Game
    {
        public Dictionary<string, IPrefab> Prefabs { get; }

        public Level CurrentScene { get; private set; }
        private string _newSceneName = null;

        readonly Sound buttonSound = new Sound("Assets/Sounds/ButtonSound.wav");
        readonly Sound switchSound = new Sound("Assets/Sounds/SwitchSound.wav");

        public event Action StopSounds;
        //ArduinoReciever _arduinoReciever = new ArduinoReciever();
        public MyGame() : base(1366, 768, false)
        {
            //constructor in custom classes serve as unity Awake function
            //this is where object initialization should take place

            //The Start() in custom classes will serve as unity Start function
            //this is where you will get dependencie/connections with other classes in the scene
            //via FindObjectOfType or other methods

            Prefabs = LoadPrefabs();
            LoadScene("Assets/StartMenu.tmx");
            OnAfterStep += LoadSceneIfNotNull;
        }

        static void Main()
        {
            new MyGame().Start();
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.R))
            {
                ReloadScene();
            }

            if (Input.GetKeyDown(Key.H) || Input.GetKeyDown(Key.J) || Input.GetKeyDown(Key.K) || Input.GetKeyDown(Key.C)
                || Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.A) || Input.GetKeyDown(Key.S) || Input.GetKeyDown(Key.D) || Input.GetKeyDown(Key.SPACE))
            {
                buttonSound.Play();
            }
            if (Input.GetKeyDown(Key.B))
            {
                switchSound.Play();
            }
            if (Input.GetKeyUp(Key.B))
            {
                switchSound.Play();
            }

            //_arduinoReciever.Update();
        }
        private void LoadSceneIfNotNull()
        {
            if (_newSceneName == null) return;
            DestroyAll();
            var level = new Level(_newSceneName);
            CurrentScene = level;
            AddChild(level);
            level.Init();

            _newSceneName = null;
        }

        public void LoadScene(string sceneName)
        {
            _newSceneName = sceneName;
            StopSounds?.Invoke();
        }

        public void ReloadScene()
        {
            _newSceneName = CurrentScene.Name;
            StopSounds?.Invoke();
        }

        protected override void OnDestroy()
        {
            OnAfterStep -= LoadSceneIfNotNull;
        }
        private void DestroyAll()
        {
            foreach (var child in GetChildren())
            {

                if (!(child is INonDestructable)) child.Destroy();

            }
        }

        Dictionary<string, IPrefab> LoadPrefabs()
        {
            var prefabsDictionary = new Dictionary<string, IPrefab>();

            try
            {
                var loader = new TiledLoader("Assets/Prefabs.tmx", MyGame.main, false, autoInstance: true);
                loader.LoadObjectGroups();
                foreach (var obj in FindObjectsOfType<GameObject>())
                    if (obj is IPrefab prefab)
                    {
                        RemoveChild(obj);
                        prefabsDictionary.Add(obj.name, prefab);
                    }
            }
            catch (Exception)
            {
                Console.WriteLine("No Prefabs.tmx file, check spelling or if file exists");
            }

            return prefabsDictionary;

        }
    }
}
