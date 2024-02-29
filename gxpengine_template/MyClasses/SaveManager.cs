
using GXPEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{

    public class SaveManager : Sprite
    {
        public static SaveManager Instance { get; private set; }
        public string PlayerName { get; private set; } = "stf";

        public SaveManager(TiledObject data) : base("Assets/square.png",true,false)
        {
            if (Instance != null)
            {
                Destroy();
            }
            else
                Instance = this;

            alpha = 0f;
        }

        public int GetHighScore()
        {
            string input = "";

            using (StreamReader reader = new StreamReader("Assets/HighScores.txt"))
            {
                var currLine = reader.ReadLine();
                if (currLine != null)
                    input += currLine;

                reader.Close();
            }
            string[] nameScorePair = input.Split(',');
            string prevScore = nameScorePair.FirstOrDefault(pair => pair.Substring(0, 3) == PlayerName);
            string intStr = prevScore.Substring(4, prevScore.Length - 4);

            return int.Parse(intStr);
        }

        public void SaveHighScore(int score)
        {
            string input = "";
            
            using (StreamReader reader = new StreamReader("Assets/HighScores.txt"))
            {
                var currLine = reader.ReadLine();
                if (currLine != null) 
                    input += currLine;


                reader.Close();
            }
            
            //edit them in input variable;
            using (StreamWriter writer = new StreamWriter("Assets/HighScores.txt"))
            {
                if(string.IsNullOrEmpty(input))
                {
                    input += "," + PlayerName + ":" + score;
                }
                else
                {
                    string[] nameScorePair = input.Split(',');
                    string prevScorePair = nameScorePair.FirstOrDefault(pair => pair.Substring(0, 3) == PlayerName);
                    if (prevScorePair != null)
                    {
                        int prevScore = int.Parse(prevScorePair.Substring(4, prevScorePair.Length - 4));
                        if (prevScore < score)
                            input = input.Replace(prevScorePair, PlayerName + ":" + score);
                    }
                    else
                        input += "," + PlayerName + ":" + score ;
                }
                input = input.TrimStart(',');
                writer.Write(input);
                writer.Close();
            }
        }

        protected override void OnDestroy()
        {
            Instance = null;
        }
    }

}
