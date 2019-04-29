﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;

public class TestingLevelManager : MonoBehaviour {

    

    public Player player;
    public GameObject gameManager;

    [Header("Level Objects")]
    public GameObject startPrefab;
    public GameObject[] straightPrefabs, obstaclePrefabs, powerupsPrefabs, coinPrefabs;
    public Vector3 startPosition;

    [Header("Tile Generation")]
    public bool gameMode = true;
    public int maxGeneratedTile = 10, pooledObstacles = 20, pooledCoins = 40;
    public int startTiles = 2;
    public TileSpawnRates tileSpawnRates;
    public float tileScale;

    [Header("Obstacles")]
    public float baseObstacleRatio = 100;

    [Header("Coins")]
    public int maxCoinSpawnPerTile = 6;
    public int minCoins = 4, maxCoins = 24;

    [Header("Power Ups")]
    public int minTilesBeforeNextPowerUps = 8;
    int currMinTilesBeforeNextPowerUps=0;
    [Header("Debug")]
    public bool showGizmos = true;


    [Header("Game Balancing")]
    public AnimationCurve CoinLane;
    public AnimationCurve ObstacleLane;
    

    //public UnityEngine.UI.Text text;
    
    List<ObjectPooler> poolers = new List<ObjectPooler>();
    List<GameObject> startObjects = new List<GameObject>();
    [SerializeField]
    List<GameObject> spawnedObjects = new List<GameObject>();
    bool initialized = false;
    int patternSpawned;
    int coinRemaining = 0;
    float continueCoinAt = -99;

    float coinXLastPos=1.5f;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitialGeneration());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < startObjects.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, startObjects[i].transform.position) > maxGeneratedTile * tileScale)
            {
                Destroy(startObjects[i]);
                startObjects.RemoveAt(i);
            }
        }
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, spawnedObjects[i].transform.position) > maxGeneratedTile * tileScale*1.25)
            {
                //Check tile type by GetComponent
                if (spawnedObjects[i].GetComponent<IObstacle>() != null)
                    poolers[1].Destroy(spawnedObjects[i]);
                else
                {
                    ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                    if (collect != null)
                    {
                        if (collect.Type == CollectibleType.Monetary) poolers[2].Destroy(spawnedObjects[i]);
                        else poolers[3].Destroy(spawnedObjects[i]);
                    }
                    else poolers[0].Destroy(spawnedObjects[i]);
                }
                spawnedObjects.RemoveAt(i);
            }
        }
        if (initialized && Vector3.Distance(startPosition, player.transform.position) < tileScale * maxGeneratedTile)
        {
            Generate(startPosition);
            startPosition += Vector3.forward * tileScale;
        }
    }

    IEnumerator InitialGeneration()
    {
        currMinTilesBeforeNextPowerUps = minTilesBeforeNextPowerUps;
        int tiles = 0;
        Vector3 spawnPos = startPosition;

        ObjectPooler pooler = gameObject.AddComponent<ObjectPooler>();
        pooler.Initialize(maxGeneratedTile * 3, straightPrefabs);
        poolers.Add(pooler);

        if (gameMode)
        {
            pooler = gameObject.AddComponent<ObjectPooler>();
            pooler.Initialize(maxGeneratedTile * 3, obstaclePrefabs);
            poolers.Add(pooler);

            pooler = gameObject.AddComponent<ObjectPooler>();
            pooler.Initialize(maxGeneratedTile * 3, coinPrefabs);
            poolers.Add(pooler);

            pooler = gameObject.AddComponent<ObjectPooler>();
            pooler.Initialize(maxGeneratedTile * 3, powerupsPrefabs);
            poolers.Add(pooler);
        }

        while (tiles < maxGeneratedTile)
        {
            if (tiles < startTiles)
            {
                GameObject obj = Instantiate(startPrefab, new Vector3(spawnPos.x, -0.5f, spawnPos.z), Quaternion.identity);
                startObjects.Add(obj);
            }
            else
            {
                if (gameMode)
                {
                    print("GameMode On ");
                    Generate(spawnPos);
                }
                else GenerateStraights(new Vector3(spawnPos.x, -0.5f, spawnPos.z));
            }
            tiles++;
            spawnPos += Vector3.forward * tileScale;
            yield return new WaitForEndOfFrame();
        }

        startPosition = spawnPos;
        initialized = true;
    }

    public void Generate(Vector3 spawnPos)
    {

        int opt=0;
        do
        {
            opt = pickTile();
        } while (opt == 3 && currMinTilesBeforeNextPowerUps != 0);

        if (opt == 3 && currMinTilesBeforeNextPowerUps == 0)
            currMinTilesBeforeNextPowerUps = minTilesBeforeNextPowerUps;

        GenerateStraights(new Vector3(spawnPos.x, -0.5f, spawnPos.z));
        float spawnX = MathRand.Pick(new float[] { -3, -1.5f, 0, 1.5f, 3 });
        if (coinRemaining != 0)
        {
            GenerateCoins(new Vector3(continueCoinAt, 0.5f, spawnPos.z), coinRemaining);
        }
        else
        {
            if (opt == 1)
            {
                int n = Random.Range(1, GetObstacleLane());
                GenerateObstacles(new Vector3(spawnX, 0.25f, spawnPos.z), n);
            }
            else if (opt == 2)
            {
                setCoinXPos();
                int n = Random.Range(minCoins, maxCoins + 1);
                int lanenumber = Random.Range(1, GetCoinLane());
                GenerateCoins(new Vector3(coinXLastPos, 0.5f, spawnPos.z), n, lanenumber);
            }
            else if (opt == 3) {
                int n = Random.Range(1, 3);
                GeneratePowerUps(new Vector3(spawnPos.x, 1f, spawnPos.z), n);
            }
        }
        if ((opt == 1 || opt == 2) && currMinTilesBeforeNextPowerUps != 0)
            currMinTilesBeforeNextPowerUps--;
     }

    

    public void GenerateStraights(Vector3 pos)
    {
        spawnedObjects.Add(poolers[0].Instantiate(pos));
    }


    public void GenerateObstacles(Vector3 pos)
    {
        GameObject go = poolers[1].Instantiate(pos);
        spawnedObjects.Add(go);
    }

    public void GenerateObstacles(Vector3 pos, int amount)
    {
        if (amount == 1)
        {
            GenerateObstacles(pos);
        }
        else
        {
            float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
            MathRand.Shuffle(ref mypos);
            for (int i = 0; i < amount; i++)
            {
                GameObject go = poolers[1].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                spawnedObjects.Add(go);
            }
        }
    }

 

    public void GeneratePowerUps(Vector3 pos, int amount)
    {
        if (amount == 1)
        {
            GameObject go = poolers[3].Instantiate(pos);
            spawnedObjects.Add(go);
        }
        else
        {
            float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
            MathRand.Shuffle(ref mypos);
            for (int i = 0; i < amount; i++)
            {

                if (i == 0)
                {
                    GameObject go = poolers[3].Instantiate(new Vector3(mypos[i],pos.y,pos.z));
                    spawnedObjects.Add(go);
                }
                else
                {
                    GenerateObstacles(new Vector3(mypos[i], pos.y, pos.z));                    
                }
            }

        }
    }


    public void setCoinXPos() {
        if (coinXLastPos > -3 && coinXLastPos < 3)
        {
            float temp = Random.Range(1, 10);
            if (temp <= 5)
                coinXLastPos = coinXLastPos - 1.5f;
            else
                coinXLastPos = coinXLastPos + 1.5f;

        }
        else
        {
            if(coinXLastPos==-3)
                coinXLastPos = coinXLastPos + 1.5f;
            else
                coinXLastPos = coinXLastPos - 1.5f;
        }
        
    }

    public void GenerateCoins(Vector3 pos, int n)
    {
       
        float step = tileScale / (Mathf.Min(maxCoinSpawnPerTile, n) + 1);
        float start = pos.z;
        int i = 1;
        for (; i <= Mathf.Min(maxCoinSpawnPerTile, n); i++)
        {
            GameObject go = poolers[2].Instantiate(new Vector3(pos.x, pos.y, start));
            spawnedObjects.Add(go);
            start += step;
        }
        n -= (i - 1);
        if (n > 0)
        {
            coinRemaining = n;
            continueCoinAt = pos.x;
        }
        else
        {
            coinRemaining = 0;
            continueCoinAt = -99;
        }


    }

    public void GenerateCoins(Vector3 pos, int n, int amount)
    {
        if (amount == 1)
            GenerateCoins(pos, n);
        else
        {
            float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
            MathRand.Shuffle(ref mypos);
            for (int i = 0; i < amount; i++)
            {
                //Minimal generate 1 coins
                if (i == 0)
                    GenerateCoins(new Vector3(mypos[i], pos.y, pos.z), n);
                else {
                    int opt = pickTile();
                    if (opt == 1 || opt ==3 || opt==0)
                        GenerateObstacles(new Vector3(mypos[i], pos.y,pos.z));
                    else if (opt==2)
                        GenerateCoins(new Vector3(mypos[i], pos.y, pos.z), n);
                }
            }
        }
    }

 

    public int GetCoinLane()
    {
        float distance = gameManager.GetComponent<GameManager>().Distance;
        int temp = (int)Mathf.Round(CoinLane.Evaluate(distance));
        return temp;
    }

    public int GetObstacleLane()
    {
        float distance = gameManager.GetComponent<GameManager>().Distance;
        int temp = (int)Mathf.Round(ObstacleLane.Evaluate(distance));
        return temp;
    }

    public int pickTile() {
        int i=MathRand.WeightedPick(new float[] {
            tileSpawnRates.baseTile.Evaluate(player.transform.position.z),
            tileSpawnRates.obstacleTile.Evaluate(player.transform.position.z),
            tileSpawnRates.coinsTile.Evaluate(player.transform.position.z),
            tileSpawnRates.powerupsTile.Evaluate(player.transform.position.z)
        });
        return i;
    }

}

