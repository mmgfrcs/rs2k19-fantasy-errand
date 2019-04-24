using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class ShopItem : MonoBehaviour {
    [HideInInspector]
    public int itemId;
    public TextMeshProUGUI itemNameText, itemDescriptionText, itemPriceText, upgradeText;
    public Image itemImage;

    public void Click()
    {
        transform.DOScale(0.9f, 0.1f);
    }

    public void ClickUp()
    {
        transform.DOScale(1f, 0.1f);
        GetComponent<Image>().DOFade(Mathf.Abs(GetComponent<Image>().color.a - 1), 0.2f);
    }
}
