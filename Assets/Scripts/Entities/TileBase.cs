using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public class TileBase : MonoBehaviour
    {
        [SerializeField]
        private TileKey tileType;

        public TileKey TileType { get => tileType; private set => tileType = value; }

        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
    }
}

