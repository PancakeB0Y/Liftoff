﻿using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_Waves : Module
    {
        Module_Waves_Visual _visual;
        TiledObject _data;

        const float _stretchW = 0.01f;
        const float _stretchH = 0.01f;

        const float _goalW = 1.00f;
        const float _goalH = 1.00f;

        float _curW = 1.0f;
        float _curH = 1.4f;
        public Module_Waves(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            _data = data;
            moduleType = ModuleTypes.Dpad;

            float minRandW = data.GetIntProperty("MinWidthStretch", 30);
            float minRandH = data.GetIntProperty("MinHeightStretch", 30);

            float randW = data.GetIntProperty("MaxWidthStretch", 80);
            float randH = data.GetIntProperty("MaxHeightStretch", 80);

            randW = Utils.Random(minRandW, randW) / 100f;
            randH = Utils.Random(minRandH, randH) / 100f;

            _curW = _goalW + (Utils.Random(0, 2) == 0 ? randW : -randW);
            _curW = Mathf.Clamp(_curW, 0.1f, 1);
            _curH = _goalH + (Utils.Random(0, 2) == 0 ? randH : -randH);
            _curH = Mathf.Clamp(_curH, 0.1f, 1.4f);

            _visual = new Module_Waves_Visual(this, data);
            _visual.SetWH(_curW, _curH);
            AddChild(_visual);
        }

        override public object Clone()
        {
            var clone = new Module_Waves(texture.filename, _cols, _rows, _data);

            return clone;
        }

        protected override void OnTimeEnd()
        {
            RaiseFailEvent();
        }

        void IsComplete()
        {
            if (_visual == null) return;

            if (_visual.IsComplete())
            {
                RaiseSuccesEvent();
            }
        }
        void ReadInputs()
        {
            if (Input.GetKey(Key.A))
            {
                _curW -= _stretchW;
                _visual.Stretch(-_stretchW, 0);
                IsComplete();
            }
            else if (Input.GetKey(Key.D))
            {
                _curW += _stretchW;
                _visual.Stretch(_stretchW, 0);
                IsComplete();
            }
            else if (Input.GetKey(Key.W))
            {
                _curH += _stretchH;
                _visual.Stretch(0, _stretchH);
                IsComplete();
            }
            else if (Input.GetKey(Key.S))
            {
                _curH -= _stretchH;
                _visual.Stretch(0, -_stretchH);
                IsComplete();
            }
        }

        void Update()
        {
            ReadInputs();
        }
    }
}
