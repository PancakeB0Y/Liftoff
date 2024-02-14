using System;
using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Module_Pump : Module
    {
        public float ChargePersentage { get; private set; }

        readonly float _maxCapacity;
        readonly float _chargeSpeed;
        readonly float _dischargeSpeed;
        float _currentCharge = 0;
        
        public Module_Pump(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, data)
        {
            _maxCapacity = data.GetFloatProperty("MaxCapacity",10);
            _chargeSpeed = data.GetFloatProperty("ChargeSpeed", 10);
            _dischargeSpeed = data.GetFloatProperty("DishargeSpeed", 0.04f);
            _currentCharge = _maxCapacity;
        }

        void Update()
        {
            if(Input.GetKey(Key.A))//whatever key
            {
                var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

                _currentCharge += _chargeSpeed * deltaInSeconds;

                if (_currentCharge >= _maxCapacity)
                {
                    _currentCharge = _maxCapacity;
                }   
                    
            }
            else
            {
                _currentCharge -= _dischargeSpeed;

                if(_currentCharge <= 0)   RaiseFailEvent();
            }

            ChargePersentage = _currentCharge / _chargeSpeed;
        }

        protected override void OnTimeEnd()
        {
            if(_currentCharge > 0)
            {
                RaiseSuccesEvent();
            }
        }
    }
}
