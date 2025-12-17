using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorShopManager : ShopManager
{
    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform);
            Debug.Log("Buy");
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);
                    
            //SkinAgency.instance.SetSkinBarriere(selectedArticle.id);
        }
        else
        {
            StartCoroutine(CantBuyIt());
        }
    }
}
