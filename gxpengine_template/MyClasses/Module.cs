using GXPEngine;
using System;
using TiledMapParser;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System.Collections;

namespace gxpengine_template.MyClasses
{
    public abstract class Module : AnimationSprite
    {
        public event Action Succes;
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
            Succes += OnSuccess;

            AddChild(timeRoutine);

            _timerText = new TextMesh("4", 400, 400);
            AddChild(_timerText);
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
            Succes?.Invoke();
        }

        protected void RaiseFailEvent() 
        {
            Fail?.Invoke();
        }

    }
}
