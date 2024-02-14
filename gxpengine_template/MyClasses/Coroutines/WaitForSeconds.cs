using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses.Coroutines
{
    internal class WaitForSeconds : ICoroutineStepper
    {
        readonly float _endWaitTimeMs;

        public WaitForSeconds(float pTimeSeconds)
        {
            _endWaitTimeMs = Time.time + pTimeSeconds * 1000;
        }

        public bool IsDone()
        {
            return Time.time >= _endWaitTimeMs;
        }
    }
}
