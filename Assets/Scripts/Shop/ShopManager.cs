using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform listArticleParent;
    [SerializeField] private GameObject articlePrefab;
    [SerializeField] private ArticleScriptable data;
    [SerializeField] private List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] private Sprite[] rareteSprite;
    protected Article selectedArticle;
    [SerializeField] public GameObject buyPannel;
    [SerializeField] public ArticleType typeArticle;

    private void Start()
    {
        RefreshShop();
    }
    
    public void RefreshShop()
    {
        listArticleParent.GetComponent<ContentScaleModifier>().ResetSize();

        foreach (Transform child in listArticleParent)
            Destroy(child.gameObject);

        articlesList.Clear();

        foreach (Article article in data.articles)
            AddItem(article);

        listArticleParent
            .GetComponent<ContentScaleModifier>()
            .SetSize(data.articles.Count);
    }

    public void AddItem(Article article)
    {
        GameObject instance = Instantiate(articlePrefab, listArticleParent);
        articlesList.Add(instance);

        ArticleUnit uiArticle = instance.GetComponent<ArticleUnit>();
        
        uiArticle.logoImage.sprite = article.logo;
        uiArticle.backGround.sprite = ChangeBackGroundRarete(article.Rarete);

        uiArticle.buyBtn.onClick.AddListener(() => UpdatePrice(article.price, article.title));
        uiArticle.buyBtn.onClick.AddListener(() => selectedArticle = article);
        
        uiArticle.articleType = typeArticle;
        if (typeArticle != ArticleType.None)
            uiArticle.id = article.id;
    }

    private void UpdatePrice(int articlePrice, string articleTitle)
    {
        Debug.Log("Update ui price");
        
        buyPannel.SetActive(true);
        
        buyPannel.transform.GetChild(2).GetComponent<Text>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<Text>().text = articleTitle;
    }

    private void BuyArticle()
    { }

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
