using GXPEngine;
using gxpengine_template.MyClasses.Modules;
using System;
using System.Collections;
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
        public readonly List<Dial> Dials;

        Module_Dials_Visual _visual;
        public Module_Dials(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            moduleType = moduleTypes.ThreeButtons;

            int winRange = 10;
            Dials = new List<Dial>
            {
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), 65, winRange), //65 = A
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), 83, winRange), //83 = S
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), 68, winRange) //68 = D
            };

            _visual = new Module_Dials_Visual(this, data);
        }

        void UpdateDials()
        {
            foreach (Dial dial in Dials)
            {
                dial.Move();
                dial.ReadInputs();
            }
        }
        bool IsComplete()
        {
            bool isWon = true;
            foreach (Dial dial in Dials)
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
            AddChild(_visual);
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

    public class Dial
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
}
