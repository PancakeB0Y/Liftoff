using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.Modules;
using System;
using System.Collections;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Bomb : AnimationSprite
    {
        public event Action Exploded;
        public static Bomb Instance;

        public int Strikes { get; set; }

        readonly Pivot _container;
        readonly int _failsAmount;
        readonly Sprite[] _failVisuals;
        ModuleManager _moduleManager;

        readonly Sound _explosionSound = new Sound("Assets/Sounds/Explosion.wav");

        int _failsLeft;
        readonly float _cooldown;
        float _currCooldown;
        public Bomb(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, -1, false, false)
        {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;

            _failsAmount = data.GetIntProperty("FailsAmount", 5);

            _cooldown = data.GetFloatProperty("WallTouchCooldown", 2);
            _currCooldown = _cooldown;

            _container = new Pivot();
            _failsLeft = _failsAmount;
            _failVisuals = new Sprite[_failsAmount];

            //alpha = 0.2f;
            AddChild(new Coroutine(Init(data)));
        }

        IEnumerator Init(TiledObject data)
        {
            yield return null;

            string failPath = data.GetStringProperty("FailBGFilePath", "Assets/Bomb_Cross.png");
            int padding = data.GetIntProperty("PaddingX", 10);
            int spacing = data.GetIntProperty("Spacing", 10);

            _moduleManager = MyUtils.MyGame.FindObjectOfType<ModuleManager>();
            _moduleManager.ModuleFailed += OnFail;

            MyUtils.MyGame.CurrentScene.AddChild(_container);

            _container.SetXY(x, y);

            int ballW = (width - padding - (spacing * (_failVisuals.Length - 1))) / _failVisuals.Length;
            for (int i = 0; i < _failVisuals.Length; i++)
            {
                var ball = new Sprite(failPath, true, false);
                _container.AddChild(ball);
                ball.width = ballW;
                ball.height = ballW;

                _failVisuals[i] = ball;

                ball.x = ballW * (i % _failVisuals.Length) + (spacing * (i % _failVisuals.Length)) + padding * 0.5f + 4f;
                ball.y = height / 2 - ball.height / 2 + 3f;

                ball.SetOrigin(ball.width / 2, ball.height / 2);
            }

            _container.SetXY(x - width / 2, y - height / 2);
        }

        void Update()
        {
            _currCooldown = Mathf.Max(_currCooldown -= Time.deltaTime * 0.001f, 0);

            if (Input.GetKey(Key.NINE) && _currCooldown == 0)//touching the walls
            {
                _currCooldown = _cooldown;

                RemoveLife();
            }

            if (_failsLeft == 0)
            {
                Exploded?.Invoke();
                _explosionSound.Play(false, 0, 0.2f);
                SpawnExplosion();
                SaveManager.Instance.SaveHighScore(_moduleManager.Score);

                Destroy();
            }
        }

        void OnFail()
        {
            RemoveLife();
        }

        void SpawnExplosion()
        {
            var explosion = MyUtils.MyGame.Prefabs["Explosion"].Clone();
            MyUtils.MyGame.CurrentScene.AddChild(explosion);
            explosion.SetXY(x, y);
        }

        void RemoveLife()
        {
            if (_failsLeft > 0)
            {
                _failsLeft--;
                int scaleSize = (int)(_failVisuals[_failsLeft].scale + 0.5f);

                var easeFunc = EaseFuncs.Factory("EaseInOutExpo");
                const int moveSpeedMillis = 300;
                var copy = _failsLeft;
                _failVisuals[_failsLeft].AddChild(new Tween(TweenProperty.scale, moveSpeedMillis, scaleSize, easeFunc).
                    OnExit
                    (
                        () =>
                        {
                            _failVisuals[copy].visible = false;
                        }

                        )

                    );
            }
        }


        protected override void OnDestroy()
        {
            //_moduleManager.ModuleFailed -= OnFail;
            Exploded = null;
            Instance = null;
        }
    }
}
