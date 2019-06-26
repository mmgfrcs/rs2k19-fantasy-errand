﻿using FantasyErrand;
using FantasyErrand.Entities.Interfaces;
using UnityEngine;

// EXAMPLE CODE
public class Coins : MonoBehaviour, ICollectible {
    [SerializeField]
    private int coinValue;

    public CollectibleType Type { get { return CollectibleType.Monetary; } }

    public int Value { get { return coinValue; } }

    public tileKey TileType
    {
        get
        {
            return tileKey.CoinGold;
        }
    }

    public void CollectibleEffect()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
