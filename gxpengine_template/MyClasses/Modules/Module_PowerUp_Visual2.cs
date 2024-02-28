using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_PowerUp_Visual2 : GameObject
    {
        readonly Sprite _bar;
        readonly Sprite _bg;
        readonly Sprite _chargeZone;
        readonly AnimationSprite _battery;
        readonly Module_PowerUp powerUp;
        readonly Pivot _container;
        readonly float[] _chargeThreshHolds;
        public Module_PowerUp_Visual2(Module_PowerUp powerUp, TiledObject data)
        {
            this.powerUp = powerUp;
            _container = new Pivot();

            _bg = new Sprite(data.GetStringProperty("BgFilePath", "Assets/PowerUp/Power_Up_RedBackground.PNG"),true,false);
            _bar = new Sprite(data.GetStringProperty("BarFilePath", "Assets/PowerUp/Power_Up_Arrow.PNG"),true,false);
            _chargeZone = new Sprite(data.GetStringProperty("ChargeZoneFilePath", "Assets/PowerUp/Power_Up_GreenPart.PNG"), true, false);
            _battery = new AnimationSprite
            (
                data.GetStringProperty("BatteryFilePath", "Assets/PowerUp/Battery_Sprite.png"),
                data.GetIntProperty("BatterySS_Cols",4),
                data.GetIntProperty("BatterySS_Rows",1),
                4,
                true, false
            );
            _chargeThreshHolds = data.GetStringProperty("ChargeThreshHoldsCSV","0.2,0.4,0.6,0.8").Split(',').Select(x=> float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;

            MyUtils.MyGame.CurrentScene.AddChild(_container);

            var w = _bg.width;
            var h = _bg.height;


            var chargeZoneHeight = (int)(powerUp.ChargeZonePersentage * h);
            _chargeZone.SetOrigin(0, chargeZoneHeight / 2);
            _chargeZone.height = chargeZoneHeight;
            _chargeZone.y = (int)(h * powerUp.ChargeZoneRandomPosition);
            _chargeZone.x += 10;

            _battery.SetXY(w + 10, 0);
            _battery.scale = 0.5f;

            _bar.x += 10;

            _container.AddChild(_bg);
            _container.AddChild(_chargeZone);
            _container.AddChild(_bar);
            _container.AddChild(_battery);

            _container.SetXY(powerUp.x, powerUp.y);

        }
        protected override void OnDestroy()
        {
            _container.Destroy();
        }
        void Update()
        {
            _bar.y = _bg.y + powerUp.CurrentBarPersentage * _bg.height;
            //battery
            int i = 0;
            foreach (var threshHold in _chargeThreshHolds)
            {
                if (threshHold <= powerUp.CurrentCharge)
                    _battery.SetFrame(i);
                i++;
            }
        }
    }
}
