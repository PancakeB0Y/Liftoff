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
        TiledObject _data;
        public Module_Dials(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            _data = data;
            moduleType = ModuleTypes.ThreeButtons;

            int winRange = 10;
            Dials = new List<Dial>
            {
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), Key.H, winRange),
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), Key.J, winRange),
                new Dial(data.GetFloatProperty("DialSpeed", 0.5f), Key.K, winRange)
            };

            _visual = new Module_Dials_Visual(this, data);
            AddChild(_visual);
        }
        override public object Clone()
        {
            var clone = new Module_Dials(texture.filename, _cols, _rows, _data);

            return clone;
        }

        void UpdateDials()
        {
            foreach (Dial dial in Dials)
            {
                dial.Move();
                dial.ReadInputs();
            }

            if (IsComplete())
            {
                RaiseSuccesEvent();
            }
        }
        bool IsComplete()
        {
            bool hasWon = true;
            foreach (Dial dial in Dials)
            {
                if (dial.IsComplete == false)
                {
                    hasWon = false;
                    break;
                }
            }

            return hasWon;
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
