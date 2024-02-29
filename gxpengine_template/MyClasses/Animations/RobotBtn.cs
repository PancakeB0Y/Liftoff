using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses.Animations
{
    public class RobotBtn : AnimationManager
    {
        public RobotBtn(TiledObject data) : base(data)
        {
            //Robot_ThreeButtons_SpriteSheet .png
            byte speed = 1;
            //this can be improved by getting all the data from tiled
            var animationSet = new AnimationSprite("Assets/RobotAnims/Robot_One_Button_SpriteSheet.png", 7, 7, 45, false, false);

            _animations = new Dictionary<string, Animation>()
            {
                {"Idle",    new Animation( this,"Idle", animationSet ,startFrame: 0, frames: 19,animDelay: 3) },
                {"Press", new Animation( this,"Press", animationSet ,startFrame: 26, frames: 19,animDelay: speed, loop: false) },
                {"Die",     new Animation( this,"Die", animationSet ,startFrame: 19, frames: 25,animDelay: speed, loop: false) },

            };

            _animations["Press"].AnimationLoopEnd += OnTriggerEnd;

            currAnimation = _animations["Idle"];
            currAnimation.StartAnim();
        }
        protected override void OnDestroy()
        {
            _animations["Press"].AnimationLoopEnd -= OnTriggerEnd;
        }

        protected override void TriggerFactory()
        {
            if (Input.GetKeyDown(Key.C))
            {
                AddTrigger("Press", 1);
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
