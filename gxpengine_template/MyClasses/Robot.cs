using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Robot : AnimationSprite
    {
        public Robot(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows)
        {

        }

        void Update()
        {
            AnimateFixed();
        }
    }
}
