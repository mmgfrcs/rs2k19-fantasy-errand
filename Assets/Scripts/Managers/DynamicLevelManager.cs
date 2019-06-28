﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Utilities;
using Affdex;

namespace FantasyErrand
{
    public class DynamicLevelManager : LevelManagerBase {

        [Header("Dynamic Balancing")]
        public Affdex.Emotions[] positiveEmotions;
        public Affdex.Emotions[] negativeEmotions;
        public EmotionManager emotionManager;
        public float dynamicSpeedModifier = 1;
        public float emotionUpdateInterval = 0.5f;

        bool isGameEnd = false;

        private float coinAmountMod = 1;
        private float tileMOd = 1;
        private float powerUpsMod = 1;

        private float obstacleAmountMod = 1;
        public static float totalPosEmotions = 0;
        public static float totalNegEmotions = 0;
        public static string enumStatus = "not started";
        internal Dictionary<Emotions, float> EmotionsList { get; set; } = new Dictionary<Emotions, float>();
        void Awake()
        {
            if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Easy) && !difficulty.Equals(Difficulty.Easy))
                gameObject.SetActive(false);
            else if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Special) && !difficulty.Equals(Difficulty.Special))
                gameObject.SetActive(false);
            else if (MainMenuManager.mainMenuDifficulty.Equals(Difficulty.Hard) && !difficulty.Equals(Difficulty.Hard))
                gameObject.SetActive(false);

        }
        protected override void Start()
        {
            EmotionManager.OnFaceResults += EmotionManager_OnFaceResults;
            Player.goldenCoinBroadcast += SetGoldenCoin;
            StartCoroutine(InitialGeneration());
            StartCoroutine(SetRateByEmotion());
            SoundManager.Instance.playBackSound();
            GameManager.OnGameEnd += CheckGameEnd;
            GameManager.OnGameStart += CheckGameStart;
        }

        // Update is called once per frame


        protected override void Update()
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
                            poolDictionary[TileKey.Overhead].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Spike"))
                            poolDictionary[TileKey.Spike].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Boulder"))
                            poolDictionary[TileKey.Boulder].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Hurdling"))
                            poolDictionary[TileKey.Hurdling].Destroy(spawnedObjects[i]);
                        else if (spawnedObjects[i].CompareTag("Wall"))
                            poolDictionary[TileKey.Wall].Destroy(spawnedObjects[i]);

                    }

                    else
                    {
                        ICollectible collect = spawnedObjects[i].GetComponent<ICollectible>();
                        if (collect != null)
                        {
                            if (collect.Type == CollectibleType.Monetary)
                            {
                                TileKey temp = spawnedObjects[i].GetComponent<CoinCollectible>().tileType;
                                poolDictionary[temp].Destroy(spawnedObjects[i]);

                            }

                            else
                            {
                                TileKey temp = collect.TileType;
                                poolDictionary[temp].Destroy(spawnedObjects[i]);
                            }
                        }
                        else
                        {
                            poolDictionary[TileKey.Straight].Destroy(spawnedObjects[i]);
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
            poolDictionary.Add(TileKey.Straight, pooler);

            if (gameMode)
            {

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, potionPhasePrefabs);
                poolDictionary.Add(TileKey.PotionPhase, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, potionMagnetPrefabs);
                poolDictionary.Add(TileKey.PotionMagnet, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, potionGoldPrefabs);
                poolDictionary.Add(TileKey.PotionGold, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, potionBoostPrefabs);
                poolDictionary.Add(TileKey.PotionBoost, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinCopperPrefabs);
                poolDictionary.Add(TileKey.CoinCopper, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinSilverPrefabs);
                poolDictionary.Add(TileKey.CoinSilver, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinGoldPrefabs);
                poolDictionary.Add(TileKey.CoinGold, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinPlatinumPrefabs);
                poolDictionary.Add(TileKey.CoinPlatinum, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, coinRubyPrefabs);
                poolDictionary.Add(TileKey.CoinRuby, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, overheadObstaclePrefabs);
                poolDictionary.Add(TileKey.Overhead, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, spikePrefabs);
                poolDictionary.Add(TileKey.Spike, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, boulderPrefabs);
                poolDictionary.Add(TileKey.Boulder, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, hurdlingPrefabs);
                poolDictionary.Add(TileKey.Hurdling, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 3, wallPrefabs);
                poolDictionary.Add(TileKey.Wall, pooler);
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
                    int n = Random.Range(minimumObstacleLane, Mathf.RoundToInt(GetObstacleLane()*obstacleAmountMod));
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
                    int lanenumber = Random.Range(minimumCoinLane, Mathf.RoundToInt(GetCoinLane()*coinAmountMod));
                    GenerateConstantCoins(new Vector3(coinXLastPos, 0.5f, spawnPos.z), n, lanenumber);
                }
                else if (opt == 3)
                {
                    GeneratePowerUps(new Vector3(spawnPos.x, 0.5f, spawnPos.z), 1);
                }
            }
            if ((opt == 1 || opt == 2) && currMinTilesBeforeNextPowerUps != 0)
                currMinTilesBeforeNextPowerUps--;
        }



        public void GenerateStraights(Vector3 pos)
        {
            spawnedObjects.Add(poolDictionary[TileKey.Straight].Instantiate(pos));
        }





        public void GenerateObstacles(Vector3 pos, int amount)
        {
            if (amount > 5)
                amount = 5;
            int rand = Random.Range((int)TileKey.Wall, (int)TileKey.Hurdling + 1);
            if (amount == 1)
            {
                GameObject go = poolDictionary[(TileKey)rand].Instantiate(pos);
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
                    else if (difficulty.Equals(Difficulty.Hard) || difficulty.Equals(Difficulty.Easy))
                    {
                        GameObject go = poolDictionary[(TileKey)rand].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                    else if (difficulty.Equals(Difficulty.Special))
                    {
                        int x = Random.Range((int)TileKey.Wall, (int)TileKey.Hurdling + 1);
                        GameObject go = poolDictionary[(TileKey)x].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                        spawnedObjects.Add(go);
                    }
                }

            }
        }

        public void generateOverhead(Vector3 pos)
        {
            GameObject go = poolDictionary[TileKey.Overhead].Instantiate(pos);
            spawnedObjects.Add(go);
            if (!difficulty.Equals(Difficulty.Special))
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
                int x = Random.Range((int)TileKey.PotionPhase, (int)TileKey.PotionMagnet + 1);
                GameObject go = poolDictionary[(TileKey)x].Instantiate(pos);
                spawnedObjects.Add(go);
            }
            else
            {
                float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
                MathRand.Shuffle(ref mypos);
                for (int i = 0; i < amount; i++)
                {
                    int x = Random.Range((int)TileKey.PotionPhase, (int)TileKey.PotionMagnet + 1);
                    GameObject go = poolDictionary[(TileKey)x].Instantiate(new Vector3(mypos[i], pos.y, pos.z));
                    spawnedObjects.Add(go);

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
                    go = poolDictionary[(TileKey)SelectCoinPooler()].Instantiate(new Vector3(pos.x, pos.y, start));
                }
                else
                {
                    go = poolDictionary[TileKey.CoinGold].Instantiate(new Vector3(pos.x, pos.y, start));
                }
                spawnedObjects.Add(go);
                start += step;
            }
        }

        public void GenerateConstantCoins(Vector3 pos, int n, int amount)
        {
            if (amount > 5)
                amount = 5;
            if (amount == 1)
                GenerateConstantCoins(pos, n);
            else
            {
                float[] mypos = { -3, -1.5f, 0, 1.5f, 3 };
                MathRand.Shuffle(ref mypos);
                for (int i = 0; i < amount; i++)
                {
                    GenerateConstantCoins(new Vector3(mypos[i], pos.y, pos.z), n);

                }
            }
        }



        public override float GetCoinLane()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(CoinLane.Evaluate(distance)*coinAmountMod);
            return temp;
        }

        public override float GetObstacleLane()
        {
            float distance = gameManager.Distance;
            float temp = Mathf.Round(ObstacleLane.Evaluate(distance)*obstacleAmountMod);
            return temp;
        }


        


        public int PickTile()
        {
            int i = MathRand.WeightedPick(new float[] {
            GetTileRate(TileType.Tile),
            GetTileRate(TileType.Obstacle),
            GetTileRate(TileType.Coin),
            GetTileRate(TileType.Powerups)
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

            return val + (int)TileKey.CoinCopper;
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
                            TileKey temps = spawnedObjects[i].GetComponent<CoinCollectible>().tileType;
                            Vector3 currPos = spawnedObjects[i].transform.position;
                            poolDictionary[temps].Destroy(spawnedObjects[i]);

                            spawnedObjects.RemoveAt(i);
                            GameObject go = poolDictionary[TileKey.CoinRuby].Instantiate(new Vector3(currPos.x, currPos.y, currPos.z));
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
                            TileKey temp = spawnedObjects[i].GetComponent<CoinCollectible>().tileType;
                            Vector3 currPos = spawnedObjects[i].transform.position;
                            poolDictionary[temp].Destroy(spawnedObjects[i]);

                            spawnedObjects.RemoveAt(i);
                            GameObject go = poolDictionary[(TileKey)SelectCoinPooler()].Instantiate(new Vector3(currPos.x, currPos.y, currPos.z));
                            spawnedObjects.Insert(i, go);
                        }
                    }
                }
            }
        }
        public float GetEmotion(Affdex.Emotions emo)
        {
             return EmotionsList[emo];
        }

        IEnumerator SetRateByEmotion()
        {
            enumStatus = "Started";
            while (true)
            {
                if (!isGameEnd)
                {
                    totalPosEmotions = 0; totalNegEmotions = 0;

                    for (int i = 0; i < positiveEmotions.Length; i++)
                        totalPosEmotions += GetEmotion(positiveEmotions[i]);

                    for (int i = 0; i < negativeEmotions.Length; i++)
                        totalNegEmotions += GetEmotion(negativeEmotions[i]);

                    totalPosEmotions = totalPosEmotions / positiveEmotions.Length;
                    totalNegEmotions = totalNegEmotions / negativeEmotions.Length;

                    if (difficulty.Equals(Difficulty.Easy))
                    {
                        if (totalNegEmotions >= totalPosEmotions && totalNegEmotions != 0)
                        {
                            obstacleMod = 1 + totalNegEmotions;//misal 1
                            obstacleAmountMod = obstacleMod;
                            coinMod = 1 - (0.5f * totalNegEmotions);
                            coinAmountMod = coinMod;
                            dynamicSpeedModifier = 4 * (totalNegEmotions);
                        }
                        else if (totalPosEmotions > totalNegEmotions)
                        {
                            obstacleMod = obstacleMod - (totalPosEmotions);
                            if (obstacleMod < 1) obstacleMod = 1;
                            obstacleAmountMod = obstacleAmountMod - (totalPosEmotions);
                            if (obstacleAmountMod < 1) obstacleAmountMod = 1;
                            coinMod = coinMod + totalPosEmotions;
                            if (coinMod > 1) coinMod = 1;
                            coinAmountMod = coinAmountMod + totalPosEmotions;
                            if (coinAmountMod > 1) coinAmountMod = 1;
                            dynamicSpeedModifier = dynamicSpeedModifier - totalPosEmotions;
                            if (dynamicSpeedModifier < 0) dynamicSpeedModifier = 0;
                        }
                    }
                    else if (difficulty.Equals(Difficulty.Hard))
                    {
                        if (totalNegEmotions >= totalPosEmotions)
                        {
                            obstacleMod = 1 - (0.5f * totalNegEmotions);
                            obstacleAmountMod = 1 - (0.5f * totalNegEmotions);
                            coinMod = 1 + (0.5f * totalNegEmotions);
                            coinAmountMod = coinMod;
                            dynamicSpeedModifier = dynamicSpeedModifier - totalNegEmotions;
                            if (dynamicSpeedModifier < 0) dynamicSpeedModifier = 0;
                        }
                        else if (totalPosEmotions > totalNegEmotions)
                        {
                            obstacleMod = obstacleMod + (totalPosEmotions);
                            if (obstacleMod > 1) obstacleMod = 1;
                            obstacleAmountMod = obstacleAmountMod + (totalPosEmotions);
                            if (obstacleAmountMod > 1) obstacleAmountMod = 1;
                            coinMod = coinMod - totalPosEmotions;
                            if (coinMod < 1) coinMod = 1;
                            coinAmountMod = coinAmountMod - totalPosEmotions;
                            if (coinAmountMod < 1) coinAmountMod = 1;
                            dynamicSpeedModifier = 4 * (totalPosEmotions);
                        }
                    }
                    else if (difficulty.Equals(Difficulty.Special))
                    {
                        if (totalNegEmotions >= totalPosEmotions)
                        {
                            obstacleMod = 1 - (0.5f * totalNegEmotions);
                            obstacleAmountMod = 1 - (0.5f * totalNegEmotions);
                            coinMod = 1 + (0.5f * totalNegEmotions);
                            coinAmountMod = coinMod;
                            dynamicSpeedModifier = dynamicSpeedModifier - totalNegEmotions;
                            if (dynamicSpeedModifier < 0) dynamicSpeedModifier = 0;

                        }
                        else if (totalPosEmotions > totalNegEmotions)
                        {
                            obstacleMod = obstacleMod + totalPosEmotions;
                            if (obstacleMod > 2) obstacleMod = 2;
                            obstacleAmountMod = obstacleAmountMod + totalPosEmotions;
                            if (obstacleAmountMod > 2) obstacleAmountMod = 2;
                            coinMod = coinMod - totalPosEmotions;
                            if (coinMod < 1) coinMod = 1;
                            coinAmountMod = coinAmountMod - totalPosEmotions;
                            if (coinAmountMod < 1) coinAmountMod = 1;
                            dynamicSpeedModifier = 4 * (totalPosEmotions);
                        }
                    }
                    gameManager.DynamicSpeedModifier = dynamicSpeedModifier;
                }
                yield return new WaitForSeconds(emotionUpdateInterval);
            }
            enumStatus = "Ended";
        }
        public void CheckGameEnd(GameEndEventArgs args)
        {
            isGameEnd = true;
        }

        public void CheckGameStart()
        {
            isGameEnd = false;
        }

        private void EmotionManager_OnFaceResults(Dictionary<Emotions, float> emotions, Dictionary<Expressions, float> expressions)
        {
            EmotionsList = emotions;
        }
    }

    

}



