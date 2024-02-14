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
        readonly List<Dial> dials;
        readonly List<Dial_Visual> visuals;

        public Module_Dials(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {

            int winRange = data.GetIntProperty("WinRange", 20);
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
        readonly int winRange;
        readonly int button;

        public int MinWinRange;
        public int MaxWinRange;

        public Dial(float speed, int button, int winRange)
        {
            this.speed = speed;
            this.button = button;
            this.winRange = winRange;
            MinWinRange = Utils.Random(0, 100 - winRange);
            MaxWinRange = MinWinRange + winRange;
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
                if (percent >= MinWinRange && percent <= MaxWinRange)
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

        Dial dial;

        int w = 50;
        int h = 50;
        public Dial_Visual(Dial dial, int y)
        {
            this.dial = dial;
            this.y = y;

            bg = new EasyDraw(w, h, false);
            bg.Clear(Color.Red);

            bar = new EasyDraw(w, 10, false);
            bar.SetXY(0, 0);
            bar.NoStroke();

            AddChild(bg);
            AddChild(bar);
        }

        void Update()
        {
            bar.Clear(Color.White);

            bar.Fill(Color.Yellow);
            float winRangeWidth = Mathf.Ceiling((dial.MaxWinRange - dial.MinWinRange) / 100f * w);
            bar.Rect(dial.MinWinRange / 100f * w + ((int)winRangeWidth / 2), 5, (int)winRangeWidth, 10);

            bar.Fill(Color.Blue);
            float moverX = bar.width * (dial.percent - 0) / (99);
            bar.Rect(moverX, 2.5f, 5, 19);
        }
    }
}
