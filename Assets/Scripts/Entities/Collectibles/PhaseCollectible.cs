using FantasyErrand.Entities.Interfaces;
using UnityEngine;
namespace FantasyErrand.Entities
{
   
    public class PhaseCollectible : MonoBehaviour,ICollectible
    {
        public static event System.Action TurnPhasing;

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

        public tileKey TileType
        {
            get
            {
                return tileKey.PotionPhase;
            }
        }

        public void CollectibleEffect()
        {
            TurnPhasing?.Invoke();
        }

    }
}