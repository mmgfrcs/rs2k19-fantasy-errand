using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using FantasyErrand.Data;
using FantasyErrand;

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
