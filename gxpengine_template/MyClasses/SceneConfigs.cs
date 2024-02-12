
using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public abstract class SceneConfigs : AnimationSprite
    {

        protected Level level;

        public SceneConfigs(TiledObject data) : base("square.png", 1, 1, -1, true, false)
        {
            visible = false;
        }

        public void Init(Level level)
        {
            this.level = level;
            Initialize(level);
        }

        protected abstract void Initialize(Level level);

    }
}
