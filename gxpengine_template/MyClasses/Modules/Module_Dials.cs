<<<<<<< HEAD:gxpengine_template/MyClasses/Modules/Module_Dials.cs
﻿using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Dials : Module
    {
        readonly List<Dial> dials;
        readonly List<Dial_Visual> visuals;

        public Module_Dials(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {

            (int minValue, int maxValue) winRange = (data.GetIntProperty("MinWinRange", 40), data.GetIntProperty("MaxWinRange", 60));
            dials = new List<Dial>
            {
                new Dial(data.GetFloatProperty("Speed1", 0.5f), 65, winRange), //65 = A
                new Dial(data.GetFloatProperty("Speed2", 0.5f), 83, winRange), //83 = S
                new Dial(data.GetFloatProperty("Speed3", 0.5f), 68, winRange) //68 = D
            };

            visuals = new List<Dial_Visual>();
            for (int i = 0; i < dials.Count; i++)
            {
                Dial_Visual curVisual = new Dial_Visual(dials[i], i * 30);
                visuals.Add(curVisual);
                AddChild(curVisual);
            }

        }

        void UpdateDials()
        {
            foreach (Dial dial in dials)
            {
                dial.Move();
                dial.ReadInputs();
            }
        }

        bool CheckIfComplete()
        {
            bool isWon = true;
            foreach (Dial dial in dials)
            {
                if (dial.isComplete == false)
                {
                    isWon = false;
                    break;
                }
            }

            Console.WriteLine(isWon);
            return isWon;
        }

        void Update()
        {
            UpdateDials();
        }

        protected override void OnTimeEnd()
        {
            if (CheckIfComplete())
                RaiseSuccesEvent();
            else
                RaiseFailEvent();
        }
    }

    internal class Dial
    {
        public float percent;
        readonly float speed;
        public bool isComplete;
        public readonly (int minValue, int maxValue) winRange;
        readonly int button;

        public Dial(float speed, int button, (int minValue, int maxValue) winRange)
        {
            this.speed = speed;
            this.button = button;
            this.winRange = winRange;
            percent = 0;
        }

        public void Move()
        {
            if (isComplete) { return; }
            percent += speed;
            if (percent > 99)
            {
                percent = 0;
            }
        }

        public void ReadInputs()
        {
            if (Input.GetKeyDown(button))
            {
                if (percent >= winRange.minValue && percent <= winRange.maxValue)
                {
                    isComplete = true;
                }
            }
        }
    }

    internal class Dial_Visual : GameObject
    {
        EasyDraw bar;
        EasyDraw bg;
        EasyDraw winRangeBar;

        Dial dial;
        public Dial_Visual(Dial dial, int y)
        {
            this.dial = dial;
            var w = 50;
            var h = 50;
            this.y = y;

            bg = new EasyDraw(w, h, false);
            bg.Clear(Color.Red);

            bar = new EasyDraw(w, 10, false);
            bar.SetXY(0, 0);
            bar.NoStroke();


            float winRangeWidth = Mathf.Ceiling((dial.winRange.maxValue - dial.winRange.minValue) / 100f * w);
            winRangeBar = new EasyDraw((int)winRangeWidth, 10, false);
            winRangeBar.SetXY(dial.winRange.minValue / 100f * w, 0);
            winRangeBar.Clear(Color.Yellow);

            AddChild(bg);
            AddChild(bar);
            AddChild(winRangeBar);
        }

        void Update()
        {
            bar.Clear(Color.White);
            bar.Fill(Color.Blue);

            float moverX = bar.width * (dial.percent - 0) / (99);
            bar.Rect(moverX, 2.5f, 5, 19);

            /*           bar.Fill(Color.Yellow);
                       bar.Rect(bar.width - 5, 2.5f, 5, 10);*/
        }
    }
}
=======
﻿using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Dials : Module
    {
        readonly List<Dial> _dials;
        readonly List<Dial_Visual> _visuals;

        public int DialWidth { get; private set; }
        public int DialHeight { get; private set; }

        string dialFilename;
        string moverFilename;
        public Module_Dials(TiledObject data) : base(data)
        {
            modulePos = modulePosition.Left;

            int winRange = data.GetIntProperty("WinRange", 10);
            dialFilename = data.GetStringProperty("dialFilename", "Assets/dial.png");
            moverFilename = data.GetStringProperty("moverFilename", "Assets/square.png");
            _dials = new List<Dial>
            {
                new Dial(data.GetFloatProperty("Speed1", 0.5f), 65, winRange), //65 = A
                new Dial(data.GetFloatProperty("Speed2", 0.5f), 83, winRange), //83 = S
                new Dial(data.GetFloatProperty("Speed3", 0.5f), 68, winRange) //68 = D
            };

            DialWidth = data.GetIntProperty("DialWidth", 150);
            DialHeight = data.GetIntProperty("DialHeight", 150);

            _visuals = new List<Dial_Visual>();
            for (int i = 0; i < _dials.Count; i++)
            {
                Dial_Visual curVisual = new Dial_Visual(dialFilename, moverFilename, _dials[i], i * (int)(DialWidth * 1.3f) + (DialWidth / 2), DialWidth, DialHeight);
                _visuals.Add(curVisual);
                AddChild(curVisual);
            }
        }

        void UpdateDials()
        {
            foreach (Dial dial in _dials)
            {
                dial.Move();
                dial.ReadInputs();
            }
        }
        bool CheckIfComplete()
        {
            bool isWon = true;
            foreach (Dial dial in _dials)
            {
                if (dial.IsComplete == false)
                {
                    isWon = false;
                    break;
                }
            }

            return isWon;
        }

        void Update()
        {
            UpdateDials();
        }

        protected override void OnTimeEnd()
        {
            if (CheckIfComplete())
                RaiseSuccesEvent();
            else
                RaiseFailEvent();
        }
    }

    internal class Dial
    {
        public float CurrentPercent { get; private set; }
        readonly float speed;
        public bool IsComplete { get; private set; }
        readonly int keyCode;

        public int WinRange { get; private set; }
        public int MinWinRange { get; private set; }
        public int MaxWinRange { get; private set; }

        public bool RotateRight { get; private set; }

        public Dial(float speed, int keyCode, int winRange)
        {
            this.speed = speed;
            this.keyCode = keyCode;
            WinRange = winRange;
            //MinWinRange = Utils.Random(0, 100 - WinRange);
            MinWinRange = 0;
            MaxWinRange = MinWinRange + winRange;
            CurrentPercent = 0;

            RotateRight = Utils.Random(0, 2) == 0 ? false : true;
            //rotateRight = true;
        }

        public void Move()
        {
            if (IsComplete) { return; }

            CurrentPercent += speed;

            if (CurrentPercent >= 100)
            {
                CurrentPercent = 0;
            }
        }

        public void ReadInputs()
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (CurrentPercent >= MinWinRange && CurrentPercent <= MaxWinRange)
                {
                    IsComplete = true;
                }
            }
        }
    }

    internal class Dial_Visual : EasyDraw
    {
        readonly EasyDraw dialVisual;
        readonly EasyDraw mover;
        readonly EasyDraw goal;

        readonly Dial dial;
        public Dial_Visual(string filename, string moverFilename, Dial dial, int ySpacing, int DialWidth, int DialHeight) : base(DialWidth, DialHeight, false)
        {
            this.dial = dial;
            y = ySpacing;
            SetOrigin(width / 2, height / 2);

            goal = new EasyDraw(moverFilename, false);
            goal.SetColor(1, 0, 0);
            goal.SetXY(0, -height / 2);
            goal.SetOrigin(goal.width / 2, goal.height / 2);
            AddChild(goal);

            dialVisual = new EasyDraw(filename, false);
            dialVisual.SetOrigin(dialVisual.width / 2, dialVisual.height / 2);
            AddChild(dialVisual);

            mover = new EasyDraw(moverFilename, false);
            mover.SetXY(0, -dialVisual.height / 2 - 5);
            mover.SetOrigin(mover.width / 2, mover.height / 2);
            dialVisual.AddChild(mover);

            goal.width = (int)((DialWidth * (dial.WinRange / 100f)) * 3.6f);
            goal.height = 30;
            dialVisual.width = DialWidth;
            dialVisual.height = DialHeight;
            mover.width = 20;
            mover.height = 20;
        }

        void Update()
        {
            if (dial.RotateRight)
            {
                dialVisual.rotation = (dial.CurrentPercent + 1) * 3.6f - (dial.WinRange / 2 * 3.6f);
            }
            else
            {
                dialVisual.rotation = -((dial.CurrentPercent + 1) * 3.6f - (dial.WinRange / 2 * 3.6f));
            }
        }
    }
}
>>>>>>> b582a8cca3771668464bdd36d2222dedc1dbed23:gxpengine_template/MyClasses/Module_Dials.cs
