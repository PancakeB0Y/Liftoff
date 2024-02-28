using GXPEngine;
using GXPEngine.Core;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Dials_Visual : GameObject
    {
        private class Dial_Visual : Sprite
        {
            public Sprite mover;

            readonly Dial _dial;

            string _greenFilename;
            bool _isGreen = false;
            public Dial_Visual(string moverFilename, string redFilename, string greenFilename, Dial dial, bool keepInCache = false, bool addCollider = false) : base(redFilename, keepInCache, addCollider)
            {
                _dial = dial;
                _greenFilename = greenFilename;

                mover = new Sprite(moverFilename, false, false);
                mover.SetXY(width / 2, height / 2);
                mover.SetOrigin(mover.width / 2, mover.height / 2);
                AddChild(mover);
            }

            void TurnGreen()
            {
                RemoveChild(mover);
                Sprite greenPart = new Sprite(_greenFilename, false, false);
                AddChild(greenPart);
                AddChild(mover);
                _isGreen = true;
            }

            public void Rotate()
            {
                if (_isGreen) { return; }

                if (_dial.IsComplete)
                {
                    TurnGreen();
                }

                if (_dial.RotateRight)
                {
                    mover.rotation = ((_dial.CurrentPercent + 1) * 3.6f - (_dial.WinRange / 2 * 3.6f));
                }
                else
                {
                    mover.rotation = -((_dial.CurrentPercent + 1) * 3.6f - (_dial.WinRange / 2 * 3.6f));
                }
            }
        }

        readonly List<Dial_Visual> _visuals;
        readonly Module_Dials _moduleLogic;

        Pivot _container;

        public Module_Dials_Visual(Module_Dials logic, TiledObject data)
        {
            string dialVisualPath = data.GetStringProperty("DialVisualPath", "Assets/Dials/Dial_Mover.png");
            string dialRedPath = data.GetStringProperty("DialRedPath", "Assets/Dials/Dial_Red.png");
            string dialGreenPath = data.GetStringProperty("DialGreenPath", "Assets/Dials/Dial_Green.png");
            _visuals = new List<Dial_Visual>();

            _moduleLogic = logic;



            AddChild(new Coroutine(Init(dialVisualPath, dialRedPath, dialGreenPath)));
        }

        IEnumerator Init(string dialVisualPath, string dialRedPath, string dialGreenPath)
        {
            yield return null;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            Vector2 pos = new Vector2(_moduleLogic.x, _moduleLogic.y);
            _moduleLogic.SetOrigin(0, 0);
            _moduleLogic.SetXY(pos.x, pos.y);
            _moduleLogic.alpha = 1f;
            _container.SetXY(_moduleLogic.x, _moduleLogic.y);

            int spacing = 30;
            int dialW = (_moduleLogic.width - (spacing * (2))) / 3;
            for (int i = 0; i < 3; i++)
            {
                Dial_Visual dial = new Dial_Visual(dialVisualPath, dialRedPath, dialGreenPath, _moduleLogic.Dials[i]);

                dial.width = dialW;
                dial.height = dialW;
                _visuals.Add(dial);

                dial.x = dialW * (i % 3) + (spacing * (i % 3));
                dial.y = _moduleLogic.height / 2 - dial.height / 2;
                _container.AddChild(dial);
            }
        }

        void Update()
        {
            for (int i = 0; i < _visuals.Count; i++)
            {
                _visuals[i].Rotate();
            }
        }

        protected override void OnDestroy()
        {
            if (_container != null)
            {
                _container.Destroy();
            }
        }
    }
}
