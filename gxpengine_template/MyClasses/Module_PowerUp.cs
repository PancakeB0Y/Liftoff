using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    internal class Module_PowerUp : Module
    {
        //Charging logic variables

        //from 0 to 1
        public float CurrentCharge {  get; private set; }
        readonly float _chargeSpeed;

        //Bar positioning logic variables

        //from 0 to 1
        public float CurrentBarPersentage { get; private set; }
        public float BarRandomPosition { get; private set; }
        public float ChargeZoneSize { get; private set; }

        readonly float _barMoveUpSpeed;
        readonly float _barMoveDownSpeed;

        readonly Module_PowerUp_Visual visual;
        public Module_PowerUp(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 10f);

            _barMoveUpSpeed = data.GetFloatProperty("BarMoveUpSpeed", 0.06f);
            _barMoveDownSpeed = data.GetFloatProperty("BarMoveDownSpeed", 0.04f);

            ChargeZoneSize = data.GetFloatProperty("ChargeZoneSize", 0.2f);
            BarRandomPosition = Utils.Random(ChargeZoneSize, 1 - ChargeZoneSize);

            visual = new Module_PowerUp_Visual(this);
            AddChild(visual);
        }
        void Update()
        {
            var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

            if(Input.GetKey(Key.S))
            {
                CurrentBarPersentage += _barMoveUpSpeed * deltaInSeconds;
            }
            else
            {
                CurrentBarPersentage -= _barMoveDownSpeed * deltaInSeconds;
            }

            if (InsideChargeZone(CurrentBarPersentage))
            {
                CurrentCharge += _chargeSpeed * deltaInSeconds;
                if (CurrentCharge >= 1)
                {
                    RaiseSuccesEvent();
                    CurrentCharge = 1;
                }
            }
            
            //Console.WriteLine("current charge " + _currentCharge);

        }
        bool InsideChargeZone(float position)
        {
            return position > BarRandomPosition - ChargeZoneSize
                    &&
                   position < BarRandomPosition + ChargeZoneSize;
        }
        protected override void OnTimeEnd()
        {
            if (CurrentCharge >= 1)
                RaiseSuccesEvent();
            else
                RaiseFailEvent();
        }
    }

    internal class Module_PowerUp_Visual : GameObject
    {
        EasyDraw bar;
        EasyDraw bg;

        EasyDraw chargeZone;

        Module_PowerUp powerUp;

        public Module_PowerUp_Visual(Module_PowerUp powerUp, int barSize = 10)
        {
            this.powerUp = powerUp;
            var w = 50;
            var h = 100;

            bg = new EasyDraw(w, h, false);
            bg.Clear(Color.Red);

            chargeZone = new EasyDraw(w, (int)(powerUp.ChargeZoneSize * h), false);
            chargeZone.y = (int)(h * powerUp.BarRandomPosition);
            bg.Clear(Color.Green);



            bar = new EasyDraw(w, barSize, false);
            bar.Clear(Color.Blue);
            Console.WriteLine("aaaa");
            AddChild(bar);
            AddChild(bg);
            AddChild(chargeZone);
        }
        void Update()
        {
            bar.y = powerUp.CurrentBarPersentage * bg.height;

        }
    }
}
