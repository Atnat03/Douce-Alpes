using UnityEngine;

public class SkinShopManager : ShopManager
{
    private void BuyArticle()
    {
        PlayerMoney.instance.AddMoney(-selectedArticle.price);
    }
}
