using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Animations
{
    public class RobotSwitch : AnimationManager
    {
        bool toggled;
        public RobotSwitch(TiledObject data) : base(data)
        {
            //Robot_ThreeButtons_SpriteSheet .png
            byte speed = 1;
            //this can be improved by getting all the data from tiled
            var animationSet = new AnimationSprite("Assets/RobotAnims/Robot_Switch_SpriteSheet.png", 8, 6, 43, false, false);

            _animations = new Dictionary<string, Animation>()
            {
                {"IdleUp",    new Animation( this,"IdleUp", animationSet ,startFrame: 0, frames: 19,animDelay: 3) },
                {"IdleDown",  new Animation( this,"IdlIdleDown", animationSet ,startFrame: 0, frames: 19,animDelay: 3) },
                {"PressUp",   new Animation( this,"PressUp", animationSet ,startFrame: 26, frames: 19,animDelay: speed, loop: false) },
                {"PressDown", new Animation( this,"PressDown", animationSet ,startFrame: 26, frames: 19,animDelay: speed, loop: false) },

            };

            _animations["PressUp"].AnimationLoopEnd += OnUpEnd;
            _animations["PressDown"].AnimationLoopEnd += OnDownEnd;

            currAnimation = _animations["IdleDown"];
            currAnimation.StartAnim();
        }
        protected override void OnDestroy()
        {
            _animations["PressUp"].AnimationLoopEnd -= OnUpEnd;
            _animations["PressDown"].AnimationLoopEnd -= OnDownEnd;
        }

        protected override void TriggerFactory()
        {
            if (Input.GetKeyDown(Key.B))
            {
                toggled = true;
                AddTrigger("PressUp", 1);
            }
            else if (Input.GetKeyUp(Key.B))
            {
                toggled = false;
                AddTrigger("PressDown", 1);

            }

        }

        protected override void OnTriggerEnd()
        {

        }
        void OnUpEnd()
        {
            prevTrigger = null;
            TransitionToAnim(_animations["IdleUp"]);

        }

        void OnDownEnd()
        {
            prevTrigger = null;
            TransitionToAnim(_animations["IdleDown"]);
        }

        void Update()
        {
            TriggerFactory();

            DoTriggers();

            currAnimation.Update();
        }
    }
}
