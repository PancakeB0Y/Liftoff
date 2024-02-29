using GXPEngine;
using System;
using TiledMapParser;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System.Collections;

namespace gxpengine_template.MyClasses
{
    public abstract class Module : AnimationSprite, ICloneable
    {
        public event Action<ModuleTypes> End;
        public event Action<Module> Success;
        public event Action<Module> Fail;

        public enum ModuleTypes { Switch, Dpad, ThreeButtons, OneButton }
        public ModuleTypes moduleType = ModuleTypes.Switch;

        public int Difficulty;
        public int SuccesScore { get; }

        public float CurrTime => currTime;
        protected float currTime;
        public float TotalTime => timer;
        protected readonly int timer;

        bool isComplete = false;

        public Module(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, addCollider: false)
        {
            timer = data.GetIntProperty("TimerSeconds", 5);
            currTime = timer;
            Difficulty = data.GetIntProperty("Difficulty", 1);
            Difficulty = (int)Mathf.Clamp(Difficulty, 1, 3);
            SuccesScore = data.GetIntProperty("SuccesScore",10);

            alpha = 0f;
            AddChild(new Coroutine(Initialize()));
        }

        public virtual object Clone()
        {
            var clone = (Module)MemberwiseClone();

            return clone;
        }

        IEnumerator Initialize()
        {
            yield return null;

            SetXY(x - width / 2, y - height / 2);
        }

        IEnumerator Timer()
        {
            while (currTime > 0)
            {
                var deltaInSeconds = Mathf.Min(Time.deltaTime * 0.001f, 0.04f);

                currTime -= deltaInSeconds;
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

        protected void RaiseSuccesEvent()
        {
            if (isComplete) { return; }
            isComplete = true;
            Success?.Invoke(this);
            End?.Invoke(moduleType);
        }

        protected void RaiseFailEvent()
        {
            if (isComplete) { return; }
            isComplete = true;
            Fail?.Invoke(this);
            End?.Invoke(moduleType);
        }


    }
}
