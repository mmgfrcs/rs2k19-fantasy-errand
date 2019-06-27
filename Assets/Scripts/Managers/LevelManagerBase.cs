﻿using FantasyErrand;
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

        public bool EasyTrigger = false;
        public bool HardTrigger = false;
        public bool CleverTrigger = false;
        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
    } 
}