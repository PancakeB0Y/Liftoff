using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Animations
{
    public class Robot3Btns : AnimationManager
    {
        public Robot3Btns(TiledObject data) : base(data)
        {
            byte speed = 1;
            var animationSet = new AnimationSprite("Assets/RobotAnims/Robot_ThreeButtons_SpriteSheet .png", 16, 6, 92, false, false);

            _animations = new Dictionary<string, Animation>()
            {
                {"Idle",    new Animation( this,"Idle", animationSet ,startFrame: 84, frames: 8,animDelay: 3) },
                {"Press1",  new Animation( this,"Press1", animationSet ,startFrame: 46, frames: 23,animDelay: speed, loop: false) },
                {"Press2",  new Animation( this,"Press2", animationSet ,startFrame: 23, frames: 23,animDelay: speed, loop: false) },
                {"Press3",  new Animation( this,"Press3", animationSet ,startFrame: 0, frames: 23,animDelay: speed, loop: false) },
                {"Die",     new Animation( this,"Die", animationSet ,startFrame: 69, frames: 14,animDelay: speed, loop: false,exitTime:744) },

            };

            _animations["Press1"].AnimationLoopEnd += OnTriggerEnd;
            _animations["Press2"].AnimationLoopEnd += OnTriggerEnd;
            _animations["Press3"].AnimationLoopEnd += OnTriggerEnd;

            currAnimation = _animations["Idle"];
            currAnimation.StartAnim();
        }
        protected override void OnDestroy()
        {
            _animations["Press1"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["Press2"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["Press3"].AnimationLoopEnd -= OnTriggerEnd;
        }

        protected override void TriggerFactory()
        {
            if (Input.GetKeyDown(Key.H))
            {
                AddTrigger("Press1", 1);
            }
            else if (Input.GetKeyDown(Key.J))
            {
                AddTrigger("Press2", 1);
            }
            else if (Input.GetKeyDown(Key.K))
            {
                AddTrigger("Press3", 1);
            }

        }
        void Update()
        {
            TriggerFactory();

            DoTriggers();

            currAnimation.Update();
        }
    }
}
