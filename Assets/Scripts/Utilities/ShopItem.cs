using DG.Tweening;
using FantasyErrand;
using FantasyErrand.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    [HideInInspector]
    public ShopData data;
    public TextMeshProUGUI itemNameText, itemDescriptionText, itemPriceText, upgradeText;
    public Image itemImage;

    internal ShopManager manager;

    public void Click()
    {
        transform.DOScale(0.9f, 0.1f);
        manager.Select(this);
    }

    public void ClickUp()
    {
        transform.DOScale(1f, 0.1f);
        
    }
}
