using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Utilities;


namespace FantasyErrand
{
    public enum ObstacleType
    {
       Spike=8,Boulder,Hurdling,Wall     
    }


    public class TestingLevelManager : MonoBehaviour {

        /// <summary>
        /// catatan(ini sesudah diganti, pas sebelum pooler[2]=coin, pooler 3 = powerups
        /// pooler 0= basic/straight
        /// pooler 1 = obstacle
        /// pooler 2 = powerups
        /// pooler 3 = copper
        /// pooler 4 = silver
        /// pooler 5 = GOld
        /// pooler 6 = PLatinum
        /// pooler 7 = ruby
        /// pooler 8 = overhead obstacle
        /// </summary>

        public Player player;
        public GameManager gameManager;

        [Header("Level Objects")]
        public GameObject startPrefab;
        public GameObject[] straightPrefabs, powerupsPrefabs, coinPrefabs,overheadObstaclePrefabs;
        public GameObject[] coinCopperPrefabs, coinSilverPrefabs,coinGoldPrefabs,coinPlatinumPrefabs,coinRubyPrefabs;
        public GameObject[] spikePrefabs,boulderPrefabs,hurdlingPrefabs,wallPrefabs;
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
        int currminTilesBeforeOverhead=0;
        public int obstacleTolerance = 4;
        [Header("Coins")]
        public int maxCoinSpawnPerTile = 6;
        public int minCoins = 4, maxCoins = 24;
        public int minimumCoinLane = 1;

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

        bool turnGoldenCoin = false;
        float coinXLastPos=1.5f;
        // Use this for initialization

        //<Coin Property Value>
        private int coinValueLevel = 0;
        public float silverDistance = 0;
        public float goldDistance = 0;
        public float platinumDistance = 0;


        void Awake()
        {
            if (MainMenuManager.difficultyLevel.Equals("easy") && !EasyTrigger)
                gameObject.SetActive(false);
            else if (MainMenuManager.difficultyLevel.Equals("normal") && !CleverTrigger)
                gameObject.SetActive(false);
            else if (MainMenuManager.difficultyLevel.Equals("hard") && !HardTrigger)
                gameObject.SetActive(false);

        }


        void Start()
        { 
            Player.goldenCoinBroadcast += SetGoldenCoin;
            StartCoroutine(InitialGeneration());
            SoundManager.Instance.playBackSound();
        }

        public bool EasyTrigger=false;
        public bool HardTrigger = false;
        public bool CleverTrigger = false;
        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < startObjects.Count; i++)
            {
                if (Vector3.Distance(player.transform.position, startObjects[i].transform.position) > maxGeneratedTile * tileScale * 1.25)
                {
                    Destroy(startObjects[i]);
                    startObjects.RemoveAt(i);
                }
            }
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (Vector3.Distance(player.transform.position, spawnedObjects[i].transform.position) > maxGeneratedTile * tileScale * 1.25 && 
                    player.transform.position.z>spawnedObjects[i].transform.position.z)
                {
                    //Check tile type by GetComponent
                    if (spawnedObjects[i].GetComponent<IObstacle>() != null)
                    {
                        if (spawnedObjects[i].CompareTag("Overhead"))
                            poolers[7].Destroy(spawnedObjects[i]);
                        else if(spawnedObjects[i].CompareTag("Spike"))
                            poolers[8].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Boulder"))
                            poolers[9].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Hurdling"))
                            poolers[10].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Wall"))
                            poolers[11].Destroy(spawnedObjects[i]);

                    }
                        
                    else
                    {
                        ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                        if (collect != null)
                        {
                            if (collect.Type == CollectibleType.Monetary)
                            {
                                CoinType temp = spawnedObjects[i].GetComponent<CoinCollectible>().coinType;
                                poolers[(int)temp].Destroy(spawnedObjects[i]);

                            }
                            //Destroy Powerups
                            else poolers[1].Destroy(spawnedObjects[i]);
                        }
                        else
                        {
                            //Destroy Obstacle
                            poolers[0].Destroy(spawnedObjects[i]);                            
                        } 
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
            SetCoinProperty();
            currMinTilesBeforeNextPowerUps = minTilesBeforeNextPowerUps;
            int tiles = 0;
            Vector3 spawnPos = startPosition;

            ObjectPooler pooler = gameObject.AddComponent<ObjectPooler>();
            pooler.Initialize(maxGeneratedTile * 3, straightPrefabs);
            poolers.Add(pooler);

            if (gameMode)
            {
                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, powerupsPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinCopperPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinSilverPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinGoldPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinPlatinumPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinRubyPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, overheadObstaclePrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, spikePrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, boulderPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, hurdlingPrefabs);
                poolers.Add(pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, wallPrefabs);
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
            ///Startpos=spawnpos    
            startPosition = spawnPos;
            initialized = true;
        }

        public void Generate(Vector3 spawnPos)
        {

            int opt = 0;
            do
            {
                opt = PickTile();
            } while (opt == 3 && currMinTilesBeforeNextPowerUps != 0);

            if (opt == 3 && currMinTilesBeforeNextPowerUps == 0)
                currMinTilesBeforeNextPowerUps = minTilesBeforeNextPowerUps;

            GenerateStraights(new Vector3(spawnPos.x, -0.5f, spawnPos.z));
            float spawnX = MathRand.Pick(new float[] { -3, -1.5f, 0, 1.5f, 3 });
            if (coinRemaining != 0)
            {
                print("Ini maslah coin rem="+coinRemaining);
                GenerateConstantCoins(new Vector3(continueCoinAt, 0.5f, spawnPos.z), coinRemaining);
            }
            else
            {
                if (opt == 1)
                {
                    currminTilesBeforeOverhead--;
                    int n = Random.Range(minimumObstacleLane, GetObstacleLane());
                    GenerateObstacles(new Vector3(spawnX, 0.5f, spawnPos.z), n);
                    if (currminTilesBeforeOverhead <= 0) currminTilesBeforeOverhead = minTilesBeforeOverhead;
                    if (n >= obstacleTolerance)
                    {
                        startPosition += Vector3.forward * tileScale;
                        GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));//generate 1 tile lebih jika obstacle terlalu banyak
                    }
                }
                else if (opt == 2)
                {
                    SetCoinXPos();
                    int n = Random.Range(minCoins, maxCoins + 1);
                    int lanenumber = Random.Range(minimumCoinLane, GetCoinLane());
                    GenerateConstantCoins(new Vector3(coinXLastPos, 0.5f, spawnPos.z), n, lanenumber);
                }
                else if (opt == 3)
                {
                    int n = Random.Range(1, 3);
                    GeneratePowerUps(new Vector3(spawnPos.x, 0.5f, spawnPos.z), n);
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
            int rand = Random.Range((int)ObstacleType.Spike, (int)ObstacleType.Wall + 1);
            if (amount == 1)
            {
                GameObject go = poolers[rand].Instantiate(pos);
                spawnedObjects.Add(go);
            }
            else
            {
                float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
                MathRand.Shuffle(ref mypos);

                for (int i = 0; i < amount; i++)
                {
                    if (currminTilesBeforeOverhead <= 0)
                    {
                        generateOverhead(new Vector3(0, pos.y, pos.z));
                        break;
                    }
                    else if (HardTrigger || EasyTrigger)
                    {
                        GameObject go = poolers[rand].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                    else if (CleverTrigger)
                    {
                        int x = Random.Range((int)ObstacleType.Spike, (int)ObstacleType.Wall);
                        GameObject go = poolers[x].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                }
                
            }
        }

        public void generateOverhead(Vector3 pos)
        {
            GameObject go = poolers[7].Instantiate(pos);
            spawnedObjects.Add(go);
            if (!CleverTrigger)
            {
                startPosition += Vector3.forward * tileScale;
                GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
            }
            else
            {
                startPosition += Vector3.forward * tileScale;
                GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
                startPosition += Vector3.forward * tileScale;
                GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
            }
                
        }

        public void GeneratePowerUps(Vector3 pos, int amount)
        {
            if (amount == 1)
            {
                GameObject go = poolers[1].Instantiate(pos);
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
                        GameObject go = poolers[1].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                    else
                    {
                            GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z),minCoins);
                    
                    }
                        
                }

            }
        }
        


        public void SetCoinXPos()
        {
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
                if (coinXLastPos == -3)
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
                GameObject go;
                if (!turnGoldenCoin)
                {
                    go = poolers[SelectCoinPooler()].Instantiate(new Vector3(pos.x, pos.y, start));
                }
                else
                {
                    go = poolers[6].Instantiate(new Vector3(pos.x, pos.y, start));
                }
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
                    GenerateCoins(new Vector3(mypos[i], pos.y, pos.z), n);
                }
            }
        }

        public void GenerateConstantCoins(Vector3 pos, int n)
        {
            float step = tileScale / (Mathf.Min(maxCoinSpawnPerTile, n) + 1);
            float start = pos.z;
            int i = 1;
            for (; i <= Mathf.Min(maxCoinSpawnPerTile, n); i++)
            {
                GameObject go;
                if (!turnGoldenCoin)
                {
                    go = poolers[SelectCoinPooler()].Instantiate(new Vector3(pos.x, pos.y, start));
                }
                else
                {
                    go = poolers[6].Instantiate(new Vector3(pos.x, pos.y, start));
                }
                spawnedObjects.Add(go);
                start += step;
            }
        }

        public void GenerateConstantCoins(Vector3 pos, int n, int amount)
        {
            if (amount == 1)
                GenerateConstantCoins(pos, n);
            else
            {
                float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
                MathRand.Shuffle(ref mypos);
                for (int i = 0; i < amount; i++)
                {
                    //Minimal generate 1 coins
                    if (i == 0)
                        GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z), n);
                    else
                    {
                        int opt = PickTile();
                        if (opt == 1)
                        {
                            GenerateObstacles(new Vector3(mypos[i], pos.y, pos.z));
                            
                        }
                        else
                            GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z), n);
                    }
                }
            }
        }



        public int GetCoinLane()
        {
            float distance = gameManager.Distance;
            int temp = (int)Mathf.Round(CoinLane.Evaluate(distance));
            return temp;
        }

        public int GetObstacleLane()
        {
            float distance = gameManager.Distance;
            int temp = (int)Mathf.Round(ObstacleLane.Evaluate(distance));
            return temp;
        }

        public int PickTile()
        {
            int i = MathRand.WeightedPick(new float[] {
            tileSpawnRates.baseTile.Evaluate(player.transform.position.z),
            tileSpawnRates.obstacleTile.Evaluate(player.transform.position.z),
            tileSpawnRates.coinsTile.Evaluate(player.transform.position.z),
            tileSpawnRates.powerupsTile.Evaluate(player.transform.position.z)
        });
            return i;
        }


        public void SetCoinProperty()
        {
            coinValueLevel = GameDataManager.instance.Data.UpgradeLevels.CoinValueLevel;
            silverDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].SilverDistance;
            goldDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].GoldDistance;
            platinumDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].PlatinumDistance;
        }


        /// <summary>
        /// Selects the coin pooler to use based on distance and Coin Value upgrade level
        /// </summary>
        /// <returns></returns>
        public int SelectCoinPooler()
        {
            int val = 0;
            float distance = gameManager.Distance;

            if (distance >= silverDistance && distance < goldDistance)
            {
                float[] probs = { 1, 1 };
                val = MathRand.WeightedPick(probs);
            }
            else if (distance >= goldDistance && distance < platinumDistance)
            {
                float[] probs = { 1, 1, 1 };
                val = MathRand.WeightedPick(probs);
            }
            else if (distance >= platinumDistance)
            {
                float[] probs = { 1, 1, 1, 1 };
                val = MathRand.WeightedPick(probs);
            }
            else
            {
                val = 0;
            }

            return val + 2;
        }

        public void SetGoldenCoin(bool isSwitched)
        {
            turnGoldenCoin = isSwitched;
            if (isSwitched)
            {
                int size = spawnedObjects.Count;
                for (int i = 0; i < size; i++)
                {
                    ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                    if (collect != null)
                    {
                        if (collect.Type == CollectibleType.Monetary)
                        {
                            CoinType temp = spawnedObjects[i].GetComponent<CoinCollectible>().coinType;
                            Vector3 currPos = spawnedObjects[i].transform.position;
                            poolers[(int)temp].Destroy(spawnedObjects[i]);

                            spawnedObjects.RemoveAt(i);
                            GameObject go = poolers[6].Instantiate(new Vector3(currPos.x, currPos.y, currPos.z));
                            spawnedObjects.Insert(i, go);
                        }
                    }
                }
            }
            else
            {
                int size = spawnedObjects.Count;
                for (int i = 0; i < size; i++)
                {
                    ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                    if (collect != null)
                    {
                        if (collect.Type == CollectibleType.Monetary)
                        {
                            CoinType temp = spawnedObjects[i].GetComponent<CoinCollectible>().coinType;
                            Vector3 currPos = spawnedObjects[i].transform.position;
                            poolers[(int)temp].Destroy(spawnedObjects[i]);

                            spawnedObjects.RemoveAt(i);
                            GameObject go = poolers[SelectCoinPooler()].Instantiate(new Vector3(currPos.x, currPos.y, currPos.z));
                            spawnedObjects.Insert(i, go);
                        }
                    }
                }
            }
        }

    }
}



