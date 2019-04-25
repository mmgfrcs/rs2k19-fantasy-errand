using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Entities;
namespace FantasyErrand
{
    public class MagnetCollectible : MonoBehaviour, ICollectible
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

        public void CollectibleEffect()
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            obj.GetComponent<PowerUpsManager>().startMagnetPowerUps();

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

