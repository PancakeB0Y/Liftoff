using GXPEngine;
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

        public event Action<moduleTypes> End;

        protected readonly int timer;
        protected float currTime;

        TextMesh _timerText;

        public enum moduleTypes { Switch, Dpad, ThreeButtons, OneButton }
        public moduleTypes moduleType = moduleTypes.Switch;
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
