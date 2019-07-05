using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class ObstacleBase : TileBase
    {
        [SerializeField]
        private bool isHurdling;
        [SerializeField]
        private float spawnRate;

        public bool IsHurdling
        {
            get
            {
                return isHurdling;
            }
        }

        public float SpawnRate
        {
            get
            {
                return spawnRate;
            }
        }
    }
}

