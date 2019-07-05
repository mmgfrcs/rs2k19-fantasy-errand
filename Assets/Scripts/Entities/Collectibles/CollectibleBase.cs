using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand.Entities
{
    public enum CollectibleType
    {
        None, Monetary, Powerups
    }

    public class CollectibleBase : TileBase
    {
        [SerializeField]
        private CollectibleType collectibleType;

        public CollectibleType CollectibleType { get => collectibleType; private set => collectibleType = value; }

        public virtual void CollectibleEffect()
        {
            if(collectibleType == CollectibleType.Powerups)
            {

            }
            else if(collectibleType == CollectibleType.Monetary)
            {

            }
        }
    }
}

