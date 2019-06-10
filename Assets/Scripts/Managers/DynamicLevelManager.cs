using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;



namespace FantasyErrand
{
    public class DynamicLevelManager : MonoBehaviour {

        public Player player;
        public GameManager gameManager;

        [Header("Level Objects")]
        public GameObject startPrefab;
        public GameObject[] straightPrefabs, obstaclePrefabs, powerupsPrefabs, coinPrefabs, overheadObstaclePrefabs;
        public GameObject[] coinCopperPrefabs, coinSilverPrefabs, coinGoldPrefabs, coinPlatinumPrefabs, coinRubyPrefabs;
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
        int currminTilesBeforeOverhead = 0;
        public int obstacleTolerance = 4;
        [Header("Coins")]
        public int maxCoinSpawnPerTile = 6;
        public int minCoins = 4, maxCoins = 24;
        public int minimumCoinLane = 1;

        [Header("Power Ups")]
        public int minTilesBeforeNextPowerUps = 8;
        int currMinTilesBeforeNextPowerUps = 0;

        [Header("Game Tile Probability")]
        public int 


        public EmotionManager emotionManager;




        void Start() {

        }

        // Update is called once per frame
        void Update()
        {
            if(emotionManager.FaceStatus == "Tracking")
            print(emotionManager.EmotionsList[0][Affdex.Emotions.Disgust]);
        }

        public float GetEmotion(Affdex.Emotions emo)
        {
            if (emo.Equals(Affdex.Emotions.Disgust))
                return emotionManager.EmotionsList[0][Affdex.Emotions.Disgust];
            else if (emo.Equals(Affdex.Emotions.Joy))
                return emotionManager.EmotionsList[0][Affdex.Emotions.Joy];
            else
                return 0;
        }
    }
}
