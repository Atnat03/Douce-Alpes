using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopManager : ShopManager
{
    private void BuyArticle()
    {
        PlayerMoney.instance.AddMoney(-selectedArticle.price);
        
        if(selectedArticle.type == ArticleType.Hat)
            SkinAgency.instance.AddHatSkinInstance(selectedArticle.id);
        else if(selectedArticle.type == ArticleType.Clothe)
            SkinAgency.instance.AddClotheSkinInstance(selectedArticle.id);
    }

    private void Update()
    {
        if (selectedArticle != null)
        {
            buyPannel.transform.GetChild(1).GetComponent<Button>().interactable = PlayerMoney.instance.isEnoughtMoney(selectedArticle.price);
        }
    }
}
