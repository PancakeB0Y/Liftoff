using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gxpengine_template.MyClasses
{
    public class ScoreManager : GameObject
    {
        public static ScoreManager Instance { get; private set; }
        public int Score { get; set; }

        public ScoreManager() {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;
        }

        
    }
}
