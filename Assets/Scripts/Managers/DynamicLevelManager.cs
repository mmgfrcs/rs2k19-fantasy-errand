using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Utilities;
using FantasyErrand.Entities.Interfaces;
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
        int currminTilesBeforeOverhead = 0;
        public float baseObstacleRatio = 100;
        public int minimumObstacleLane = 1;
        public int minTilesBeforeOverhead = 8;
        public int obstacleTolerance = 4;
        public int LessPosMinObs = 1;
        public int LessPosMaxObs = 1;
        public int LessNegMinObs = 1;
        public int LessNegMaxObs = 1;
        public int MorePosMinObs = 1;
        public int MorePosMaxObs = 1;
        public int MoreNegMinObs = 1;
        public int MoreNegMaxObs = 1;
        public int EqualMinObs = 1;
        public int EqualMaxObs = 1;


        [Header("Coins")]
        float continueCoinAt = -99;
        int coinRemaining = 0;
        public int maxCoinSpawnPerTile = 6;
        public int minCoins = 4, maxCoins = 24;
        public int minimumCoinLane = 1;
        public int LessPosMinCoins= 1;
        public int LessPosMaxCoins = 1;
        public int LessNegMinCoins = 1;
        public int LessNegMaxCoins = 1;
        public int MorePosMinCoins = 1;
        public int MorePosMaxCoins= 1;
        public int MoreNegMinCoins= 1;
        public int MoreNegMaxCoins = 1;
        public int EqualMinCoins= 1;
        public int EqualMaxCoins= 1;


        [Header("Power Ups")]
        public int minTilesBeforeNextPowerUps = 8;
        int currMinTilesBeforeNextPowerUps = 0;

        [Header("Game Tile Probability")]
        public float rateObstacle = 30;
        public float rateBaseTile = 30;
        public float rateCoin = 30;
        public float ratePowerUps = 30;
        public float rateSpeed=0.5f;
        public float emotionUpdateInterval = 0.5f;
        public float rateBottomLimit = 10;
        public float rateUpperLimit = 200;
        public float speedUpperLimit = 30;
        public float speedBottomLimit = 10;

        [Header("Emotion Multiplier")]
        public float posEmoCoin=1;
        public float posEmoPowerUps=1;
        public float posEmoBaseTile=1;
        public float posEmoObstacle=1;
        public float posEmoSpeedMult = 0.2f;

        public float negEmoCoin=1;
        public float negEmoObstacle=1;
        public float negEmoBaseTile=1;
        public float negEmoPowerUps=1;
        public float negEmoSpeedMult = 0.1f;

        List<ObjectPooler> poolers = new List<ObjectPooler>();
        List<GameObject> startObjects = new List<GameObject>();
        [SerializeField]
        List<GameObject> spawnedObjects = new List<GameObject>();
        bool initialized = false;

        public EmotionManager emotionManager;
        bool turnGoldenCoin = false;
        float coinXLastPos = 1.5f;


        //<Coin Property Value>
        private int coinValueLevel = 0;
        private float silverDistance = 0;
        private float goldDistance = 0;
        private float platinumDistance = 0;

        public Affdex.Emotions[] positiveEmotions;
        public Affdex.Emotions[] negativeEmotions;

        private bool isGameEnd=false;


        IEnumerator SetRateByEmotion()
        {
            
            while (true)
            {
                if (!isGameEnd)
                {
                    float totalPosEmotions = 0, totalNegEmotions = 0;

                    for (int i = 0; i < positiveEmotions.Length; i++)
                        totalPosEmotions += GetEmotion(positiveEmotions[i]);

                    for (int i = 0; i < negativeEmotions.Length; i++)
                        totalNegEmotions += GetEmotion(negativeEmotions[i]);

                    float temp = rateBaseTile + (totalPosEmotions * posEmoBaseTile) + (totalNegEmotions * negEmoBaseTile);
                    if (temp > rateBottomLimit && temp < rateUpperLimit)
                        rateBaseTile = temp;
                    temp = ratePowerUps + (totalPosEmotions * posEmoPowerUps) + (totalNegEmotions * negEmoPowerUps);
                    if (temp > rateBottomLimit && temp < rateUpperLimit)
                        ratePowerUps = temp;
                    temp = rateCoin + (totalPosEmotions * posEmoCoin) + (totalNegEmotions * negEmoCoin);
                    if (temp > rateBottomLimit && temp < rateUpperLimit)
                        rateCoin = temp;
                    temp = rateObstacle + (totalPosEmotions * rateObstacle) + (totalNegEmotions * rateObstacle);
                    if (temp > rateBottomLimit && temp < rateUpperLimit)
                        rateObstacle = temp;
                    temp = rateSpeed + (totalPosEmotions * posEmoSpeedMult) + (totalNegEmotions * negEmoSpeedMult);
                    if ((gameManager.GetCurrSpeed() + temp) > speedBottomLimit && (gameManager.GetCurrSpeed() + temp) < rateUpperLimit)
                    {
                        rateSpeed = temp;
                        gameManager.DynamicSpeedModifier = rateSpeed;
                    }
                    print("Ienum Worked");
                }

                
                yield return new WaitForSeconds(emotionUpdateInterval);
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
                pooler.Initialize(maxGeneratedTile * 3, obstaclePrefabs);
                poolers.Add(pooler);

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
                print("Ini maslah coin rem=" + coinRemaining);
                GenerateConstantCoins(new Vector3(continueCoinAt, 0.5f, spawnPos.z), coinRemaining);
            }
            else
            {
                if (opt == 1)
                {
                    currminTilesBeforeOverhead--;
                    int n = Random.Range(GetMinObstacleLane(), GetMaxObstacleLane());
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
                    int n = Random.Range(minimumCoinLane, minimumCoinLane);
                    int lanenumber = Random.Range(GetMinCoinLane(), GetMaxCoinLane());
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
            if (amount == 1)
            {
                GenerateObstacles(pos);
            }
            else
            {
                float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
                MathRand.Shuffle(ref mypos);
                Stack<GameObject> tempObject = new Stack<GameObject>();
                for (int i = 0; i < amount; i++)
                {
                    if (i == 0)
                        GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z), minCoins);
                    else
                    {
                        if (currminTilesBeforeOverhead <= 0)
                            generateOverhead(new Vector3(0, pos.y, pos.z));
                        else
                            GenerateObstacles(new Vector3(mypos[i], pos.y, pos.z));
                    }
                }

            }
        }

        public void generateOverhead(Vector3 pos)
        {
            GameObject go = poolers[8].Instantiate(pos);
            spawnedObjects.Add(go);
            startPosition += Vector3.forward * tileScale;
            GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
        }

        public void GeneratePowerUps(Vector3 pos, int amount)
        {
            if (amount == 1)
            {
                GameObject go = poolers[2].Instantiate(pos);
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
                        GameObject go = poolers[2].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                    else
                    {
                        GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z), minCoins);

                    }

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
                    go = poolers[7].Instantiate(new Vector3(pos.x, pos.y, start));
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


        void Start()
        {
            Player.goldenCoinBroadcast += SetGoldenCoin;
            StartCoroutine(InitialGeneration());
            StartCoroutine(SetRateByEmotion());
            GameManager.OnGameEnd += CheckGameEnd;
            GameManager.OnGameStart += CheckGameStart;
            SoundManager.Instance.playBackSound();
        }

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
                    player.transform.position.z > spawnedObjects[i].transform.position.z)
                {
                    //Check tile type by GetComponent
                    if (spawnedObjects[i].GetComponent<IObstacle>() != null)
                    {
                        if (spawnedObjects[i].CompareTag("Overhead"))
                            poolers[8].Destroy(spawnedObjects[i]);
                        else
                            poolers[1].Destroy(spawnedObjects[i]);
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
                            else poolers[2].Destroy(spawnedObjects[i]);
                        }
                        else
                        {
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

        public float GetEmotion(Affdex.Emotions emo)
        {
            if (emotionManager.EmotionsList != null)
            {
                if (emotionManager.FaceStatus.Equals("Tracking"))
                {
                    print("Bagian ini Jalan");
                    return emotionManager.EmotionsList[0][emo];

                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public void SetCoinProperty()
        {
            coinValueLevel = GameDataManager.instance.Data.UpgradeLevels.CoinValueLevel;
            silverDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].SilverDistance;
            goldDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].GoldDistance;
            platinumDistance = GameDataManager.instance.UpgradeEffects.CoinValueUpgrades[coinValueLevel].PlatinumDistance;
        }

        public int PickTile()
        {
            int i = MathRand.WeightedPick(new float[] {
            rateBaseTile,
            rateObstacle,
            rateCoin,
            ratePowerUps
        });
            return i;
        }

        public int GetMinObstacleLane()
        {
            float totalPosEmotions = 0, totalNegEmotions = 0;
            for (int i = 0; i < positiveEmotions.Length; i++)
                totalPosEmotions += GetEmotion(positiveEmotions[i]);
            totalPosEmotions = totalPosEmotions / positiveEmotions.Length;

            for (int i = 0; i < negativeEmotions.Length; i++)
                totalNegEmotions += GetEmotion(negativeEmotions[i]);
            totalNegEmotions = totalNegEmotions / negativeEmotions.Length;

            ///4 case positive emotion dominasi diatas 50%, positive emo dominasi dbawah 50%,negatie emo diatas 50, negative emo dibawah 50, positiv dan negativ equals
            if (totalPosEmotions > totalNegEmotions && totalPosEmotions >= 0.5)
                return MorePosMinObs;
            else if (totalPosEmotions > totalNegEmotions && totalPosEmotions < 0.5)
                return LessPosMinObs;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions >= 0.5)
                return MoreNegMinObs;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions < 0.5)
                return LessNegMinObs;
            else
                return EqualMinObs;
        }


        public int GetMaxObstacleLane()
        {
            float totalPosEmotions = 0, totalNegEmotions = 0;
            for (int i = 0; i < positiveEmotions.Length; i++)
                totalPosEmotions += GetEmotion(positiveEmotions[i]);
            totalPosEmotions = totalPosEmotions / positiveEmotions.Length;

            for (int i = 0; i < negativeEmotions.Length; i++)
                totalNegEmotions += GetEmotion(negativeEmotions[i]);
            totalNegEmotions = totalNegEmotions / negativeEmotions.Length;

            ///4 case positive emotion dominasi diatas 50%, positive emo dominasi dbawah 50%,negatie emo diatas 50, negative emo dibawah 50, positiv dan negativ equals
            if (totalPosEmotions > totalNegEmotions && totalPosEmotions >= 0.5)
                return MorePosMaxObs;
            else if (totalPosEmotions > totalNegEmotions && totalPosEmotions < 0.5)
                return LessPosMaxObs;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions >= 0.5)
                return MoreNegMaxObs;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions < 0.5)
                return LessNegMaxObs;
            else
                return EqualMaxObs;
        }

        public int GetMinCoinLane()
        {
            float totalPosEmotions = 0, totalNegEmotions = 0;
            for (int i = 0; i < positiveEmotions.Length; i++)
                totalPosEmotions += GetEmotion(positiveEmotions[i]);
            totalPosEmotions = totalPosEmotions / positiveEmotions.Length;

            for (int i = 0; i < negativeEmotions.Length; i++)
                totalNegEmotions += GetEmotion(negativeEmotions[i]);
            totalNegEmotions = totalNegEmotions / negativeEmotions.Length;
            if (totalPosEmotions > totalNegEmotions && totalPosEmotions >= 0.5)
                return MorePosMinCoins;
            else if (totalPosEmotions > totalNegEmotions && totalPosEmotions < 0.5)
                return LessPosMinCoins;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions >= 0.5)
                return MoreNegMinCoins;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions < 0.5)
                return LessNegMinCoins;
            else
                return EqualMinCoins;
        }

        public int GetMaxCoinLane()
        {
            float totalPosEmotions = 0, totalNegEmotions = 0;
            for (int i = 0; i < positiveEmotions.Length; i++)
                totalPosEmotions += GetEmotion(positiveEmotions[i]);
            totalPosEmotions = totalPosEmotions / positiveEmotions.Length;

            for (int i = 0; i < negativeEmotions.Length; i++)
                totalNegEmotions += GetEmotion(negativeEmotions[i]);
            totalNegEmotions = totalNegEmotions / negativeEmotions.Length;
            if (totalPosEmotions > totalNegEmotions && totalPosEmotions >= 0.5)
                return MorePosMaxCoins;
            else if (totalPosEmotions > totalNegEmotions && totalPosEmotions < 0.5)
                return LessPosMaxCoins;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions >= 0.5)
                return MoreNegMaxCoins;
            else if (totalNegEmotions > totalPosEmotions && totalNegEmotions < 0.5)
                return LessNegMaxCoins;
            else
                return EqualMaxCoins;
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

            return val + 3;
        }

        public void CheckGameEnd(GameEndEventArgs args)
        {
            isGameEnd = true;
        }

        public void CheckGameStart()
        {
            isGameEnd = false;
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
                            GameObject go = poolers[7].Instantiate(new Vector3(currPos.x, currPos.y, currPos.z));
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
