using System;
using System.Collections;
using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.Modules;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Pump : Module
    {
        //clamp 0 to 1
        public float ChargePersentage { get; private set; } = 1f;

        readonly float _chargeSpeed;
        readonly float _dischargeSpeed;

        public Module_Pump(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            moduleType = moduleTypes.OneButton;

            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 0.1f);
            _dischargeSpeed = data.GetFloatProperty("DishargeSpeed", 0.04f);
            alpha = 0;
            //need visual
            Module_Pump_Visual2 visual = new Module_Pump_Visual2(this,data);
            AddChild(visual);
        }

        void Update()
        {
            var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

            if (Input.GetKey(Key.A))//whatever key
            {

                ChargePersentage += _chargeSpeed * deltaInSeconds;

                if (ChargePersentage >= 1)
                {
                    ChargePersentage = 1;
                }

            }
            else
            {
                ChargePersentage -= _dischargeSpeed * deltaInSeconds;

                if (ChargePersentage <= 0) RaiseFailEvent();
            }

        }

        protected override void OnTimeEnd()
        {
            if (ChargePersentage > 0)
            {
                RaiseSuccesEvent();
            }
        }
    }
}

