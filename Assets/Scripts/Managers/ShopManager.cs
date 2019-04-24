using FantasyErrand.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FantasyErrand
{
    public class ShopManager : MonoBehaviour
    {
        public Transform shopList;
        public GameObject shopItemPrefab;
        public ShopData[] data;
        List<ShopItem> shopItems = new List<ShopItem>();
        GameObject selected;

        // Use this for initialization
        void Start()
        {
            foreach(var d in data)
            {
                GameObject go = Instantiate(shopItemPrefab, shopList);
                ShopItem item = go.GetComponent<ShopItem>();
                int upgrades = GameDataManager.instance.Data.UpgradeLevels.Get((int)d.itemId);
                item.itemNameText.text = d.itemName;
                item.itemPriceText.text = string.Format("${0:N0}", d.itemPrices[upgrades]);
                item.itemDescriptionText.text = d.itemDescription;
                item.itemImage.sprite = d.itemImage;
                item.upgradeText.text = $"{upgrades}/{d.upgrades}";
                shopItems.Add(item);
            }
        }

        public void Select(GameObject sender)
        {
            selected = sender;
            sender.transform.DOScale(0.9f, 0.2f);
        }

        void DrawShopItems()
        {

        }

        public void BuyItem()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

