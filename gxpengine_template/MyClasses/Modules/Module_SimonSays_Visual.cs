using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_SimonSays_Visual : GameObject
    {
        private class SimonBall : Sprite
        {
            public bool IsDoneMoving { get; set; } = true;
            public TextMesh TextMesh { get; }

            public SimonBall(string filename, bool keepInCache = false, bool addCollider = true) : base(filename, keepInCache, addCollider)
            {
                TextMesh = new TextMesh("0",width,height);
                TextMesh.SetXY(width / 2, height / 2);

                AddChild(TextMesh);
            }
            
        }

        readonly Module_SimonSays _moduleLogic;
        readonly Module_SimonSays_Selector _moduleSelector;

        readonly SimonBall[] _simonBalls;
        readonly int _swapSpeedMillis;
        readonly string _easeFuncName;
        readonly Pivot _container;
        readonly int _textSize;

        public Module_SimonSays_Visual(Module_SimonSays logic, Module_SimonSays_Selector selector , TiledObject data)
        {
            string simonBallPath = data.GetStringProperty("SimonBallPath", "Assets/square.png");
            _swapSpeedMillis = data.GetIntProperty("SwapSpeedMilllis", 400);
            _easeFuncName = data.GetStringProperty("EaseFuncName", "EaseOutBack");
            _textSize = data.GetIntProperty("TextSize", 20);

            _moduleLogic = logic;
            _moduleSelector = selector;

            selector.Selected += OnBallSelect;
            selector.Deselected += OnBallDeselect;
            logic.OrderChanged += ChangeBalls;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            _simonBalls = new SimonBall[_moduleLogic.RandomNumers.Count];


            AddChild(new Coroutine(Init(simonBallPath)));
        }

        IEnumerator Init(string simonBallPath)
        {
            yield return null;
            _container.SetXY(_moduleLogic.x, _moduleLogic.y);

            for (int i = 0; i < _moduleLogic.RandomNumers.Count; i++)
            {
                var ball = new SimonBall(simonBallPath, true, false);
                _container.AddChild(ball);
                ball.width = _moduleLogic.width / _simonBalls.Length;
                ball.height = _moduleLogic.height;
                ball.TextMesh.Text = _moduleLogic.RandomNumers[i].ToString();
                ball.TextMesh.TextSize = _textSize;
                _simonBalls[i] = ball;

                ball.x = i * (ball.width + 10);

            }
        }
        private void OnBallDeselect(int index)
        {
            _simonBalls[index].SetColor(1, 1, 1);
        }

        private void OnBallSelect(int index)
        {
            _simonBalls[index].SetColor(0, .5f, 0);
        }
        
        protected override void OnDestroy()
        {
            _container.Destroy();
            _moduleLogic.OrderChanged -= ChangeBalls;
            _moduleSelector.Selected -= OnBallSelect;
            _moduleSelector.Deselected -= OnBallDeselect;
        }

        void Update()
        {
            _moduleSelector.CanUse = _simonBalls.All(b=> b != null && b.IsDoneMoving);
        }

        void ChangeBalls(int from, int to)
        {
            var fromBall = _simonBalls[from];
            var toBall = _simonBalls[to];
            var distanceBetweenBalls = (int)(fromBall.x - toBall.x);
            
            fromBall.IsDoneMoving = false;
            toBall.IsDoneMoving = false;

            var easeFunc = EaseFuncs.Factory(_easeFuncName);
            fromBall.AddChild(new Tween(TweenProperty.x, _swapSpeedMillis, -distanceBetweenBalls, easeFunc).
                OnExit
                (
                    () => fromBall.IsDoneMoving = true)
                );

            toBall.AddChild(new Tween(TweenProperty.x, _swapSpeedMillis, distanceBetweenBalls, easeFunc).
                OnStart(
                () =>
                {
                    toBall.SetColor(0, .5f, 0);
                    fromBall.SetColor(0, .5f, 0);
                }).
                OnExit( 
                ()=> 
                {
                    toBall.SetColor(1, 1, 1);
                    fromBall.SetColor(1, 1, 1);

                    toBall.IsDoneMoving = true; 
                    _moduleLogic.CheckSucces(); 
                }));

            (_simonBalls[from], _simonBalls[to]) = (_simonBalls[to], _simonBalls[from]);

        }


    }
}
