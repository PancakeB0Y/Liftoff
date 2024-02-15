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
