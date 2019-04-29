using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
namespace FantasyErrand.Entities
{
    public class BoostCollectible : MonoBehaviour,ICollectible
    {
        GameObject powerupsManager;
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

        public void CollectibleEffect()
        {
            powerupsManager.GetComponent<PowerUpsManager>().StartBoostPowerUps();
        }

        // Use this for initialization
        void Start()
        {
            powerupsManager = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}