using FantasyErrand.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsCollectible : MonoBehaviour,ICollectible{

    
    private int value;
    CollectibleType type;

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
            return value;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
