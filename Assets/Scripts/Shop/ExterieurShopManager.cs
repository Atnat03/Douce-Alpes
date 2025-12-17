using UnityEngine;

public class ExterieurShopManager : ShopManager
{
    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform);
            Debug.Log("Buy");
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);
            
            if(selectedArticle.type == ArticleType.Grange)
                SkinAgency.instance.SetSkinGrange(selectedArticle.id);
            if(selectedArticle.type == ArticleType.Barriere)
                SkinAgency.instance.SetSkinBarriere(selectedArticle.id);
        }
        else
        {
            StartCoroutine(CantBuyIt());
        }
    }
}
