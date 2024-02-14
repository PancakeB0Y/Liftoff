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
