using GXPEngine;
using System.Collections.Generic;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Animations
{
    public class Robot_DPad : AnimationManager
    {

        public Robot_DPad(TiledObject data) : base(data)
        {

            byte speed = 2;
            //this can be improved by getting all the data from tiled
            var animationSet = new AnimationSprite("Assets/RobotAnims/Robot_Dpad_Sprite.png", 10, 10, 97, false, false);

            _animations = new Dictionary<string, Animation>()
            {
                {"Idle",    new Animation( this,"Idle", animationSet ,startFrame: 81, frames: 15,animDelay: 3,exitTime:0) },
                {"L_Press", new Animation( this,"L_Press", animationSet ,startFrame: 0, frames: 17,animDelay: speed, loop: false) },
                {"T_Press", new Animation( this,"T_Press", animationSet ,startFrame: 37, frames: 13,animDelay: speed, loop: false) },
                {"R_Press", new Animation( this,"R_Press", animationSet ,startFrame: 50, frames: 17,animDelay: speed, loop: false) },
                {"B_Press", new Animation( this,"B_Press", animationSet ,startFrame: 17, frames: 9,animDelay: speed, loop: false) },
                {"M_Press", new Animation( this,"M_Press", animationSet ,startFrame: 26, frames: 10,animDelay: speed, loop: false) },
                {"Die",     new Animation( this,"Die", animationSet ,startFrame: 67, frames: 14,animDelay: speed, loop: false,exitTime:755) },

            };

            _animations["L_Press"].AnimationLoopEnd += OnTriggerEnd;
            _animations["T_Press"].AnimationLoopEnd += OnTriggerEnd;
            _animations["R_Press"].AnimationLoopEnd += OnTriggerEnd;
            _animations["B_Press"].AnimationLoopEnd += OnTriggerEnd;
            _animations["M_Press"].AnimationLoopEnd += OnTriggerEnd;

            currAnimation = _animations["Idle"];
            currAnimation.StartAnim();

        }

        protected override void TriggerFactory()
        {
            if (Input.GetKeyDown(Key.A))
            {
                AddTrigger("L_Press", 1);
            }
            else if (Input.GetKeyDown(Key.W))
            {
                AddTrigger("T_Press", 1);
            }
            else if (Input.GetKeyDown(Key.D))
            {
                AddTrigger("R_Press", 1);
            }
            else if (Input.GetKeyDown(Key.S))
            {
                AddTrigger("B_Press", 1);
            }
            else if (Input.GetKeyDown(Key.SPACE))
            {
                AddTrigger("M_Press", 1);
            }
        }

        protected override void OnDestroy()
        {
            _animations["L_Press"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["T_Press"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["R_Press"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["B_Press"].AnimationLoopEnd -= OnTriggerEnd;
            _animations["M_Press"].AnimationLoopEnd -= OnTriggerEnd;
        }

        void Update()
        {
            TriggerFactory();
            
            DoTriggers();

            currAnimation.Update();
        }

    }
}
