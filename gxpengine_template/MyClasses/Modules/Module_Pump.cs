using System;
using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Pump : Module
    {
        //clamp 0 to 1
        public float ChargePersentage { get; private set; }

        readonly float _chargeSpeed;
        readonly float _dischargeSpeed;
        public Module_Pump(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            //modulePos = modulePosition.Right;

            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 0.1f);
            _dischargeSpeed = data.GetFloatProperty("DishargeSpeed", 0.04f);

            //need visual
        }

        void Update()
        {
            if (Input.GetKey(Key.A))//whatever key
            {
                var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

                ChargePersentage += _chargeSpeed * deltaInSeconds;

                if (ChargePersentage >= 1)
                {
                    ChargePersentage = 1;
                }

            }
            else
            {
                ChargePersentage -= _dischargeSpeed;

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
