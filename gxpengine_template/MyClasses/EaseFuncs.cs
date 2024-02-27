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
        
        public static float EaseInSine(float t)
        {
            return 1 - Mathf.Cos((t * Mathf.PI) / 2);
        }
        
        public static float EaseOutElastic(float x)
        {
            float c4 = (2 * Mathf.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1;
        }
        
        public static float EaseInOutExpo(float x)
        {
            return x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2
              : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
        }

        public static Func<float,float> Factory(string easeFuncName)
        {
            switch (easeFuncName)
            {
                case "EaseOutBack":
                    return EaseOutBack;
                case "Linear":
                    return Linear;
                case "EaseInSine":
                    return EaseInSine;
                case "EaseOutElastic":
                    return EaseOutElastic;
                case "EaseInOutExpo":
                    return EaseInOutExpo;
                default:
                    Console.WriteLine("Warning! there's no ease func with name: " + easeFuncName);
                    return Linear;
            }
        }

    }
}
