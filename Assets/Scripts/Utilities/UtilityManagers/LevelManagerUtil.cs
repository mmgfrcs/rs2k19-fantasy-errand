
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManagerUtil : MonoBehaviour {
    public Player player;

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

    [Header("Debug")]
    public bool showGizmos = true;

    //public UnityEngine.UI.Text text;
    List<ObjectPooler> poolers = new List<ObjectPooler>();
    List<GameObject> startObjects = new List<GameObject>();
    List<GameObject> spawnedObjects = new List<GameObject>();

    bool initialized = false;
    int patternSpawned;
    int coinRemaining = 0;
    float continueCoinAt = -99;

    // Use this for initialization
    void Start () {
        StartCoroutine(InitialGeneration());
	}
	
	// Update is called once per frame
	void Update () {
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
            if (Vector3.Distance(player.transform.position, spawnedObjects[i].transform.position) > maxGeneratedTile * tileScale)
            {
                //Check tile type by GetComponent
                if (spawnedObjects[i].GetComponent<IObstacle>() != null)
                    poolers[1].Destroy(spawnedObjects[i]);
                else
                {
                    ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                    if (collect != null)
                    {
                        if(collect.Type == CollectibleType.Monetary) poolers[2].Destroy(spawnedObjects[i]);
                        else poolers[3].Destroy(spawnedObjects[i]);
                    }
                    else poolers[0].Destroy(spawnedObjects[i]);
                }
                spawnedObjects.RemoveAt(i);
            }
        }
        if(initialized && Vector3.Distance(startPosition, player.transform.position) < tileScale * maxGeneratedTile)
        {
            Generate(startPosition);
            startPosition += Vector3.forward * tileScale;
        }
    }

    IEnumerator InitialGeneration()
    {
        int tiles = 0;
        Vector3 spawnPos = startPosition;

        ObjectPooler pooler = gameObject.AddComponent<ObjectPooler>();
        pooler.Initialize(maxGeneratedTile * 3, straightPrefabs);
        poolers.Add(pooler);

        if(gameMode)
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
            if(tiles < startTiles)
            {
                GameObject obj = Instantiate(startPrefab, new Vector3(spawnPos.x, -0.5f, spawnPos.z), Quaternion.identity);
                startObjects.Add(obj);
            }
            else
            {
                if (gameMode) Generate(spawnPos);
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
        int opt = MathRand.WeightedPick(tileSpawnRates);
        print("Generating " + opt);
        GenerateStraights(new Vector3(spawnPos.x, -0.5f, spawnPos.z));
        if (coinRemaining != 0) GenerateCoins(new Vector3(continueCoinAt, 0.5f, spawnPos.z), coinRemaining);
        else
        {
            float spawnX = MathRand.Pick(new float[] { -3, -1.5f, 0, 1.5f, 3 });
            if (opt == 1)
            {
                GenerateObstacles(new Vector3(spawnX, 0.25f, spawnPos.z));
            }
            else if (opt == 2)
            {
                int n = Random.Range(minCoins, maxCoins + 1);
                GenerateCoins(new Vector3(spawnX, 0.5f, spawnPos.z), n);
            }
            else if (opt == 3) spawnedObjects.Add(poolers[3].Instantiate(new Vector3(spawnPos.x, 1f, spawnPos.z)));

        }
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

    public void GenerateCoins(Vector3 pos, int n)
    {
        float step = tileScale / ( Mathf.Min(maxCoinSpawnPerTile, n) + 1);
        float start = pos.z;
        int i = 1;
        for (; i <= Mathf.Min(maxCoinSpawnPerTile, n); i++)
        {
            GameObject go = poolers[2].Instantiate(new Vector3(pos.x, pos.y, start));
            spawnedObjects.Add(go);
            start += step;
        }
        n -= i;
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
}

[System.Serializable]
public class TileSpawnRates
{
    public float baseTile = 100, obstacleTile = 100, coinsTile = 100, powerupsTile = 100;

    public float this[int val]
    {
        get
        {
            if (val == 0) return baseTile;
            else if (val == 1) return obstacleTile;
            else if (val == 2) return coinsTile;
            else if (val == 3) return powerupsTile;
            else throw new System.IndexOutOfRangeException("There are only 4 rates in TileSpawnRates");
        }
        set
        {
            if (value == 0) baseTile = value;
            else if (value == 1) obstacleTile = value;
            else if (value == 2) coinsTile = value;
            else if (value == 3) powerupsTile = value;
            else throw new System.IndexOutOfRangeException("There are only 4 rates in TileSpawnRates");
        }
    }

    public float[] ToArray()
    {
        return new float[] { baseTile, obstacleTile, coinsTile, powerupsTile };
    }

    public static implicit operator float[](TileSpawnRates rate)
    {
        return rate.ToArray();
    }
}
