using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
namespace FantasyErrand.Entities
{

    public delegate void startPhasing();
   
    public class PhaseCollectible : MonoBehaviour,ICollectible
    {
        public static event startPhasing TurnPhasing;

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
            TurnPhasing?.Invoke();
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