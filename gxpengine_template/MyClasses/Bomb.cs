using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.Modules;
using System.Collections;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Bomb : AnimationSprite
    {
        public static Bomb Instance;
        public int Strikes { get; set; }

        readonly Pivot _container;
        readonly int _failsAmount;
        readonly Sprite[] _failVisuals;
        ModuleManager _moduleManager;

        public Bomb(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, -1, false, false)
        {
            string simonBallPath = data.GetStringProperty("FailBGFilePath", "Assets/square.png");
            _failsAmount = data.GetIntProperty("FailsAmount", 5);
            _container = new Pivot();

            _failVisuals = new Sprite[_failsAmount];

            AddChild(new Coroutine(Init(simonBallPath)));
        }

        IEnumerator Init(string failPath)
        {
            yield return null;
            _moduleManager = MyUtils.MyGame.CurrentScene.FindObjectOfType<ModuleManager>();
            _moduleManager.ModuleFailed += OnFail;
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            _container.SetXY(x, y);
            int spacing = 30;

            int ballW = (width - (spacing * (_failVisuals.Length - 1))) / _failVisuals.Length;
            for (int i = 0; i < _failVisuals.Length; i++)
            {
                var ball = new Sprite(failPath, true, false);
                _container.AddChild(ball);
                ball.width = ballW;
                ball.height = ballW;

                _failVisuals[i] = ball;

                ball.x = ballW * (i % _failVisuals.Length) + (spacing * (i % _failVisuals.Length));
                ball.y = height / 2 - ball.height / 2;

            }
        }

        void OnFail()
        {
            _failVisuals[0].visible = false;
        }

        protected override void OnDestroy()
        {
            _moduleManager.ModuleFailed -= OnFail;
            Instance = null;
        }
    }
}
