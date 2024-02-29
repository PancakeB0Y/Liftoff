using GXPEngine;
using gxpengine_template.MyClasses.Modules;
using System;
using System.Xml.Linq;
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

        readonly TiledObject _data;
        public Module_PowerUp(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            moduleType = ModuleTypes.Switch;
            _data = data;

            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 0.1f);

            _barMoveUpSpeed = data.GetFloatProperty("BarMoveUpSpeed", 0.06f);
            _barMoveDownSpeed = data.GetFloatProperty("BarMoveDownSpeed", 0.04f);

            ChargeZonePersentage = data.GetFloatProperty("ChargeZoneSize", 0.2f);
            ChargeZoneRandomPosition = Utils.Random(ChargeZonePersentage, 1 - ChargeZonePersentage);
            alpha = 0;
            var visual = new Module_PowerUp_Visual2(this,data);
            AddChild(visual);
        }

        override public object Clone()
        {
            var clone = new Module_PowerUp(texture.filename, _cols, _rows, _data);

            return clone;
        }
        void Update()
        {
            var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

            if (Input.GetKey(Key.B))
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
