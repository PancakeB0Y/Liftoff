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

        readonly TextMesh _timerText;

        public Module(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, addCollider:false)
        {
            timer = data.GetIntProperty("TimerSeconds", 5);
            currTime = timer;
            var timeRoutine = new Coroutine(Timer());

            Fail += OnFail;
            Success += OnSuccess;

            AddChild(timeRoutine);

            _timerText = new TextMesh("4", 400, 400);
            AddChild(new Coroutine(Initi()));
        }

        IEnumerator Initi()
        {
            yield return null;

            SetXY(x-width/2, y-height/2);

            var test = new Sprite("Assets/square.png", true, false);
            test.SetOrigin(test.width / 2, test.height / 2);
            test.SetColor(1, 0, 0);
            test.width = 5;
            test.height = 5;
            AddChild(test);

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

        //these need to be removed
        protected virtual void OnFail()
        {
            Destroy();
            Fail -= OnFail;
            Success -= OnSuccess;

        }
        protected virtual void OnSuccess()
        {
            Destroy();
            Success -= OnSuccess;
            Fail -= OnFail;

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
    public abstract class Module : AnimationSprite
    {
        public event Action Success;
        public event Action Fail;

        public event Action<moduleTypes> End;

        protected readonly int timer;
        protected float currTime;

        TextMesh _timerText;

        public enum moduleTypes { Switch, Dpad, ThreeButtons, OneButton }
        public moduleTypes moduleType = moduleTypes.Switch;
        public Module(TiledObject data) : base("Assets/square.png", 1, 1)
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

        protected virtual void StartTimer()
        {
            var timeRoutine = new Coroutine(Timer());
            AddChild(timeRoutine);
        }

        protected virtual void LoadVisuals()
        {

        }

        public virtual void StartModule()
        {
            StartTimer();
            LoadVisuals();
        }

        protected virtual void OnTimeEnd()
        {

        }
        public void OnFail()
        {
            Console.WriteLine("Module failed " + moduleType);
            Fail -= OnFail;
            Destroy();
        }
        public void OnSuccess()
        {
            Console.WriteLine("Module success " + moduleType);
            Success -= OnSuccess;
            Destroy();
        }
        protected void RaiseSuccesEvent()
        {
            Success?.Invoke();
            End?.Invoke(moduleType);
        }

        protected void RaiseFailEvent()
        {
            Fail?.Invoke();
            End?.Invoke(moduleType);
        }

    }
}
>>>>>>> 3cfc9c4790888f313981e10d708843ca83967071:gxpengine_template/MyClasses/Module.cs
