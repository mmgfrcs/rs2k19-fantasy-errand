using DG.Tweening;
using FantasyErrand.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FantasyErrand
{
    public class ShopManager : MonoBehaviour
    {
        public Transform shopList;
        public GameObject shopItemPrefab;
        public Button buyButton;
        public TextMeshProUGUI moneyText;
        public ShopData[] data;
        public Image fader;
        public SceneChanger changer;

        List<ShopItem> shopItems = new List<ShopItem>();
        ShopItem selected;

        TextMeshProUGUI btnText;

        void Start()
        {
            fader.color = Color.black;
            fader.DOFade(0f, 1f).onComplete += () => fader.gameObject.SetActive(false);
            btnText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Buy";
            buyButton.interactable = false;
            moneyText.text = $"<sprite=0 tint=1> {GameDataManager.instance.Data.Coins.ToString("n0")}";
            DrawShopItems();
        }

        public void Select(ShopItem sender)
        {
            if (selected != null) {
                selected.GetComponent<Image>().DOFade(0, 0.2f);
            }
            sender.GetComponent<Image>().DOFade(1, 0.2f);
            selected = sender;

            int upgrades = GameDataManager.instance.Data.UpgradeLevels.Get(selected.data.itemId);
            if(upgrades == selected.data.upgrades)
            {
                btnText.text = $"Max Level";
                buyButton.interactable = false;
            }
            else
            {
                btnText.text = $"<sprite=0 tint=1> {selected.data.itemPrices[upgrades].ToString("n0")}\nBuy";

                if (GameDataManager.instance.Data.Coins >= selected.data.itemPrices[upgrades])
                    buyButton.interactable = true;
                else buyButton.interactable = false;
            }
        }

        void DrawShopItems()
        {
            int i = 0;
            for(; i < shopItems.Count; i++)
            {
                if (i >= data.Length) shopItems[i].gameObject.SetActive(false);
                else DrawTextInItems(shopItems[i], data[i]);
            }

            if(i < data.Length)
            {
                for(; i < data.Length; i++)
                {
                    GameObject go = Instantiate(shopItemPrefab, shopList);
                    ShopItem item = go.GetComponent<ShopItem>();
                    DrawTextInItems(item, data[i]);
                    shopItems.Add(item);
                }
            }
        }

        void DrawTextInItems(ShopItem item, ShopData shopData)
        {
            item.manager = this;
            int upgrades = GameDataManager.instance.Data.UpgradeLevels.Get(shopData.itemId);
            item.data = shopData;
            item.itemNameText.text = shopData.itemName;
            if (upgrades == shopData.upgrades) item.itemPriceText.text = "Max Level";
            else item.itemPriceText.text = GameDataManager.instance.Data.Coins >= shopData.itemPrices[upgrades] ? string.Format("${0:N0}", shopData.itemPrices[upgrades]) : string.Format("<color=red>${0:N0}</color>", shopData.itemPrices[upgrades]);
            item.itemDescriptionText.text = shopData.itemDescriptions[upgrades];
            item.itemImage.sprite = shopData.itemImage;
            item.upgradeText.text = $"{upgrades}/{shopData.upgrades}";
        }

        public void BuyItem()
        {
            int upgrades = GameDataManager.instance.Data.UpgradeLevels.Get(selected.data.itemId);
            print($"Bought {selected.data.itemName} for {selected.data.itemPrices[upgrades]} coins");
            
            GameDataManager.instance.Data.Coins -= Mathf.RoundToInt(selected.data.itemPrices[upgrades]);
            GameDataManager.instance.Data.UpgradeLevels.LevelUp(selected.data.itemId);
            GameDataManager.instance.SaveGameDataToFile();
            moneyText.text = $"<sprite=0 tint=1> {GameDataManager.instance.Data.Coins.ToString("n0")}";
            Select(selected);
            DrawShopItems();
        }

        // Update is called once per frame
        public void OnBack()
        {
            changer.ChangeScene("Main");
        }
    }
}

