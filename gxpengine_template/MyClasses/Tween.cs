using GXPEngine;
using System;

namespace gxpengine_template.MyClasses
{
    public enum TweenProperty { x, y, rotation };

    public class Tween : GameObject
    {
        
        // parameters:
        TweenProperty target;
        int totalTimeMs;
        int delta;

        // Values that change over the life time:
        float lastCurveValue = 0;
        int currentTimeMs = 0;

        /// <summary>
        /// Returns a tween curve value, for input parameter t between 0 and 1.
        /// Currently only one tween curve (EaseInOut) is implemented.
        /// </summary>
        Func<float, float> GetCurveValue;

        Action _onCompletedAction;
        Action _onStartAction;
        Action _onExitAction;
        bool _started;
        //for some reason, you can still call .Destroy() when an item is destroyed.
        //So, I need to make sure that I call OnCompleted only once
        bool _completed;

        /// <summary>
        /// Adding this tween as child to a game object will tween that objects given property [target] over [timeMs] milliseconds, 
        /// with a value change of [delta].
        /// After tweening is done, this Tween destroys itself.
        /// </summary>
        public Tween(TweenProperty target, int timeMs, int delta, Func<float,float> easeFunc)
        {
            GetCurveValue = easeFunc;
            this.target = target;
            totalTimeMs = timeMs;
            this.delta = delta;
        }

        void Update()
        {
            if (parent == null) return;

            if(!_started)
            {
                _onStartAction?.Invoke();
                _started = true;
            }

            // Keep track of our life time:
            currentTimeMs += Time.deltaTime;

            ApplyTween();

            // Destroy this tween when it's done:
            if (currentTimeMs >= totalTimeMs)
            {
                _onCompletedAction?.Invoke();
                parent = null;
                Destroy();
            }
        }

        void ApplyTween()
        {
            // Get the current tween curve value. Note that currentTimeMs/totalTimeMs is a value between 0 and 1 (or slightly more than 1),
            //  and typical tweening curves have values from 0 to 1, so newCurveValue goes typically from 0 to delta.
            float newCurveValue = GetCurveValue(1f * currentTimeMs / totalTimeMs) * delta;

            // Since the parent game object's properties may simultaneously be influenced by other things as well (other tweens??),
            // we only make relative changes here:
            float outputDelta = newCurveValue - lastCurveValue;

            switch (target)
            {
                case TweenProperty.x:
                    parent.x += outputDelta;
                    break;
                case TweenProperty.y:
                    parent.y += outputDelta;
                    break;
                case TweenProperty.rotation:
                    parent.rotation += outputDelta;
                    break;
            }

            lastCurveValue = newCurveValue;
        }

        protected override void OnDestroy()
        {
            if(!_completed)
            {
                _onExitAction?.Invoke();
                _completed = true;
            }
            _onCompletedAction = null;
            _onExitAction = null;
            _onStartAction = null;
        }
        /// <summary>
        /// Action that gets called when the tween is destroyed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Tween OnExit(Action action)
        {
            _onExitAction = action;
            return this;
        }
        /// <summary>
        /// Action that gets called when the tween finishes the task
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Tween OnCompleted(Action action)
        {
            _onCompletedAction = action;
            return this;
        }
        /// <summary>
        /// Action that gets called before the first iteration of the tween
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Tween OnStart(Action action)
        {
            _onStartAction = action;
            return this;
        }
    }

        
}
