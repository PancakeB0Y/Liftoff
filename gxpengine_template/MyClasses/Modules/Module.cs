<<<<<<< HEAD:gxpengine_template/MyClasses/Modules/Module.cs
﻿using GXPEngine;
using System;
using TiledMapParser;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System.Collections;

namespace gxpengine_template.MyClasses
{
    public abstract class Module : AnimationSprite
    {
        public event Action Success;
        public event Action Fail;

        protected readonly int timer;
        protected float currTime;

        TextMesh _timerText;

        public Module(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows)
        {
            timer = data.GetIntProperty("TimerSeconds", 5);
            currTime = timer;
            var timeRoutine = new Coroutine(Timer());

            Fail += OnFail;
            Success += OnSuccess;

            AddChild(timeRoutine);

            _timerText = new TextMesh("4", 400, 400);
            //AddChild(_timerText);
        }
        IEnumerator Timer()
        {
            while (currTime > 0)
            {
                var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

                currTime -= deltaInSeconds;
                _timerText.Text = currTime.ToString();
                yield return null;
            }
            OnTimeEnd();
        }
        protected virtual void OnTimeEnd()
        {

        }
        protected void OnFail()
        {
            Destroy();
        }
        protected void OnSuccess()
        {
            Destroy();
        }
        protected void RaiseSuccesEvent()
        {
            Success?.Invoke();
        }

        protected void RaiseFailEvent()
        {
            Fail?.Invoke();
        }

    }
}
=======
﻿using GXPEngine;
using System;
using TiledMapParser;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System.Collections;

namespace gxpengine_template.MyClasses
{
    public abstract class Module : Sprite
    {
        public event Action Success;
        public event Action Fail;

        protected readonly int timer;
        protected float currTime;

        TextMesh _timerText;

        public enum modulePosition { Left, Right, Top, Bottom }
        public modulePosition modulePos = modulePosition.Left;
        public Module(TiledObject data) : base("Assets/square.png")
        {
            timer = data.GetIntProperty("TimerSeconds", 5);
            currTime = timer;
            /*var timeRoutine = new Coroutine(Timer());
            AddChild(timeRoutine);*/

            Fail += OnFail;
            Success += OnSuccess;

            _timerText = new TextMesh("4", 400, 400);
            alpha = 0f;
            //AddChild(_timerText);
        }
        IEnumerator Timer()
        {
            while (currTime > 0)
            {
                var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

                currTime -= deltaInSeconds;
                _timerText.Text = currTime.ToString();
                yield return null;
            }
            OnTimeEnd();
        }

        public void StartTimer()
        {
            var timeRoutine = new Coroutine(Timer());
            AddChild(timeRoutine);
        }

        protected virtual void OnTimeEnd()
        {

        }
        protected void OnFail()
        {
            Console.WriteLine("Module failed");
            Destroy();
        }
        protected void OnSuccess()
        {
            Console.WriteLine("Module success");
            Destroy();
        }
        protected void RaiseSuccesEvent()
        {
            Success?.Invoke();
        }

        protected void RaiseFailEvent()
        {
            Fail?.Invoke();
        }

    }
}
>>>>>>> b582a8cca3771668464bdd36d2222dedc1dbed23:gxpengine_template/MyClasses/Module.cs
