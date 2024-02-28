using GXPEngine;
using gxpengine_template.MyClasses.Coroutines;
using gxpengine_template.MyClasses.UI;
using System.Collections;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{
    public class ScoreManager : Sprite
    {
        public static ScoreManager Instance { get; private set; }
        public int Score 
        { 
            get => _score;
            set 
            { 
                _score = value;
                _textMesh.Text = _score.ToString();
            }
        }
        int _score;
        public string PlayerName { get; private set; } = "stf";
        readonly TextMesh _textMesh;

        public ScoreManager(TiledObject data): base("Assets/square.png",true,false)
        {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;
            alpha =  0f;
            _textMesh = new TextMesh("0000", 100, 100, CenterMode.Min);
            AddChild(new Coroutine(Init()));
        }

        IEnumerator Init()
        {
            yield return null;

            MyUtils.MyGame.CurrentScene.AddChild(_textMesh);
            _textMesh.SetXY(x, y);
        }
        protected override void OnDestroy()
        {
            Instance = null;
        }

    }
}
