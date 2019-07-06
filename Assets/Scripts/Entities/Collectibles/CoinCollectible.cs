using UnityEngine;

using FantasyErrand.Entities;
namespace FantasyErrand
{
    public enum CoinType
    {
        None, Copper = 2, Silver, Gold, Platinum, Ruby
    };

    public class CoinCollectible : CollectibleBase
    {

        [SerializeField]
        private CoinType coinType;
        [SerializeField]
        private int value;

        public CollectibleType CoinType
        {
            get
            {
                return CollectibleType.Monetary;
            }
        }

        public int CoinValue
        {
            get
            {
                return value;
            }
        }

        protected override void Update()
        {
            base.Update();
            transform.Rotate(0, 90 * Time.deltaTime, 0);
        }
    }
}
