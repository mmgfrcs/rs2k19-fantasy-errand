using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;

using FantasyErrand;
using FantasyErrand.Utilities;


namespace FantasyErrand
{
    public class StaticLevelManager : LevelManagerBase {

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
            base.Start();
            StartCoroutine(InitialGeneration());
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
                    player.transform.position.z>spawnedObjects[i].transform.position.z)
                {
                    //Check tile type by GetComponent
                    if (spawnedObjects[i].GetComponent<CollectibleBase>() != null)
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
                        CollectibleBase collect = spawnedObjects[i].GetComponent<CollectibleBase>();
                        if (collect != null)
                        {
                            if (collect.CollectibleType == CollectibleType.Monetary)
                            {
                                TileKey temp = spawnedObjects[i].GetComponent<CoinCollectible>().TileType;
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
                pooler.Initialize(maxGeneratedTile * 1, potionPhasePrefabs);
                poolDictionary.Add(TileKey.PotionPhase, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, potionMagnetPrefabs);
                poolDictionary.Add(TileKey.PotionMagnet, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, potionGoldPrefabs);
                poolDictionary.Add(TileKey.PotionGold, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, potionBoostPrefabs);
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
                pooler.Initialize(maxGeneratedTile * 1, overheadObstaclePrefabs);
                poolDictionary.Add(TileKey.Overhead, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, spikePrefabs);
                poolDictionary.Add(TileKey.Spike, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, boulderPrefabs);
                poolDictionary.Add(TileKey.Boulder, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, hurdlingPrefabs);
                poolDictionary.Add(TileKey.Hurdling, pooler);

                pooler = gameObject.AddComponent<ObjectPooler>();
                pooler.Initialize(maxGeneratedTile * 1, wallPrefabs);
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

            if (opt == 3 && currMinTilesBeforeNextPowerUps <= 0)
                currMinTilesBeforeNextPowerUps = minTilesBeforeNextPowerUps;

            GenerateStraights(new Vector3(spawnPos.x, -0.5f, spawnPos.z));
            float spawnX = MathRand.Pick(new float[] { -3, -1.5f, 0, 1.5f, 3 });
            if (coinRemaining != 0)
            {
                GenerateConstantCoins(new Vector3(continueCoinAt, 0.5f, spawnPos.z), coinRemaining);
            }
            else
            {
                if (opt == 1)
                {
                    currminTilesBeforeOverhead--;
                    int n = Random.Range(minimumObstacleLane, Mathf.RoundToInt(GetObstacleLane()));
                    GenerateObstacles(new Vector3(spawnX, 0.5f, spawnPos.z), n);
                    if (currminTilesBeforeOverhead <= 0) currminTilesBeforeOverhead = minTilesBeforeOverhead;
                }
                else if (opt == 2)
                {
                    SetCoinXPos();
                    int n = Random.Range(minCoins, maxCoins + 1);
                    int lanenumber = Random.Range(minimumCoinLane, Mathf.RoundToInt(GetCoinLane()));
                    GenerateConstantCoins(new Vector3(coinXLastPos, 0.5f, spawnPos.z), n, lanenumber);
                }
                else if (opt == 3)
                {
                    GeneratePowerUps(new Vector3(spawnPos.x, 0.5f, spawnPos.z), 1);
                }
            }
            if ((opt!=3) && currMinTilesBeforeNextPowerUps != 0)
                currMinTilesBeforeNextPowerUps--;
        }



        public void GenerateStraights(Vector3 pos)
        {
            spawnedObjects.Add(poolDictionary[TileKey.Straight].Instantiate(pos));
        }





        public void GenerateObstacles(Vector3 pos, int amount)
        {
            if (amount >= 5)
                amount = 4;
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
                //kasih jarak ke obstacle seandainya dia mengenai overhead
                if (currminTilesBeforeOverhead == 1)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        startPosition += Vector3.forward * tileScale;
                        GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
                    }
                }
                if (amount >= obstacleTolerance && currminTilesBeforeOverhead != 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        startPosition += Vector3.forward * tileScale;
                        GenerateStraights(new Vector3(startPosition.x, -0.5f, startPosition.z));
                    }
                    //generate 2  tile lebih jika obstacle terlalu banyak
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
                    go = poolDictionary[TileKey.CoinRuby].Instantiate(new Vector3(pos.x, pos.y, start));
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


    }

}




