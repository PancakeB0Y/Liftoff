using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Dials : AnimationSprite
    {
        readonly List<Dial> dials;

        (int minValue, int maxValue) winRange = (0, 100);

        public Module_Dials(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows)
        {
            dials = new List<Dial>
            {
                new Dial(data.GetFloatProperty("Speed", 0.5f), 65, winRange), //65 = A
                new Dial(data.GetFloatProperty("Speed", 0.5f), 83, winRange), //83 = S
                new Dial(data.GetFloatProperty("Speed", 0.5f), 68, winRange) //68 = D
            };
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
            CheckIfComplete();
        }
    }

    public class Dial
    {
        float percent;
        readonly float speed;
        public bool isComplete;
        readonly (int minValue, int maxValue) winRange;
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
}
