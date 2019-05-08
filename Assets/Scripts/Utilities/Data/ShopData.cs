using FantasyErrand.Utilities;
using UnityEngine;

namespace FantasyErrand.Data
{
    [CreateAssetMenu(fileName = "New Data", menuName = "Data/Shop Data")]
    public class ShopData : ScriptableObject
    {
        public ItemID itemId = 0;
        public Sprite itemImage;
        public string itemName;

        [Header("Upgrade-Specific")]
        public int upgrades;
        [TextArea]
        public string[] itemDescriptions;
        public float[] itemPrices;
    }
}
