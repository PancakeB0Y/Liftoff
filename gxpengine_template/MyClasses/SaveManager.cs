
using GXPEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TiledMapParser;

namespace gxpengine_template.MyClasses
{

    public class SaveManager : Sprite, IStartable
    {
        public enum DifficultyLevels
        {
            S,
            A,
            B,
            C,
            D,
            E,
            F
        }
        public string PlayerName { get; private set; } = "stf";

        public SaveManager(TiledObject data) : base("Assets/square.png",true,false)
        {
            alpha = 0f;
        }



        public void SaveHighScore()
        {
            int score = 0;
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
                    string prevScore = nameScorePair.FirstOrDefault(pair => pair.Substring(0, 3) == PlayerName);
                    if (prevScore != null)
                        input = input.Replace(prevScore, PlayerName + ":" + score);
                    else
                        input += "," + PlayerName + ":" + score ;
                }
                input = input.TrimStart(',');
                writer.Write(input);
                writer.Close();
            }
        }

        public void Start()
        {

            SaveHighScore();
        }
    }

}
