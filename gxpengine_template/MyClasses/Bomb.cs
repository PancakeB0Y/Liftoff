using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Bomb : AnimationSprite
    {
        int _timerSeconds;
        public Bomb(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, -1, false, false)
        {
            _timerSeconds = data.GetIntProperty("TimerSeconds", 120);

        }


    }
}
