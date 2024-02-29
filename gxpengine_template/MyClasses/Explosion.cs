using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class Explosion : AnimationSprite, IPrefab
    {
        readonly TiledObject _data;
        public Explosion(string filename, int cols, int rows, TiledObject data) : base(filename, cols, rows, cols*rows, false, false)
        {
            _data = data;
            SetCycle(0, cols * rows, (byte)data.GetIntProperty("AnimSpeed", 5));
        }

        public GameObject Clone()
        {
            var clone = new Explosion(texture.filename, _cols, _rows, _data);
            clone.SetOrigin(width / 2, height / 2);
            return clone;
        }

        void Update()
        {
            AnimateFixed();

            if(currentFrame == frameCount - 1)
            {
                MyUtils.MyGame.ReloadScene();

                Destroy();
            }
        }
    }
}
