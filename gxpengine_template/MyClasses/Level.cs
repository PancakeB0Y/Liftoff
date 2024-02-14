using System;
using GXPEngine;
using gxpengine_template.MyClasses;
using TiledMapParser;

namespace gxpengine_template
{
    public class Level : GameObject
    {

        public event Action<Level> LevelStarted;
        public event Action BackgroundLoaded;
        public event Action UILoaded;
        public string Name { get; }

        public Level(string fileName)
        {
            Name = fileName;

        }
        
        //not in constructor because level has to be parent of game first
        public void Init()
        {
            var loader = new TiledLoader(Name, MyGame.main, addColliders: false, autoInstance: true);

            int index;
            SceneConfigs sceneConfigs = null;

            //sceneConfigs
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "SceneConfig", out index))
            {
                loader.rootObject = MyGame.main;
                loader.addColliders = false;
                loader.LoadObjectGroups(index);
                sceneConfigs = game.FindObjectOfType<SceneConfigs>();
                sceneConfigs?.Init(this);
            }

            //background
            if (loader.map.ImageLayers != null)
            {
                loader.rootObject = this;
                loader.LoadImageLayers();

                BackgroundLoaded?.Invoke();
            }

            //managers
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "Managers", out index))
            {
                loader.rootObject = MyGame.main;
                loader.addColliders = false;
                loader.LoadObjectGroups(index);
            }

            //level objects
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "Object Layer 1", out index))
            {
                loader.rootObject = this;
                loader.addColliders = true;
                loader.LoadObjectGroups(index);
            }

            //ui
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "UI", out index))
            {
                loader.addColliders = false;
                loader.rootObject = game;
                loader.LoadObjectGroups(index);
                UILoaded?.Invoke();
            }

            //worldText
            if (loader.map.ObjectGroups.TryGetIndex(x => x.Name == "WorldText", out index))
            {
                loader.rootObject = this;
                loader.addColliders = false;
                loader.LoadObjectGroups(index);
            }


            //it's useless after Scene Load so I destroy it
            sceneConfigs?.LateDestroy();

            foreach (var startable in MyGame.main.FindInterfaces<IStartable>())
            {
                startable.Start();
            }
            LevelStarted?.Invoke(this);
        }    

    }
}
