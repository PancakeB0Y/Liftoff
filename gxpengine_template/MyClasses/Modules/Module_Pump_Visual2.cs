using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Pump_Visual2 : GameObject
    {

        readonly Module_Pump _moduleLogic;
        readonly Pivot _container;
        readonly Sprite _arrow;
        readonly Sprite _meter;
        readonly int _startDeg;
        readonly int _endDeg;

        public Module_Pump_Visual2(Module_Pump logic, TiledObject data)
        {
            _moduleLogic = logic;

            _arrow = new Sprite(data.GetStringProperty("ArrowPath", "Assets/Air_Arrow.PNG"), true, false);
            _arrow.SetOrigin(_arrow.width/2, _arrow.height/2);

            _meter = new Sprite(data.GetStringProperty("MeterPath", "Assets/Air_In.PNG"), true, false);
            _meter.SetOrigin(_meter.width / 2, _meter.height / 2);

            _startDeg = data.GetIntProperty("StartDeg", 30);
            _endDeg = data.GetIntProperty("EndDeg", 330);

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);

            AddChild(new Coroutine(Init()));

        }

        IEnumerator Init()
        {
            yield return null;

            _container.SetXY(_moduleLogic.x, _moduleLogic.y);
            _container.AddChild(_meter);
            _meter.SetXY(_moduleLogic.width / 2, _moduleLogic.height / 2);
            _container.AddChild(_arrow);
            _arrow.SetXY(_moduleLogic.width / 2, _moduleLogic.height / 2);

        }
        protected override void OnDestroy()
        {
            _container.Destroy();
        }
        void Update()
        {
            _arrow.rotation = _startDeg + _moduleLogic.ChargePersentage * (_endDeg - _startDeg) ;
        }
    }
}
