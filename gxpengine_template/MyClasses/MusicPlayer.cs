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
    public class MusicPlayer : Sprite
    {
        readonly string _musicFile;
        readonly bool _loop;
        readonly float _volume;

        readonly SoundChannel _music;
        public MusicPlayer(TiledObject data) : base("Assets/square.png", true, false)
        {
            alpha = 0f;

            _musicFile = data.GetStringProperty("MusicFile", "Assets/Sounds/MainTheme.wav");
            _loop = data.GetBoolProperty("Loop", true);
            _volume = data.GetFloatProperty("Volume", 1);

            _music = new Sound(_musicFile, _loop).Play(false, 0, _volume);
            ((MyGame)game).StopSounds += StopMusic;
        }

        public void StopMusic()
        {
            _music.Stop();
        }
    }
}
