using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Modules
{
    public class Module_InfoCurrent_Visual : GameObject
    {
        readonly Module_InfoCurrent _moduleLogic;

        Pivot _container;
        public Module_InfoCurrent_Visual(Module_InfoCurrent logic, TiledObject data)
        {
            _moduleLogic = logic;

            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;

            _container = new Pivot();
            MyUtils.MyGame.CurrentScene.AddChild(_container);
        }
    }
}
