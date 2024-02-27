
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace gxpengine_template.MyClasses
{

    public class SaveManager
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
        public void SaveHighScore()
        {
            int score = ScoreManager.Instance.Score;
            string input = "";
            
            using (StreamReader reader = new StreamReader("Assets/HighScores.txt"))
            {
                var currLine = reader.ReadLine();
                if (currLine == null) return;

                input += currLine;

                reader.Close();
            }
            
            //edit them in input variable;
            using (StreamWriter writer = new StreamWriter("Assets/HighScores.txt"))
            {
                var playerName = ScoreManager.Instance.PlayerName;
                string[] nameScorePair = input.Split(',');

                string prevScore = nameScorePair.FirstOrDefault(pair => pair.Substring(0, 3) == playerName);
                
                if(prevScore == null) 
                {
                    input += playerName + ":" + score;  
                }
                else
                {
                    input.Replace(prevScore, playerName + ":" + score);
                }


                writer.Write(input);
                writer.Close();
            }
        }
    }

}
