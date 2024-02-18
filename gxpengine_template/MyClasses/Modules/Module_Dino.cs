using GXPEngine;
using GXPEngine.Core;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                width = 10;
                height = 10;
            }

        }

        readonly string[] _cactiFilePaths;
        List<Cactus> _cacti = new List<Cactus>();
        int _cactusSpawnDistance;
        float _moveSpeed;


        Sprite _dino;
        bool _dinoJumped;
        float _jumpPower;
        float _dinoVel;
        float _dinoGravity = 15;
        float _terminalVel;

        Ground _ground;
        Ground[] _groundWrapper;

        int _currentScore;
        int _scorePenalty;
        int _scoreReward;

        int _winScore;
        TextMesh _scoreDisplay;
        
        Pivot _container;

        public Module_Dino(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            string dinoFilePath = data.GetStringProperty("DinoFilePath");
            _cactiFilePaths = data.GetStringProperty("CactiFilePathsCSV").Split(',');
            _cactusSpawnDistance = data.GetIntProperty("SpawnDistance");
            _moveSpeed = data.GetFloatProperty("CactusMoveSpeed",1);
            
            _dino = new Sprite(dinoFilePath);
            _container.AddChild(_dino);
            

            _ground = new Ground(dinoFilePath);
            //for dino to ignore everything else
            _groundWrapper = new Ground[1] { _ground };
            _container.AddChild(_ground);

            _jumpPower = data.GetFloatProperty("DinoJumpPower",2);
            _terminalVel = data.GetFloatProperty("DinoTerminalVel",10);

            _scorePenalty = data.GetIntProperty("ScorePenalty", 1);
            _scoreReward = data.GetIntProperty("ScoreReward", 10);
            _winScore = data.GetIntProperty("WinScore", 10);

            _scoreDisplay = new TextMesh("000", 100, 30);
            _container.AddChild(_scoreDisplay);
            
            alpha = 0.1f;

            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;
            _container.SetXY(x, y);

            _dino.color = (uint)Color.Green.ToArgb();
            _dino.SetScaleXY(0.1f);

            _ground.y = 10;
            _ground.x = -width/2;
            _ground.width *= 2;
            _ground.height = 32;
            _ground.color = (uint)Color.Brown.ToArgb();

            _scoreDisplay.SetOrigin(0, 0);
            _scoreDisplay.HorizontalAlign = CenterMode.Min;
            _scoreDisplay.SetXY(-width / 2, -height / 2);
            _scoreDisplay.TextColor = Color.Wheat;
        }

        void Update()
        {
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
            if (CheckCanSpawnCactus())
            {
                var newCactus = new Cactus(_cactiFilePaths[randomCactusIndex], true);
                _container.AddChild(newCactus);
                _cacti.Add(newCactus);
                newCactus.SetXY(width / 2, 0);
            }
        }

        bool CheckCanSpawnCactus()
        {
            if (_cacti.Count == 0) return true;

            return (width / 2) - _cacti[_cacti.Count - 1].x > _cactusSpawnDistance;
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

                if (cactus.x < -width / 2)
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
            //jump
            if (Input.GetKey(Key.D) && dinoColl?.other is Ground && !_dinoJumped)
            {
                _dinoVel = -_jumpPower;
                _dinoJumped = true;
            }
            else if (!(dinoColl?.other is Ground))
            {
                _dinoJumped = false;
            }

            //gravity
            _dinoVel += _dinoGravity * Mathf.Min(Time.deltaTime * 0.001f, 0.04f);
            _dinoVel = Mathf.Min(_dinoVel, _terminalVel);
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
