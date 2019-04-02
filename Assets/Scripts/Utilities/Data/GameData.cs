using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    public class GameData
    {
        public string PlayerName { get; internal set; }
        public int Coins { get; internal set; }
        public UpgradeLevels UpgradeLevels { get; internal set; }
        public float CumulativeScore { get; internal set; }
        public float CumulativeDistance { get; internal set; }
        public float CumulativeCoins { get; internal set; }
        public int CumulativeRuns { get; internal set; }
        public GameRecord Record { get; private set; }

        public GameData(string name)
        {
            PlayerName = name;
            UpgradeLevels = new UpgradeLevels();
            Coins = 0;
            Record = null;
        }

        public void SetRecord(GameRecord record)
        {
            if (Record == null || record.Highscore > Record.Highscore)
            {
                Record = record;
            }
        }
    }

    public class UpgradeLevels
    {
        public int MagnetLevel { get; private set; }
        public int PhaseLevel { get; private set; }
        public int BoostLevel { get; private set; }
        public int GoldenCoinLevel { get; private set; }
        public int CoinValueLevel { get; private set; }
        public int LivesLevel { get; private set; }
        public int MultiplierLevel { get; private set; }

        public void MagnetLevelUp() => MagnetLevel++;
        public void PhaseLevelUp() => PhaseLevel++;
        public void BoostLevelUp() => BoostLevel++;
        public void GoldenCoinLevelUp() => GoldenCoinLevel++;
        public void CoinValueLevelUp() => CoinValueLevel++;
        public void LivesLevelUp() => LivesLevel++;
        public void MultiplierLevelUp() => MultiplierLevel++;
    }

    public class GameRecord
    {
        public float Highscore { get; private set; }
        public float LongestDistance { get; private set; }
        public int MostCoinsCollected { get; private set; }
        public int RecordMultiplier { get; private set; }

        public GameRecord(float highscore, float longestDist, int mostCoins, int recordMult)
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

