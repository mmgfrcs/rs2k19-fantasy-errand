using FantasyErrand.Entities.Interfaces;
using UnityEngine;
namespace FantasyErrand.Entities
{
    public delegate void startBoost();
    public class BoostCollectible : MonoBehaviour,ICollectible
    {
        public static event startBoost TurnBoost;
        GameObject powerupsManager;
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
            TurnBoost?.Invoke();
            transform.position = new Vector3(0, 0, -9999);
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