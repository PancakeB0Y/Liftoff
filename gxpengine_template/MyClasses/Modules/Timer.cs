using GXPEngine;

namespace gxpengine_template.MyClasses.Modules
{
    public class Timer : GameObject
    {
        readonly EasyDraw _fill;
        readonly Sprite _bg;
        public Timer(string filename, bool keepInCache = false, bool addCollider = true)
        {
            _bg = new Sprite(filename, keepInCache, addCollider);
            _fill = new EasyDraw(_bg.width, _bg.height, false);
            _fill.Clear(60, 118, 22);
            AddChild(_fill);
            AddChild(_bg);
            _fill.SetOrigin(0, _fill.height);
            _fill.SetXY(0, _fill.height);
        }

        public void SetPersentage(float t)
        {
            _fill.height = (int)(_bg.height * t);
        }

        public void SetAlpha(float t)
        {
            _fill.alpha = t;
            _bg.alpha = t;
        }
    }
}
