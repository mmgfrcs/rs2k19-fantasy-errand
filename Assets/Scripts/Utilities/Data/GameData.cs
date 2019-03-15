using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    public class GameData
    {
        public string PlayerName { get; internal set; }
        public GameRecord Record { get; private set; }
        
        public void SetRecord(GameRecord record)
        {

        }
    }

    public class GameRecord
    {
        public int Highscore { get; private set; }
        public int LongestDistance { get; private set; }
        public int MostCoinsCollected { get; private set; }
        public int RecordMultiplier { get; private set; }

        public GameRecord(int highscore, int longestDist, int mostCoins, int recordMult)
        {
            Highscore = highscore;
            LongestDistance = longestDist;
            MostCoinsCollected = mostCoins;
            RecordMultiplier = recordMult;
        }

        public void SetNewHighScore(int highscore, int longestDist, int mostCoins, int recordMult)
        {
            if(highscore > Highscore)
            {
                Highscore = highscore;
                LongestDistance = longestDist;
                MostCoinsCollected = mostCoins;
                RecordMultiplier = recordMult;
            }
        }
    }
}

