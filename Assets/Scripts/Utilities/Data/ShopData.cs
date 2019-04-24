using FantasyErrand.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FantasyErrand.Data
{
    [CreateAssetMenu(fileName = "New Data", menuName = "Data/Shop Data")]
    public class ShopData : ScriptableObject
    {
        public ItemID itemId = 0;
        public Sprite itemImage;
        public string itemName;
        [TextArea]
        public string itemDescription;
        public int upgrades;
        public float[] itemPrices;
    }
}
