using UnityEngine;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand
{

    public enum CoinType
    {
        None, Copper=3, Silver, Gold, Platinum, Ruby
    };

    public class CoinCollectible : MonoBehaviour, ICollectible
    {

        [SerializeField]
        public CoinType coinType;
        public int value;
        CollectibleType type;
        bool magnetActivated=false;
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

        }

        // Use this for initialization
        void Start()
        {
            //player = GameObject.FindGameObjectWithTag("Player");
            PowerUpsManager.magnetBroadcast += setMagnet;

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, 90 * Time.deltaTime, 0);
            if (magnetActivated)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 10 * Time.deltaTime);
            }
        }

        void setMagnet(bool activated, int range,int speed)
        {
            magnetActivated = activated;
            magnetRange = range;
            magnetSpeed = speed;
        }

        public void SetTarget(GameObject obj,float speed)
        {
            player = obj;
            magnetActivated=true;
            magnetSpeed = speed;
        }

        public void SetMagnet(bool test)
        {
            magnetActivated = test;
        }
    }
}
