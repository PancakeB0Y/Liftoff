using GXPEngine;
using gxpengine_template.MyClasses.Modules;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class DifficultyManager : Sprite, IStartable
    {
        readonly struct DifficultyThreshold
        {
            public readonly int Score;
            public readonly string Symbol;

            public DifficultyThreshold(int score, string symbol)
            {
                Score = score;
                Symbol = symbol;
            }
        }
        public static DifficultyManager Instance { get; private set; }
        public int Difficulty { get; set; } = 0;
        readonly float[] _difficultyMultipliers;
        TextMesh _textMesh;
        ModuleManager _moduleManager;
        DifficultyThreshold[] _scoreThreshHolds;

        public DifficultyManager(TiledObject data) : base("Assets/square.png", true, false)
        {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;

            alpha = 0;
            _difficultyMultipliers = data.GetStringProperty("DifficultyMultipliersCSV", "1.0,1.1,1.2,1.3,1.4").Split(',').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            
            _scoreThreshHolds = data.GetStringProperty("ScoreThreshHoldsCSV", "F.10,E.20").Split(',').Select
            (
                s =>
                {
                    string[] pair = s.Split('.');
                    return new DifficultyThreshold(int.Parse(pair[1]), pair[0]);
                }
            
            ).ToArray();

            _textMesh = new TextMesh("0", 200, 200, MyUtils.MainColor, Color.Transparent, CenterMode.Center,CenterMode.Center, textSize: 30, fontFileName: "Assets/Courier New Bold.ttf", fontStyle: FontStyle.Bold);
        }

        public void Start()
        {
            _moduleManager = MyUtils.MyGame.FindObjectOfType<ModuleManager>();
            MyUtils.MyGame.CurrentScene.AddChild(_textMesh);
            _textMesh.SetXY(x, y);
            _moduleManager.ScoreUpdate += OnScoreUpdate;
        }

        private void OnScoreUpdate()
        {
            Console.WriteLine("score update");
            string letter = "E";
            foreach (var threshHold in _scoreThreshHolds)
            {
                if (_moduleManager.Score < threshHold.Score)
                    break;
                letter = threshHold.Symbol;
            }

           _textMesh.Text = letter;

        }

        public int GetMultipliedScore(int score)
        {
            return (int)(score * _difficultyMultipliers[Difficulty]);
        }

        protected override void OnDestroy()
        {
            Instance = null;
        }
    }
}
