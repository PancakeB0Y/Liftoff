<<<<<<< HEAD:gxpengine_template/MyClasses/Modules/Module_PowerUp.cs
using GXPEngine;
using System;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_PowerUp : Module
    {
        //from 0 to 1
        public float CurrentCharge { get; private set; }
        readonly float _chargeSpeed;

        //Bar positioning logic variables

        //from 0 to 1
        public float CurrentBarPersentage { get; private set; }
        public float ChargeZoneRandomPosition { get; private set; }
        public float ChargeZonePersentage { get; private set; }

        readonly float _barMoveUpSpeed;
        readonly float _barMoveDownSpeed;

        readonly Module_PowerUp_Visual visual;
        public Module_PowerUp(string fn, int c, int r, TiledObject data) : base(fn,c,r,data)
        {

            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 0.1f);

            _barMoveUpSpeed = data.GetFloatProperty("BarMoveUpSpeed", 0.06f);
            _barMoveDownSpeed = data.GetFloatProperty("BarMoveDownSpeed", 0.04f);

            ChargeZonePersentage = data.GetFloatProperty("ChargeZoneSize", 0.2f);
            ChargeZoneRandomPosition = Utils.Random(ChargeZonePersentage, 1 - ChargeZonePersentage);
            alpha = 0;
            visual = new Module_PowerUp_Visual(this);
            AddChild(visual);
        }
        void Update()
        {
            var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

            if (Input.GetKey(Key.S))
            {
                CurrentBarPersentage += _barMoveUpSpeed * deltaInSeconds;

            }
            else
            {
                CurrentBarPersentage -= _barMoveDownSpeed * deltaInSeconds;
            }

            CurrentBarPersentage = Mathf.Clamp(CurrentBarPersentage, 0, 1);

            if (InsideChargeZone(CurrentBarPersentage))
            {
                CurrentCharge += _chargeSpeed * deltaInSeconds;

                if (CurrentCharge >= 1)
                {
                    RaiseSuccesEvent();
                    CurrentCharge = 1;
                }
            }

        }
        bool InsideChargeZone(float position)
        {
            return position > ChargeZoneRandomPosition - ChargeZonePersentage / 2
                    &&
                   position < ChargeZoneRandomPosition + ChargeZonePersentage / 2;
        }
        protected override void OnTimeEnd()
        {
            if (CurrentCharge >= 1)
                RaiseSuccesEvent();
            else
                RaiseFailEvent();
        }
    }
}
=======
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
        //from 0 to 1
        public float CurrentCharge { get; private set; }
        readonly float _chargeSpeed;

        //Bar positioning logic variables

        //from 0 to 1
        public float CurrentBarPersentage { get; private set; }
        public float ChargeZoneRandomPosition { get; private set; }
        public float ChargeZonePersentage { get; private set; }

        readonly float _barMoveUpSpeed;
        readonly float _barMoveDownSpeed;

        readonly Module_PowerUp_Visual visual;
        public Module_PowerUp(TiledObject data) : base(data)
        {
            moduleType = moduleTypes.Switch;

            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 0.1f);

            _barMoveUpSpeed = data.GetFloatProperty("BarMoveUpSpeed", 0.06f);
            _barMoveDownSpeed = data.GetFloatProperty("BarMoveDownSpeed", 0.04f);

            ChargeZonePersentage = data.GetFloatProperty("ChargeZoneSize", 0.2f);
            ChargeZoneRandomPosition = Utils.Random(ChargeZonePersentage, 1 - ChargeZonePersentage);

            visual = new Module_PowerUp_Visual(this);
        }
        void Update()
        {
            var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

            if (Input.GetKey(Key.S))
            {
                CurrentBarPersentage += _barMoveUpSpeed * deltaInSeconds;
            }
            else
            {
                CurrentBarPersentage -= _barMoveDownSpeed * deltaInSeconds;
            }
            CurrentBarPersentage = Mathf.Clamp(CurrentBarPersentage, 0, 1);

            if (InsideChargeZone(CurrentBarPersentage))
            {
                CurrentCharge += _chargeSpeed * deltaInSeconds;
                Console.WriteLine("charging " + CurrentCharge);

                if (CurrentCharge >= 1)
                {
                    RaiseSuccesEvent();
                    CurrentCharge = 1;
                }
            }

        }
        bool InsideChargeZone(float position)
        {
            return position > ChargeZoneRandomPosition - ChargeZonePersentage / 2
                    &&
                   position < ChargeZoneRandomPosition + ChargeZonePersentage / 2;
        }
        protected override void OnTimeEnd()
        {
            if (CurrentCharge >= 1)
                RaiseSuccesEvent();
            else
                RaiseFailEvent();
        }

        protected override void LoadVisuals()
        {
            AddChild(visual);
        }
    }

    internal class Module_PowerUp_Visual : GameObject
    {
        EasyDraw bar;
        EasyDraw bg;

        EasyDraw chargeZone;

        EasyDraw battery;

        Module_PowerUp powerUp;

        public Module_PowerUp_Visual(Module_PowerUp powerUp, int barSize = 2)
        {
            this.powerUp = powerUp;
            var w = 50;
            var h = 100;

            bg = new EasyDraw(w, h, false);
            bg.Clear(Color.Red);

            var chargeZoneHeight = (int)(powerUp.ChargeZonePersentage * h);
            chargeZone = new EasyDraw(w, chargeZoneHeight, false);
            chargeZone.SetOrigin(0, chargeZoneHeight / 2);
            chargeZone.y = (int)(h * powerUp.ChargeZoneRandomPosition);
            chargeZone.Clear(Color.Green);

            bar = new EasyDraw(w, barSize, false);
            bar.Clear(Color.Blue);
            var bat_w = 10;
            var bat_h = 30;
            battery = new EasyDraw(bat_w, bat_h, false);
            battery.SetXY(w + 10, 0);
            battery.SetScaleXY(1, -1);

            AddChild(bg);
            AddChild(chargeZone);
            AddChild(bar);
            AddChild(battery);
        }
        void Update()
        {
            bar.y = bg.y + powerUp.CurrentBarPersentage * bg.height;
            battery.Clear(Color.Red);
            battery.ShapeAlign(CenterMode.Min, CenterMode.Min);
            battery.Fill(Color.Green);
            battery.Rect(0, 0, battery.width, battery.height * powerUp.CurrentCharge);
        }
    }
}
>>>>>>> 3cfc9c4790888f313981e10d708843ca83967071:gxpengine_template/MyClasses/Module_PowerUp.cs
