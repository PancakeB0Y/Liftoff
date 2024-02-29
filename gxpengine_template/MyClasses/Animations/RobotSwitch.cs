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
        bool _toggled;
        public RobotSwitch(TiledObject data) : base(data)
        {
            //Robot_ThreeButtons_SpriteSheet .png
            byte speed = 1;
            //this can be improved by getting all the data from tiled
            var animationSet = new AnimationSprite("Assets/RobotAnims/Robot_Switch_SpriteSheet.png", 7, 8, 55, false, false);

            _animations = new Dictionary<string, Animation>()
            {
                {"IdleUp",    new Animation( this,"IdleUp", animationSet ,startFrame: 9, frames: 6,animDelay: 3) },
                {"IdleDown",  new Animation( this,"IdleDown", animationSet ,startFrame: 28, frames: 7,animDelay: 3) },
                {"PressUp",   new Animation( this,"PressUp", animationSet ,startFrame: 0, frames: 9,animDelay: speed, loop: false) },
                {"PressDown", new Animation( this,"PressDown", animationSet ,startFrame: 15, frames: 13,animDelay: speed, loop: false) },
                {"DieDown", new Animation( this,"DieDown", animationSet ,startFrame: 35, frames: 10,animDelay: speed, loop: false,exitTime:744) },
                {"DieUp", new Animation( this,"DieUp", animationSet ,startFrame: 45, frames: 10,animDelay: speed, loop: false,exitTime:744) },

            };

            _animations["PressUp"].AnimationLoopEnd += OnUpEnd;
            _animations["PressDown"].AnimationLoopEnd += OnDownEnd;
            _animations["DieUp"].AnimationLoopEnd += SelfDestroy;
            _animations["DieDown"].AnimationLoopEnd += SelfDestroy;


            currAnimation = _animations["IdleDown"];
            currAnimation.StartAnim();
        }
        protected override void OnDestroy()
        {
            _animations["PressUp"].AnimationLoopEnd -= OnUpEnd;
            _animations["PressDown"].AnimationLoopEnd -= OnDownEnd;

        }

        void SelfDestroy()
        {
            _animations["DieUp"].AnimationLoopEnd -= SelfDestroy;
            _animations["DieDown"].AnimationLoopEnd -= SelfDestroy;
            Destroy();
        }

        protected override void OnExlposion()
        {
            if (_toggled)
                AddTrigger("DieUp", 2);
            else
                AddTrigger("DieDown", 2);

            Bomb.Instance.Exploded -= OnExlposion;
        }

        protected override void TriggerFactory()
        {
            if (Input.GetKeyDown(Key.B))
            {
                _toggled = true;
                AddTrigger("PressUp", 1);
            }
            else if (Input.GetKeyUp(Key.B))
            {
                _toggled = false;
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
