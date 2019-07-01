using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Entities;

namespace FantasyErrand.Entities
{
    public delegate void startGoldenCoin();
    public class GoldenCoinCollectible : MonoBehaviour,ICollectible
    {
        public CollectibleType Type
        {
            get
            {
                return CollectibleType.Powerups;
            }
        }

        public int Value
        {
            get
            {
                return 0;
            }

        }

        public TileKey TileType
        {
            get
            {
                return TileKey.CoinRuby;
            }
        }

        public static event startBoost TurnGoldenCoin;

        public void CollectibleEffect()
        {
            TurnGoldenCoin?.Invoke();
            transform.position = new Vector3(0, 0, -9999);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}