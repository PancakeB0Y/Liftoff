using GXPEngine;
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
        public Module_Dials(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            moduleType = moduleTypes.ThreeButtons;

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
            /*for (int i = 0; i < _dials.Count; i++)
            {
                Dial_Visual curVisual = new Dial_Visual(dialFilename, moverFilename, _dials[i], i * (int)(DialWidth * 1.3f) + (DialWidth / 2), DialWidth, DialHeight);
                _visuals.Add(curVisual);
                AddChild(curVisual);
            }*/
        }

        void UpdateDials()
        {
            foreach (Dial dial in _dials)
            {
                dial.Move();
                dial.ReadInputs();
            }
        }
        bool IsComplete()
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

        protected override void LoadVisuals()
        {
            for (int i = 0; i < _dials.Count; i++)
            {
                Dial_Visual curVisual = new Dial_Visual(dialFilename, moverFilename, _dials[i], i * (int)(DialWidth * 1.3f) + (DialWidth / 2), DialWidth, DialHeight);
                _visuals.Add(curVisual);
                AddChild(curVisual);
            }
        }

        void Update()
        {
            UpdateDials();
        }

        protected override void OnTimeEnd()
        {
            if (IsComplete())
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
