using FantasyErrand.Entities.Interfaces;
using UnityEngine;
namespace FantasyErrand
{
    public delegate void startMagnet();
    

    public class MagnetCollectible : MonoBehaviour, ICollectible
    {
        public static event startMagnet TurnMagnet;

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
            TurnMagnet?.Invoke();
            transform.position=new Vector3(0, 0, -9999);
        }
    }

}

