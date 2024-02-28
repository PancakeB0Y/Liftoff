using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_InfoCurrent_Visual : GameObject
    {
        public bool IsWireDoneMoving = true;

        readonly Module_InfoCurrent _moduleLogic;

        readonly Sprite _bg;
        readonly Sprite _wire;

        readonly List<Sprite> _goodFiles;
        readonly List<Sprite> _badFiles;
        readonly AnimationSprite _box;
        Pivot _container;

        Vector2 _wireXYPos1;
        Vector2 _wireXYPos2;

        Vector2 _goodStartPos;
        Vector2 _badStartPos;

        public Module_InfoCurrent_Visual(Module_InfoCurrent logic, TiledObject data)
        {
            _moduleLogic = logic;

            _bg = new Sprite(data.GetStringProperty("BgFilePath", "Assets/InfoCurrent/InfoCurrent_Background.png"), true, false);
            _wire = new Sprite(data.GetStringProperty("WireFilePath", "Assets/InfoCurrent/InfoCurrent_Wire.png"), true, false);

            string goodFilePath = data.GetStringProperty("GoodFilePath", "Assets/InfoCurrent/InfoCurrent_Good.png");
            string badFilePath = data.GetStringProperty("BadFilePath", "Assets/InfoCurrent/InfoCurrent_Bad.png");

            _goodFiles = new List<Sprite>();

            for (int i = 0; i < _moduleLogic.GoodFiles.Length; i++)
            {
                _goodFiles.Add(new Sprite(goodFilePath, true, false));
            }

            _badFiles = new List<Sprite>();
            for (int i = 0; i < _moduleLogic.GoodFiles.Length; i++)
            {
                _badFiles.Add(new Sprite(badFilePath, true, false));
            }

            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);


            _wireXYPos1 = new Vector2(90, 79);
            _wireXYPos2 = new Vector2(90, 100);

            if (!_moduleLogic.IsOnWrongPath)
            {
                _wire.SetXY(_wireXYPos1.X, _wireXYPos1.Y);
            }
            else
            {
                _wire.SetXY(_wireXYPos2.X, _wireXYPos2.Y);
            }

            _goodStartPos = new Vector2(190, 78);

            _container.AddChild(_bg);
            _container.AddChild(_wire);

            foreach (Sprite sprite in _goodFiles)
            {
                sprite.alpha = 0;
                sprite.SetXY(_goodStartPos.X, _goodStartPos.Y);
                _container.AddChild(sprite);
            }

            _badStartPos = new Vector2(190, 99);
            foreach (Sprite sprite in _badFiles)
            {
                sprite.alpha = 0;
                sprite.SetXY(_badStartPos.X, _badStartPos.Y);
                _container.AddChild(sprite);
            }

            _container.SetXY(_moduleLogic.x, _moduleLogic.y);
        }

        public void MoveWire()
        {
            int travelDist = (int)(_wireXYPos1.Y - _wireXYPos2.Y);

            var easeFunc = EaseFuncs.Factory("EaseInOutExpo");
            const int moveSpeedMillis = 400;

            IsWireDoneMoving = false;

            if (_moduleLogic.IsOnWrongPath)
            {
                _wire.AddChild(new Tween(TweenProperty.y, moveSpeedMillis, -travelDist, easeFunc).
                    OnExit
                    (
                        () => IsWireDoneMoving = true)
                    );
            }
            else
            {
                _wire.AddChild(new Tween(TweenProperty.y, moveSpeedMillis, travelDist, easeFunc).
                    OnExit
                    (
                        () => IsWireDoneMoving = true)
                    );
            }

        }

        void MoveFiles()
        {
            if (_goodFiles.Count != 0)
            {
                for (int i = 0; i < _goodFiles.Count; i++)
                {
                    if (_moduleLogic.GoodFiles[i].pos == -1)
                    {
                        _goodFiles[i].alpha = 0;
                        continue;
                    }

                    if (_moduleLogic.GoodFiles[i].isSpawned == true)
                    {
                        _goodFiles[i].alpha = 1;
                        _goodFiles[i].x = _goodStartPos.X - _moduleLogic.GoodFiles[i].pos * 1.25f;
                    }
                }
            }

            if (_badFiles.Count != 0)
            {
                for (int i = 0; i < _badFiles.Count; i++)
                {
                    if (_moduleLogic.BadFiles[i].pos == -1)
                    {
                        _badFiles[i].alpha = 0;
                        continue;
                    }

                    if (_moduleLogic.BadFiles[i].isSpawned == true)
                    {
                        _badFiles[i].alpha = 1;
                        _badFiles[i].x = _badStartPos.X - _moduleLogic.BadFiles[i].pos * 1.23f;
                    }
                }
            }
        }

        void Update()
        {
            MoveFiles();
        }

        protected override void OnDestroy()
        {
            _container.Destroy();
        }
    }
}
