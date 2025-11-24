using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform listArticleParent;
    [SerializeField] private GameObject articlePrefab;
    [SerializeField] private ArticleScriptable data; // Unique ScriptableObject
    [SerializeField] private List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] private Sprite[] rareteSprite;

    private void Start()
    {
        RefreshShop();
    }
    
    public void RefreshShop()
    {
        // Reset UI container
        listArticleParent.GetComponent<ContentScaleModifier>().ResetSize();

        foreach (Transform child in listArticleParent)
            Destroy(child.gameObject);

        articlesList.Clear();

        // Créer les articles
        foreach (Article article in data.articles)
            AddItem(article);

        // Adapter la taille du scroll
        listArticleParent
            .GetComponent<ContentScaleModifier>()
            .SetSize(data.articles.Count);
    }

    private void AddItem(Article article)
    {
        GameObject instance = Instantiate(articlePrefab, listArticleParent);
        articlesList.Add(instance);

        ArticleUnit uiArticle = instance.GetComponent<ArticleUnit>();
        /*uiArticle.titleTxt.text = article.title;
        uiArticle.priceTxt.text = article.price.ToString();*/

        uiArticle.logoImage.sprite = article.logo;
        uiArticle.backGround.sprite = ChangeBackGroundRarete(article.Rarete);

        //uiArticle.buyBtn.onClick.AddListener(() => BuyArticle(article));
    }

    private Sprite ChangeBackGroundRarete(RareteItem articleRarete)
    {
        return articleRarete switch
        {
            RareteItem.Commum => rareteSprite[0],
            RareteItem.Rare => rareteSprite[1],
            RareteItem.Legendaire => rareteSprite[2],
        };
    }

}
