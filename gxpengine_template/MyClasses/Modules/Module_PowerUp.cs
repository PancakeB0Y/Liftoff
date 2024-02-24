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
            moduleType = moduleTypes.Switch;

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

        protected override void LoadVisuals()
        {
            AddChild(visual);
        }
    }
}
