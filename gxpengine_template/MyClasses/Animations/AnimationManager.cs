using GXPEngine;
using System.Collections.Generic;
using System.Linq;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Animations
{
    public abstract class AnimationManager : Sprite
    {
        //triggers are just a tool to easily activate animations based on priority
        protected readonly struct Trigger
        {
            public readonly string AnimName;
            public readonly byte Priority;

            public Trigger(string animName, byte priority)
            {
                AnimName = animName;
                Priority = priority;
            }
        }
        protected Dictionary<string, Animation> _animations;
        protected readonly List<Trigger> triggers = new List<Trigger>();
        protected Animation currAnimation;
        protected Trigger? prevTrigger;

        protected AnimationManager(TiledObject data ) : base("Assets/square.png",true,false)
        {
            alpha = 0;
        }

        protected void TransitionToAnim(Animation newAnim)
        {
            currAnimation.EndAnim();
            currAnimation = newAnim;
            currAnimation.StartAnim();
        }

        protected void AddTrigger(string name, byte priority)
        {
            if (!triggers.Any(x => x.AnimName == name))
                triggers.Add(new Trigger(name, priority));
        }
        protected virtual void TriggerFactory() { }

        protected void DoTriggers()
        {
            if (triggers.Count == 0) return;//activates triggered animation with the most priority
            
            Trigger maxPriorityTrigger = triggers.Aggregate((i, j) => i.Priority > j.Priority ? i : j);

            if (prevTrigger == null || prevTrigger.Value.Priority <= maxPriorityTrigger.Priority)
            {
                TransitionToAnim(_animations[maxPriorityTrigger.AnimName]);
                triggers.Remove(maxPriorityTrigger);
                prevTrigger = maxPriorityTrigger;
            }

        }

        protected virtual void OnTriggerEnd()
        {
            TransitionToAnim(_animations["Idle"]);
            prevTrigger = null;

        }
    }
}
