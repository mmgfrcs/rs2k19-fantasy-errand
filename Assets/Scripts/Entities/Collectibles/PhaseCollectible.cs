using FantasyErrand.Entities.Interfaces;
using UnityEngine;
namespace FantasyErrand.Entities
{
   
    public class PhaseCollectible : MonoBehaviour,ICollectible
    {
        public static event System.Action TurnPhasing;
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
            TurnPhasing?.Invoke();
            transform.position = new Vector3(0, 0, -9999);
        }

    }
}