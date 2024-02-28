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
        readonly Sprite _pumpPipe;
        readonly AnimationSprite _pump;
        readonly byte _pumpAnimSpeed;
        public Module_Pump_Visual2(Module_Pump logic, TiledObject data)
        {
            _moduleLogic = logic;

            _arrow = new Sprite(data.GetStringProperty("ArrowPath", "Assets/Air_Arrow.PNG"), true, false);
            _arrow.SetOrigin(_arrow.width/2, _arrow.height/2);

            _meter = new Sprite(data.GetStringProperty("MeterPath", "Assets/Air_In.PNG"), true, false);
            _meter.SetOrigin(_meter.width / 2, _meter.height / 2);

            _pumpPipe = new Sprite(data.GetStringProperty("PipePath"),true,false);
            _pumpPipe.SetOrigin(_pumpPipe.width / 2, _pumpPipe.height / 2);

            _pump = new AnimationSprite(data.GetStringProperty("PumpPath"), 7, 4, 25, true, false);
            _pump.SetOrigin(_pump.width / 2, _pump.height / 2);
            _pumpAnimSpeed = (byte)data.GetIntProperty("PumpAnimSpeed", 2);
            _pump.SetCycle(0, 24, _pumpAnimSpeed);
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

            _container.AddChild(_pumpPipe);
            _pumpPipe.SetXY(_moduleLogic.width / 2, _moduleLogic.height / 2);

            _container.AddChild(_pump);
            _pump.SetXY(_moduleLogic.width / 2 +140, _moduleLogic.height / 2);

        }
        protected override void OnDestroy()
        {
            _container.Destroy();
        }
        void Update()
        {
            if(_moduleLogic.Charging)
            {
                _pump.AnimateFixed();
            }
            _arrow.rotation = _startDeg + _moduleLogic.ChargePersentage * (_endDeg - _startDeg) ;
        }
    }
}
