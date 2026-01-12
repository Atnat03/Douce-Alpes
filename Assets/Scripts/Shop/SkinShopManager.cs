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

            AudioManager.instance.PlaySound(3, 1f, 0.25f); 
            
            if(selectedArticle.type == ArticleType.Hat)
                SkinAgency.instance.AddHatSkinInstance(selectedArticle.id);
            else if(selectedArticle.type == ArticleType.Clothe)
                SkinAgency.instance.AddClotheSkinInstance(selectedArticle.id);

            selectedArticleUI.UpdateStack();
        }
        else
        {
            AudioManager.instance.PlaySound(5);
            StartCoroutine(CantBuyIt());
        }
    }
    
    protected override void AddItem(Article article)
    {
        GameObject instance = Instantiate(articlePrefab, listArticleParent);
        articlesList.Add(instance);

        ArticleSkinUnit uiArticle = instance.GetComponent<ArticleSkinUnit>();
        
        uiArticle.id = article.id;
        uiArticle.logoImage.sprite = article.logo;
        uiArticle.backGround.sprite = ChangeBackGroundRarete(article.Rarete);

        uiArticle.buyBtn.onClick.AddListener(() => UpdatePrice(article.price, article.title, article.id));
        uiArticle.buyBtn.onClick.AddListener(() =>
        {
            selectedArticle = article;
            selectedArticleUI = uiArticle;
        });        
        uiArticle.articleType = article.type;
        uiArticle.price.text = article.price.ToString();
    }
}
