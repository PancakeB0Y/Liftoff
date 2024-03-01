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
        readonly Sprite _bgWire;
        readonly Sprite _box;
        readonly Sprite _goodBox;
        readonly Sprite _badBox;
        Pivot _container;

        Vector2 _wireXYPos1;
        Vector2 _wireXYPos2;

        Vector2 _goodStartPos;
        Vector2 _badStartPos;

        const float _distMult = 1.55f;

        const int boxLitTimer = 300;
        int curBoxLitTimer = 0;
        public Module_InfoCurrent_Visual(Module_InfoCurrent logic, TiledObject data)
        {
            _moduleLogic = logic;

            _bg = new Sprite(data.GetStringProperty("BgFilePath", "Assets/InfoCurrent/InfoCurrent_Background.png"), true, false);
            _wire = new Sprite(data.GetStringProperty("WireFilePath", "Assets/InfoCurrent/InfoCurrent_Slider.png"), true, false);
            _bgWire = new Sprite(data.GetStringProperty("BgWireFilePath", "Assets/InfoCurrent/InfoCurrent_BgWire.png"), true, false);
            _box = new Sprite(data.GetStringProperty("BoxFilePath", "Assets/InfoCurrent/InfoCurrent_Box.png"), true, false);

            _goodBox = new Sprite(data.GetStringProperty("BoxFilePath", "Assets/InfoCurrent/InfoCurrent_Box.png"), true, false);
            _goodBox.SetColor(0.5f, 1f, 0.5f);
            _badBox = new Sprite(data.GetStringProperty("BoxFilePath", "Assets/InfoCurrent/InfoCurrent_Box.png"), true, false);
            _badBox.SetColor(1f, 0.5f, 0.5f);

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

            _bg.SetScaleXY(0.9f);
            _bg.SetOrigin(43, -25);
            _container.AddChild(_bg);

            Vector2 boxXY = new Vector2(-5, 53);
            float boxScale = 0.4f;

            _box.SetXY(boxXY.X, boxXY.Y);
            _box.SetScaleXY(boxScale);
            _container.AddChild(_box);

            _goodBox.SetXY(boxXY.X, boxXY.Y);
            _goodBox.SetScaleXY(boxScale);
            _goodBox.alpha = 0;
            _container.AddChild(_goodBox);

            _badBox.SetXY(boxXY.X, boxXY.Y);
            _badBox.SetScaleXY(boxScale);
            _badBox.alpha = 0;
            _container.AddChild(_badBox);

            _bgWire.SetOrigin(_bgWire.width, 0);
            _bgWire.SetXY(230, 76);
            _bgWire.width = 160;
            _bgWire.height = 30;
            _container.AddChild(_bgWire);

            _wireXYPos1 = new Vector2(112, 86);
            _wireXYPos2 = new Vector2(112, 115);

            if (!_moduleLogic.IsOnWrongPath)
            {
                _wire.SetXY(_wireXYPos1.X, _wireXYPos1.Y);
            }
            else
            {
                _wire.SetXY(_wireXYPos2.X, _wireXYPos2.Y);
            }
            _wire.SetScaleXY(0.6f);
            _wire.width = 55;

            _goodStartPos = new Vector2(215, 77);

            _container.AddChild(_wire);

            foreach (Sprite sprite in _goodFiles)
            {
                sprite.alpha = 0;
                sprite.SetXY(_goodStartPos.X, _goodStartPos.Y);
                sprite.SetScaleXY(0.6f);
                _container.AddChild(sprite);
            }

            _badStartPos = new Vector2(215, 77);
            foreach (Sprite sprite in _badFiles)
            {
                sprite.alpha = 0;
                sprite.SetXY(_goodStartPos.X, _goodStartPos.Y);
                sprite.SetScaleXY(0.6f);
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
                        _goodFiles[i].x = _goodStartPos.X - _moduleLogic.GoodFiles[i].pos * _distMult;
                    }
                }
            }

            if (_badFiles.Count != 0)
            {
                for (int i = 0; i < _badFiles.Count; i++)
                {
               
                    if(i >= _moduleLogic.BadFiles.Length) { break; }
                    if (_moduleLogic.BadFiles[i].pos == -1)
                    {
                        _badFiles[i].alpha = 0;
                        continue;
                    }

                    if (_moduleLogic.BadFiles[i].isSpawned == true)
                    {
                        _badFiles[i].alpha = 1;
                        _badFiles[i].x = _badStartPos.X - _moduleLogic.BadFiles[i].pos * _distMult;
                    }
                }
            }
        }

        public void LightBox(bool isGood)
        {
            AddChild(new Coroutine(LightBoxCR(isGood)));
        }

        IEnumerator LightBoxCR(bool isGood)
        {
            while (curBoxLitTimer < boxLitTimer)
            {
                curBoxLitTimer += Time.deltaTime;
                if (isGood)
                {
                    _goodBox.alpha = 1;
                    _badBox.alpha = 0;
                }
                else
                {
                    _badBox.alpha = 1;
                    _goodBox.alpha = 0;
                }
                yield return null;
            }

            _badBox.alpha = 0;
            _goodBox.alpha = 0;
            curBoxLitTimer = 0;
        }

        void Update()
        {
            MoveFiles();
        }

        protected override void OnDestroy()
        {
            if (_container == null) { return; }
            _container.Destroy();
        }
    }
}
