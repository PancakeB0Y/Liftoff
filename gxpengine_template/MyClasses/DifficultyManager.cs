using GXPEngine;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class DifficultyManager : Sprite
    {
        public static DifficultyManager Instance { get; private set; }
        public int Difficulty { get; set; } = 0;
        public DifficultyManager(TiledObject data) : base("Assets/square.png",true,false)
        {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;

        }
        protected override void OnDestroy()
        {
            Instance = null;
        }
    }
}
