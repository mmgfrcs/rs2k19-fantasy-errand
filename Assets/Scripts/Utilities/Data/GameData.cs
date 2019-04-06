using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Utilities
{
    [Serializable]
    public class GameData
    {
        [SerializeField]
        private string _playerName;
        [SerializeField]
        private int _coins;
        [SerializeField]
        private UpgradeLevels _upgradeLevels;
        [SerializeField]
        private float _cumulativeScore;
        [SerializeField]
        private float _cumulativeDistance;
        [SerializeField]
        private float _cumulativeCoins;
        [SerializeField]
        private int _cumulativeRuns;
        [SerializeField]
        private GameRecord _record;

        public string PlayerName
        {
            get
            {
                return _playerName;
            }
            internal set
            {
                _playerName = value;
            }
        }
        public int Coins
        {
            get
            {
                return _coins;
            }
            internal set
            {
                _coins = value;
            }
        }
        public UpgradeLevels UpgradeLevels
        {
            get
            {
                return _upgradeLevels;
            }
            internal set
            {
                _upgradeLevels = value;
            }
        }
        public float CumulativeScore
        {
            get
            {
                return _cumulativeScore;
            }
            internal set
            {
                _cumulativeScore = value;
            }
        }
        public float CumulativeDistance
        {
            get
            {
                return _cumulativeDistance;
            }
            internal set
            {
                _cumulativeDistance = value;
            }
        }
        public float CumulativeCoins
        {
            get
            {
                return _cumulativeCoins;
            }
            internal set
            {
                _cumulativeCoins = value;
            }
        }
        public int CumulativeRuns
        {
            get
            {
                return _cumulativeRuns;
            }
            internal set
            {
                _cumulativeRuns = value;
            }
        }
        public GameRecord Record
        {
            get
            {
                return _record;
            }
            private set
            {
                _record = value;
            }
        }

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

    [Serializable]
    public class UpgradeLevels
    {
        [SerializeField]
        private int _magnetLevel;
        [SerializeField]
        private int _phaseLevel;
        [SerializeField]
        private int _boostLevel;
        [SerializeField]
        private int _goldenCoinLevel;
        [SerializeField]
        private int _coinValueLevel;
        [SerializeField]
        private int _livesLevel;
        [SerializeField]
        private int _multiplierLevel;

        public int MagnetLevel
        {
            get
            {
                return _magnetLevel;
            }
            private set
            {
                _magnetLevel = value;
            }
        }
        public int PhaseLevel
        {
            get
            {
                return _phaseLevel;
            }
            private set
            {
                _phaseLevel = value;
            }
        }
        public int BoostLevel
        {
            get
            {
                return _boostLevel;
            }
            private set
            {
                _boostLevel = value;
            }
        }
        public int GoldenCoinLevel
        {
            get
            {
                return _goldenCoinLevel;
            }
            private set
            {
                _goldenCoinLevel = value;
            }
        }
        public int CoinValueLevel
        {
            get
            {
                return _coinValueLevel;
            }
            private set
            {
                _coinValueLevel = value;
            }
        }
        public int LivesLevel
        {
            get
            {
                return _livesLevel;
            }
            private set
            {
                _livesLevel = value;
            }
        }
        public int MultiplierLevel
        {
            get
            {
                return _multiplierLevel;
            }
            private set
            {
                _multiplierLevel = value;
            }
        }

        public void MagnetLevelUp() => MagnetLevel++;
        public void PhaseLevelUp() => PhaseLevel++;
        public void BoostLevelUp() => BoostLevel++;
        public void GoldenCoinLevelUp() => GoldenCoinLevel++;
        public void CoinValueLevelUp() => CoinValueLevel++;
        public void LivesLevelUp() => LivesLevel++;
        public void MultiplierLevelUp() => MultiplierLevel++;
    }

    [Serializable]
    public class GameRecord
    {
        [SerializeField]
        private float _highscore;
        [SerializeField]
        private float _longestDistance;
        [SerializeField]
        private int _mostCoinsCollected;
        [SerializeField]
        private int _recordMultiplier;

        public float Highscore
        {
            get
            {
                return _highscore;
            }
            private set
            {
                _highscore = value;
            }
        }
        public float LongestDistance
        {
            get
            {
                return _longestDistance;
            }
            private set
            {
                _longestDistance = value;
            }
        }
        public int MostCoinsCollected
        {
            get
            {
                return _mostCoinsCollected;
            }
            private set
            {
                _mostCoinsCollected = value;
            }
        }
        public int RecordMultiplier
        {
            get
            {
                return _recordMultiplier;
            }
            private set
            {
                _recordMultiplier = value;
            }
        }

        public GameRecord(float highscore, float longestDist, int mostCoins, int recordMult)
        {
            Highscore = highscore;
            LongestDistance = longestDist;
            MostCoinsCollected = mostCoins;
            RecordMultiplier = recordMult;
        }

        public void SetNewHighScore(int highscore, int longestDist, int mostCoins, int recordMult)
        {
            if (highscore > Highscore)
            {
                Highscore = highscore;
                LongestDistance = longestDist;
                MostCoinsCollected = mostCoins;
                RecordMultiplier = recordMult;
            }
        }
    }
}

