using GXPEngine;


namespace gxpengine_template.MyClasses
{
    public class MyGame : Game
    {
        public MyGame() : base(600,600,false)
        {
        }
        static void Main()
        {
            new MyGame().Start();
        }

    }
}
