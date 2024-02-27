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

            string _greenFilename;
            bool _isGreen = false;
            public Dial_Visual(string moverFilename, string redFilename, string greenFilename, bool keepInCache = false, bool addCollider = false) : base(redFilename, keepInCache, addCollider)
            {
                _greenFilename = greenFilename;

                mover = new Sprite(moverFilename, false, false);
                mover.SetXY(width / 2, height / 2);
                mover.SetOrigin(mover.width / 2, mover.height / 2);
                AddChild(mover);
            }

            void TurnGreen()
            {
                Sprite greenPart = new Sprite(_greenFilename, false, false);
                AddChild(greenPart);
                _isGreen = true;
            }

            public void Rotate(Dial dial)
            {
                if (_isGreen) { return; }

                if (dial.IsComplete)
                {
                    TurnGreen();
                }

                if (dial.RotateRight)
                {
                    mover.rotation = (dial.CurrentPercent + 1) * 3.6f - (dial.WinRange / 2 * 3.6f);
                }
                else
                {
                    mover.rotation = -((dial.CurrentPercent + 1) * 3.6f - (dial.WinRange / 2 * 3.6f));
                }
            }
        }

        readonly List<Dial_Visual> _visuals;
        readonly Module_Dials _moduleLogic;

        readonly Pivot _container;

        public Module_Dials_Visual(Module_Dials logic, TiledObject data)
        {
            string dialVisualPath = data.GetStringProperty("DialVisualPath", "Assets/Dials/Dial_Mover.png");
            string dialRedPath = data.GetStringProperty("DialRedPath", "Assets/Dials/Dial_Red.png");
            string dialGreenPath = data.GetStringProperty("DialGreenPath", "Assets/Dials/Dial_Green.png");
            _visuals = new List<Dial_Visual>();

            _moduleLogic = logic;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            AddChild(new Coroutine(Init(dialVisualPath, dialRedPath, dialGreenPath)));
        }

        IEnumerator Init(string dialVisualPath, string dialRedPath, string dialGreenPath)
        {
            yield return null;

            Vector2 pos = new Vector2(_moduleLogic.x, _moduleLogic.y);

            _moduleLogic.SetOrigin(0, 0);
            _moduleLogic.SetXY(pos.x, pos.y);
            _moduleLogic.alpha = 1f;
            _container.SetXY(_moduleLogic.x, _moduleLogic.y);

            int spacing = 30;
            int dialW = (_moduleLogic.width - (spacing * (2))) / 3;
            for (int i = 0; i < 3; i++)
            {
                Dial_Visual dial = new Dial_Visual(dialVisualPath, dialRedPath, dialGreenPath);
                _container.AddChild(dial);

                dial.width = dialW;
                dial.height = dialW;
                _visuals.Add(dial);

                dial.x = dialW * (i % 3) + (spacing * (i % 3));
                dial.y = _moduleLogic.height / 2 - dial.height / 2;
            }
        }

        void Update()
        {
            for (int i = 0; i < _visuals.Count; i++)
            {
                _visuals[i].Rotate(_moduleLogic.Dials[i]);
            }
        }

        protected override void OnDestroy()
        {
            _container.Destroy();
        }
    }
}
