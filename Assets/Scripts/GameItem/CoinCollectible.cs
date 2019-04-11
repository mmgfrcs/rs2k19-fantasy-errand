using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Entities;
public class CoinCollectible : MonoBehaviour,ICollectible {

    [SerializeField]
    private int value;
    CollectibleType type;
    bool magnetPowerUps;
    public GameObject player;
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

    public int magnetRange;
    public bool activateMagnet;
    public float magnetSpeed;

    public void CollectibleEffect()
    {
        throw new System.NotImplementedException();
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject obj =GameObject.Find("GameManagers");
        float scoreValue = obj.GetComponent<GameManager>().Score;
    }

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 90*Time.deltaTime, 0);
        if (Vector3.Distance(player.transform.position, transform.position) < magnetRange && activateMagnet)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, magnetSpeed * Time.deltaTime);
        }
	}
    

}
