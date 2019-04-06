using FantasyErrand.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EXAMPLE CODE
public class Coins : MonoBehaviour, ICollectible {
    [SerializeField]
    private int coinValue;

    public CollectibleType Type { get { return CollectibleType.Monetary; } }

    public int Value { get { return coinValue; } }

    public void CollectibleEffect()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
