using GXPEngine;
using GXPEngine.Core;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Waves_Visual : GameObject
    {

        readonly Module_Waves _moduleLogic;
        readonly Sprite _visual;
        readonly Sprite _foreground;
        Sprite _moverVisual;

        Pivot _container;

        float initW;
        float initH;

        public Module_Waves_Visual(Module_Waves logic, TiledObject data)
        {
            string wavesVisualPath = data.GetStringProperty("WavesVisualPath", "Assets/Waves/Sine_Wave.png");
            string moverVisualPath = data.GetStringProperty("MoverVisualPath", "Assets/Waves/Sine_Mover.png");
            string backgroundPath = data.GetStringProperty("BackgroundPath", "Assets/Waves/Waves_Background.png");

            _moduleLogic = logic;

            _visual = new Sprite(wavesVisualPath);
            _moverVisual = new Sprite(moverVisualPath);
            _foreground = new Sprite(backgroundPath);
            initW = _moverVisual.width;
            initH = _moverVisual.height;

            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            Vector2 pos = new Vector2(_moduleLogic.x, _moduleLogic.y);
            _moduleLogic.SetOrigin(0, 0);
            _moduleLogic.SetXY(pos.x, pos.y);
            _moduleLogic.alpha = 1f;
            _container.SetXY(_moduleLogic.x, _moduleLogic.y);

            _visual.SetXY(92, 16);
            _moverVisual.SetXY(92 + _moverVisual.width / 2, 16 + _moverVisual.height / 2);
            _moverVisual.SetColor(1, 1, 0);
            _moverVisual.SetOrigin(_moverVisual.width / 2, _moverVisual.height / 2);

            _container.AddChild(_visual);
            _container.AddChild(_moverVisual);
            _container.AddChild(_foreground);
        }

        public void SetWH(float w, float h)
        {
            _moverVisual.width = (int)(_moverVisual.width * w);
            _moverVisual.height = (int)(_moverVisual.height * h);
        }

        public void Stretch(float stretchW, float stretchH)
        {
            float newW = initW * stretchW;
            float newH = initH * stretchH;

            _moverVisual.width = (int)(_moverVisual.width + newW);
            _moverVisual.height = (int)(_moverVisual.height + newH);
        }

        public bool IsComplete()
        {
            bool hasWon = false;

            float diffW = 10f;
            float diffH = 10f;

            if (_moverVisual.width <= initW + diffW && _moverVisual.width >= initW - diffW && _moverVisual.height <= initH + diffH && _moverVisual.height >= initH - diffH)
            {
                hasWon = true;
            }

            return hasWon;
        }


        protected override void OnDestroy()
        {
            _container.Destroy();
        }
    }
}
