using GXPEngine;
using GXPEngine.Core;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Dino : Module
    {
        private class Cactus : Sprite
        {
            public bool Passed { get; set; }

            public Cactus(string filename, bool keepInCache = false, bool addCollider = true) : base(filename, keepInCache, addCollider)
            {
            }

        }

        readonly string[] _cactiFilePaths;
        List<Cactus> _cacti = new List<Cactus>();
        int _cactusMinSpawnDistance;
        int _cactusMaxSpawnDistance;
        float _moveSpeed;

        //related to Dino
        AnimationSprite _dino;
        bool _dinoJumped;
        float _jumpPower;
        float _dinoVel;
        float _dinoGravity = 15;
        float _terminalVel;
        byte _dinoAnimDelay;

        Sprite _bg;

        Ground _ground;
        Ground[] _groundWrapper;

        int _currentScore;
        int _scorePenalty;
        int _scoreReward;

        int _winScore;
        TextMesh _scoreDisplay;
        
        Pivot _container;
        int _currentSpawnDistance;

        public Module_Dino(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            string dinoFilePath = data.GetStringProperty("DinoFilePath");
            int dinoSsCols = data.GetIntProperty("DinoSS_Cols");
            int dinoSsRows = data.GetIntProperty("DinoSS_Rows");
            string bgFilePath = data.GetStringProperty("BgFilePath");

            _cactiFilePaths = data.GetStringProperty("CactiFilePathsCSV").Split(',');
            _cactusMinSpawnDistance = data.GetIntProperty("MinSpawnDistance", 96);
            _cactusMaxSpawnDistance = data.GetIntProperty("MaxSpawnDistance", 126);
            _moveSpeed = data.GetFloatProperty("CactusMoveSpeed",1);
            _dinoAnimDelay = (byte)data.GetIntProperty("DinoAnimDelay", 255);

            _bg = new Sprite(bgFilePath, true, false);
            _dino = new AnimationSprite(dinoFilePath,dinoSsCols,dinoSsRows,-1,true);
            _ground = new Ground("Assets/square.png",true);

            //for dino to ignore everything else
            _groundWrapper = new Ground[1] { _ground };

            _jumpPower = data.GetFloatProperty("DinoJumpPower",2);
            _terminalVel = data.GetFloatProperty("DinoTerminalVel",10);

            _scorePenalty = data.GetIntProperty("ScorePenalty", 1);
            _scoreReward = data.GetIntProperty("ScoreReward", 10);
            _winScore = data.GetIntProperty("WinScore", 10);

            _scoreDisplay = new TextMesh("000", 100, 30);


            alpha = 0f;

            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;
            SetOrigin(0, 0);
            _container.SetXY(x, y);
            
            _bg.SetOrigin(_bg.width/2, _bg.height/2);
            _container.AddChild(_bg);
            _bg.SetXY(width/2,height/2);
            
            _container.AddChild(_ground);
            _container.AddChild(_dino);
            _container.AddChild(_scoreDisplay);

            _ground.y = height - 50;
            _ground.x = width / 2 - _bg.width / 2;
            _ground.width = _bg.width;
            _ground.height = 40;
            _ground.color = (uint)Color.Brown.ToArgb();
            _ground.alpha = 0.2f;

            _dino.color = (uint)Color.Green.ToArgb();
            _dino.x = _ground.y - _dino.height;

            _scoreDisplay.SetOrigin(0, 0);
            _scoreDisplay.HorizontalAlign = CenterMode.Min;
            _scoreDisplay.SetXY(0, 0);
            _scoreDisplay.TextColor = Color.Wheat;
        
        }
        int started = 0;
        void Update()
        {
            //need this delay so obstacles spawn wher ground is
            if (started++ <5)
                return;

            CactusSpawner();
            HandleCacti();
            HandleDino();
            
            if (_currentScore >= _winScore)
            {
                RaiseSuccesEvent();
            }
        }

        void CactusSpawner()
        {
            var randomCactusIndex = Utils.Random(0, _cactiFilePaths.Length);
            if ( CheckCanSpawnCactus() )
            {
                var newCactus = new Cactus(_cactiFilePaths[randomCactusIndex], true);
                _container.AddChild(newCactus);
                _cacti.Add(newCactus);
                newCactus.SetXY((width + _bg.width) / 2, _ground.y - newCactus.height);
                _currentSpawnDistance = Utils.Random(_cactusMinSpawnDistance, _cactusMaxSpawnDistance);
            }
        }

        bool CheckCanSpawnCactus()
        {
            if (_cacti.Count == 0) return true;

            return (width / 2 + _bg.width / 2) - _cacti[_cacti.Count - 1].x > _currentSpawnDistance;
        }

        void HandleCacti()
        {
            for (int i = 0; i < _cacti.Count; i++)
            {
                Cactus cactus = _cacti[i];

                Collision col = cactus.MoveUntilCollision(-_moveSpeed, 0);
                if (!cactus.Passed && cactus.x < _dino.x)
                {
                    cactus.Passed = true;
                    _currentScore += _scoreReward;
                    _scoreDisplay.Text = _currentScore.ToString();
                }

                if (cactus.x < (width - _bg.width) / 2)
                    cactus.LateDestroy();

                if (col == null) continue;
                cactus.LateDestroy();
                _cacti.Remove(cactus);
                i--;
                _currentScore -= _scorePenalty;
                _currentScore = Mathf.Max(0, _currentScore);
                _scoreDisplay.Text = _currentScore.ToString();

            }
        }

        void HandleDino()
        {
            Collision dinoColl = _dino.MoveUntilCollision(0, _dinoVel, _groundWrapper);
            bool isGrounded = dinoColl != null && dinoColl.other is Ground;
            //jump
            if (Input.GetKey(Key.D) && isGrounded && !_dinoJumped)
            {
                _dinoVel = -_jumpPower;
                _dinoJumped = true;
            }
            else if (!(isGrounded))
            {
                _dinoJumped = false;
            }

            if (isGrounded)
                _dino.SetCycle(0, 7, _dinoAnimDelay);
            else
                _dino.SetCycle(4, 1, _dinoAnimDelay);

            //gravity
            _dinoVel += _dinoGravity * Mathf.Min(Time.deltaTime * 0.001f, 0.04f);
            _dinoVel = Mathf.Min(_dinoVel, _terminalVel);

            _dino.AnimateFixed();
        }

        protected override void OnTimeEnd()
        {
            RaiseFailEvent();
        }

        protected override void OnDestroy()
        {
            _container.Destroy();
        }
        
    }

}
