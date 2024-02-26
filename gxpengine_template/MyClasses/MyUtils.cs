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

        public static bool IndexIsOnGridCorner<T>(this IEnumerable<T> collection, int columns, int index)
        {
            return index == 0 ||
                (index == columns - 1) ||
                (index == collection.Count() - columns) ||
                index == collection.Count() - 1;
        }

        private static void FindInterfaces<T>(this GameObject obj, List<T> interfaces)
        {
            if (obj.GetChildCount() == 0) return;

            foreach (var c in obj.GetChildren())
            {
                if (c is T iface) interfaces.Add(iface);

                c.FindInterfaces(interfaces);
            }
        }

        public static bool IsPrime(this int n)
        {
            if (n <= 1)  return false;

            if (n == 2 || n == 3)  return true;

            if (n % 2 == 0 || n % 3 == 0)  return false;

            for (int i = 5; i <= Mathf.Sqrt(n); i += 6)
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;

            return true;
        }
        
        /// <summary>
        /// Doesn't work with negative numbers
        /// </summary>
        /// <param name="n"></param>
        /// <param name="divider"></param>
        /// <returns></returns>
        
        public static bool IsPrime(this int n, out int divider)
        {
            divider = -1;
            if (n <= 1)  return false;
            

            if (n == 2 || n == 3)  return true;

            if (n % 2 == 0 )
            {
                divider = 2;
                return false;
            }

            if (n % 3 == 0)
            {
                divider = 3;
                return false;
            }

            for (int i = 5; i <= Mathf.Sqrt(n); i += 6)
                if (n % i == 0)
                {
                    divider = i;
                    return false;
                }
                else if (n % (i + 2) == 0)
                {
                    divider = i + 2;
                    return false;
                }

            return true;
        }
        
    }
}
