using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses
{
    public class Ground : Sprite
    {
        public Ground(string filename, bool keepInCache = false, bool addCollider = true) : base(filename, keepInCache, addCollider)
        {
        }
    }
}
