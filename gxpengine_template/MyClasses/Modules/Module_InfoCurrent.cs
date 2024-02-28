using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_InfoCurrent : Module
    {

        Module_InfoCurrent_Visual _visual;
        TiledObject _data;

        public Module_InfoCurrent(string fn, int c, int r, TiledObject data) : base(fn, c, r, data)
        {
            _data = data;
            moduleType = ModuleTypes.Switch;

            _visual = new Module_InfoCurrent_Visual(this, data);
            AddChild(_visual);
        }

        override public object Clone()
        {
            var clone = new Module_InfoCurrent(texture.filename, _cols, _rows, _data);
            clone.width = width;
            clone.height = height;

            return clone;
        }

        protected override void StartTimer()
        {
        }
    }
}
