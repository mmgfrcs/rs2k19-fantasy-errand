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

        public TileKey TileType
        {
            get
            {
                return TileKey.PotionPhase;
            }
        }

        public void CollectibleEffect()
        {
            TurnPhasing?.Invoke();
        }

    }
}