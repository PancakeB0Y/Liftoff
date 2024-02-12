using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gxpengine_template.MyClasses
{
    public static class MyUtils
    {
        public static MyGame MyGame => (MyGame)MyGame.main;

        public static bool TryGetIndex<T>(this T[] array, Predicate<T> predicate, out int index)
        {
            index = Array.FindIndex(array, predicate);
            return index > -1;
        }

        public static List<T> FindInterfaces<T>(this GameObject obj) where T : class
        {
            List<T> startObjs = new List<T>();
            obj.FindInterfaces(startObjs);
            return startObjs.ToList();
        }

        private static void FindInterfaces<T>(this GameObject obj, List<T> interfaces) 
        {
            if (obj.GetChildCount() == 0) return;

            foreach (var c in obj.GetChildren()) 
            { 
                if(c is T iface)  interfaces.Add(iface);

                c.FindInterfaces(interfaces);
            }
        }
        
    }
}
