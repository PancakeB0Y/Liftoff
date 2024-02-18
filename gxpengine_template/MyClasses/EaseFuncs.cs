using GXPEngine;
using System;

namespace gxpengine_template.MyClasses
{
    public static class EaseFuncs
    {
        public static float EaseOutBack(float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            return -1.5f * t * t * t + t * t + 1.5f * t; 
        }
        public static float Linear(float t)
        {
            return Mathf.Clamp(t, 0, 1);
        }
        public static Func<float,float> Factory(string easeFuncName)
        {
            switch (easeFuncName)
            {
                case "EaseOutBack":
                    return EaseOutBack;
                default:
                    Console.WriteLine("Warning! there's no ease func with name: " + easeFuncName);
                    return Linear;
            }
        }
    }
}
