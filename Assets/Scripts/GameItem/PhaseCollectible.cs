using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
namespace FantasyErrand
{
    public class PhaseCollectible : MonoBehaviour,ICollectible
    {
        GameObject player;

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
            player.GetComponent<PowerUpsManager>().startPhasePowerUps();
        }

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}