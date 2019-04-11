using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;

public class CoinCollectible : MonoBehaviour,ICollectible {

    [SerializeField]
    private int value;
    CollectibleType type;

    public CollectibleType Type
    {
        get
        {
            return CollectibleType.Monetary;
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
        GameObject obj =GameObject.Find("GameManagers");
        float scoreValue = obj.GetComponent<GameManager>().Score;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
