using GXPEngine;
using System.Drawing;

namespace gxpengine_template.MyClasses
{
    public class Module_PowerUp_Visual : GameObject
    {
        readonly EasyDraw bar;
        readonly EasyDraw bg;
        readonly EasyDraw chargeZone;
        readonly EasyDraw battery;
        readonly Module_PowerUp powerUp;

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
            battery.Rect(0,0,battery.width,battery.height * powerUp.CurrentCharge);
        }
    }
}
