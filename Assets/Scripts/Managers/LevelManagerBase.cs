using FantasyErrand;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand
{
    public enum ObstacleType
    {
        Spike = 8, Boulder, Hurdling, Wall
    }
    public enum TileKey
    {
        Straight, Overhead, Wall, Boulder, Spike, Hurdling, CoinCopper, CoinSilver, CoinGold, CoinRuby, CoinPlatinum, PotionPhase, PotionBoost, PotionGold, PotionMagnet
    }
    public enum TileType
    {
        Coin, Powerups, Tile, Obstacle
    }

    public enum Difficulty
    {
        Easy, Hard, Special
    }

    public class LevelManagerBase : MonoBehaviour
    {
        public Player player;
        public GameManager gameManager;
        public Difficulty difficulty;

        [Header("Level Objects")]
        public GameObject startPrefab;
        public GameObject[] straightPrefabs, overheadObstaclePrefabs, spikePrefabs, boulderPrefabs, hurdlingPrefabs, wallPrefabs;
        public GameObject coinCopperPrefabs, coinSilverPrefabs, coinGoldPrefabs, coinPlatinumPrefabs, coinRubyPrefabs, potionPhasePrefabs, potionBoostPrefabs, potionGoldPrefabs, potionMagnetPrefabs;
        public Vector3 startPosition;
        
        [Header("Tile Generation")]
        public bool gameMode = true;
        public int maxGeneratedTile = 10, pooledObstacles = 20, pooledCoins = 40;
        public int startTiles = 2;
        public TileSpawnRates tileSpawnRates;
        public float tileScale;

        [Header("Obstacles")]
        public float baseObstacleRatio = 100;
        public int minimumObstacleLane = 1;
        public int minTilesBeforeOverhead = 8;
        public int obstacleTolerance = 4;

        [Header("Coins")]
        public int maxCoinSpawnPerTile = 6;
        public int minCoins = 4, maxCoins = 24;
        public int minimumCoinLane = 1;

        [Header("Power Ups")]
        public int minTilesBeforeNextPowerUps = 8;
        protected int currMinTilesBeforeNextPowerUps = 0;

        [Header("Debug")]
        public bool showGizmos = true;
        public bool showDebugText = true;

        [Header("Game Balancing")]
        public AnimationCurve CoinLane;
        public AnimationCurve ObstacleLane;

        protected List<GameObject> startObjects = new List<GameObject>();
        protected List<GameObject> spawnedObjects = new List<GameObject>();
        protected bool initialized = false;
        protected int patternSpawned;
        protected int coinRemaining = 0;
        protected float continueCoinAt = -99;
        protected Dictionary<TileKey, ObjectPooler> poolDictionary = new Dictionary<TileKey, ObjectPooler>();
        protected bool turnGoldenCoin = false;
        protected float coinXLastPos = 1.5f;
        protected int currminTilesBeforeOverhead = 0;

        //<Coin Property Value>
        protected int coinValueLevel = 0;
        public float silverDistance = 0;
        public float goldDistance = 0;
        public float platinumDistance = 0;

        internal float coinMod = 0;
        internal float obstacleMod = 0;
        protected float coinAmountMod = 1;
        protected float obstacleAmountMod = 1;



        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        internal float GetTileRate(TileType tiles)
        {
            float i = 0;
            if (tiles.Equals(TileType.Coin))
            {
                i = tileSpawnRates.coinsTile.Evaluate(player.transform.position.z) + coinMod;
                return i;
            }

            else if (tiles.Equals(TileType.Obstacle))
            {
                i = tileSpawnRates.obstacleTile.Evaluate(player.transform.position.z) + obstacleMod;
                return i;
            }

            else if (tiles.Equals(TileType.Powerups))
            {
                i = tileSpawnRates.powerupsTile.Evaluate(player.transform.position.z);
                return i;
            }
            else
            {
                i = tileSpawnRates.baseTile.Evaluate(player.transform.position.z);
                return i;
            }
        }

        public virtual float GetCoinLane()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(CoinLane.Evaluate(distance));
            return temp;
        }

        public virtual float GetObstacleLane()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(ObstacleLane.Evaluate(distance));
            return temp;
        }

        private float GetMinObs()
        {
            float temp = minimumObstacleLane*obstacleAmountMod;
            return temp;
        }

        private float GetMaxObs()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(ObstacleLane.Evaluate(distance)*obstacleAmountMod);
            return temp;
        }

        private float GetMinCoins()
        {
            float temp = minimumCoinLane * coinAmountMod;
            return temp;
        }

        private float GetMaxCoins()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(CoinLane.Evaluate(distance) * coinAmountMod);
            return temp;
        }

        public float getNeutral(Affdex.Emotions[] type)
        {
            float totalEmo = 0;
            if (GameDataManager.instance.NeutralData.emotions != null)
            {
                for (int i = 0; i < type.Length; i++)
                {
                    totalEmo += GameDataManager.instance.NeutralData.emotions[type[i].ToString()];
                }
            }
            
            return totalEmo / (type.Length * 100);
        }

    } 
}
