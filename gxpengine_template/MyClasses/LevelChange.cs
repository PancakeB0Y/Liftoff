using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class LevelChange : Sprite
    {
        string _nextLevel;
        public LevelChange(TiledObject data) : base("Assets/square.png", true, false)
        {
            alpha = 0f;

            _nextLevel = data.GetStringProperty("NextLevel", "Assets/LVL2.tmx");
        }

        void HandleInputs()
        {
            if (Input.AnyKeyDown() && !Input.GetKey(Key.R))
            {
                ((MyGame)game).LoadScene(_nextLevel);
            }
        }

        void Update()
        {
            HandleInputs();
        }



    }
}
