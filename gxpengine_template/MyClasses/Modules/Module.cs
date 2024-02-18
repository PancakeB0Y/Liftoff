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
        public event Action Success;
        public event Action Fail;

        protected readonly int timer;
        protected float currTime;

        TextMesh _timerText;

        public Module(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, addCollider:false)
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
