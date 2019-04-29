using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities.Interfaces;
using FantasyErrand;
using FantasyErrand.Entities;

namespace FantasyErrand
{
    public class CoinCollectible : MonoBehaviour, ICollectible
    {

        [SerializeField]
        private int value;
        CollectibleType type;
        private bool magnetActivated;
        private GameObject player;
        private float magnetSpeed;
        private float magnetRange;
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



        public void CollectibleEffect()
        {
            throw new System.NotImplementedException();
        }

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            PowerUpsManager.magnetBroadcast += setMagnet;

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, 90 * Time.deltaTime, 0);
            if (magnetActivated)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < magnetRange)
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, magnetSpeed * Time.deltaTime);
                }
            }
        }

        void setMagnet(bool activated, int range,int speed)
        {
            magnetActivated = activated;
            magnetRange = range;
            magnetSpeed = speed;
        }
    }
}
