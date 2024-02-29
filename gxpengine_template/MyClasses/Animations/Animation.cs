using GXPEngine;
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;

namespace gxpengine_template.MyClasses.Animations
{
    public class Animation
    {
        //fires only on the reset of loop, not on animation end
        public event Action AnimationLoopEnd;
        public event Action AnimationExit;

        public string Name { get; }

        readonly AnimationSprite _animSprite;
        readonly int _startFrame;
        readonly int _endFrame;
        //a pause before entering a new animation
        readonly int _exitTime;
        readonly bool _loop;
        readonly byte _animDelay;
        bool _endedAnimation;
        int _currExitTime;
        //context is just a container object

        public Animation(Sprite context, string name, AnimationSprite animSprite, int startFrame, int frames, byte animDelay, bool loop = true, int exitTime = 10)
        {
            context.AddChild(animSprite);

            Name = name;
            _animDelay = animDelay;
            _startFrame = startFrame;
            _endFrame = frames + startFrame;
            _loop = loop;
            _animSprite = animSprite;
            _exitTime = exitTime;
            _currExitTime = exitTime;
        }


        public void Update()
        {

            if (!_endedAnimation && !_loop && _animSprite.currentFrame == _endFrame - 1)
            {
                _currExitTime -= Time.deltaTime;
                if (_currExitTime > 0) return;// prevents calling multiple times endAnim

                EndAnim();
                AnimationLoopEnd?.Invoke();
                return;
            }

            _animSprite.AnimateFixed();

        }

        public void StartAnim()
        {
            _animSprite.SetCycle(_startFrame, _endFrame - _startFrame, _animDelay);
            _animSprite.SetOrigin(_animSprite.width / 2, _animSprite.height / 2);
            _animSprite.visible = false;


            _endedAnimation = false;
            _animSprite.SetFrame(_startFrame);
            _animSprite.visible = true;///////change this to set cycle?
            _currExitTime = _exitTime;
        }

        public void EndAnim()
        {
            AnimationExit?.Invoke();
            _endedAnimation = true;
            _animSprite.visible = false;
        }

        public void Mirror(bool horizontally, bool vertically)
        {
            _animSprite.Mirror(horizontally, vertically);
        }
    }
}