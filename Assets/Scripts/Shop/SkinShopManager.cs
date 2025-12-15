using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopManager : ShopManager
{
    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform);
            Debug.Log("Buy");
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);
                    
            if(selectedArticle.type == ArticleType.Hat)
                SkinAgency.instance.AddHatSkinInstance(selectedArticle.id);
            else if(selectedArticle.type == ArticleType.Clothe)
                SkinAgency.instance.AddClotheSkinInstance(selectedArticle.id);
        }
        else
        {
            StartCoroutine(CantBuyIt());
        }
    }
}
