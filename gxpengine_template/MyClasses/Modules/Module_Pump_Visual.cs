using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Pump_Visual : GameObject
    {
        Module_Pump _moduleLogic;

        Pivot _container;
        EasyDraw bg;
        EasyDraw fill;

        public Module_Pump_Visual(Module_Pump logic)
        {
            _moduleLogic = logic;
            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);
            bg = new EasyDraw(_moduleLogic.width, _moduleLogic.height);
            fill = new EasyDraw(_moduleLogic.width, 1);
            bg.Clear(10,160,10);
            visible = true;
            fill.Clear(200,60,0);
            AddChild( new Coroutine(Init()) );
        }

        IEnumerator Init()
        {
            yield return null;

            _container.SetXY(_moduleLogic.x, _moduleLogic.y);

            _container.AddChild(bg);
            _container.AddChild(fill);
            bg.width = _moduleLogic.width;
            bg.height = _moduleLogic.height;
            fill.width = _moduleLogic.width;

            fill.width = bg.width;
        }

        void Update()
        {
            fill.height = (int)(bg.height * (1 - _moduleLogic.ChargePersentage));
        }

        protected override void OnDestroy()
        {
            _container.Destroy();
        }
    }
}
