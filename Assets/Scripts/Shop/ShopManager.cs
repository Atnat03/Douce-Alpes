using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] private Transform listArticleParent;
    [SerializeField] private GameObject articlePrefab;

    [SerializeField] private ArticleScriptable articleData;

    public void Start()
    {
        foreach (Article article in articleData.articles)
        {
            AddItem(article);
        }

        listArticleParent.GetComponent<ContentScaleModifier>().SetSize();
    }

    public void AddItem(Article article)
    {
        GameObject instance = Instantiate(articlePrefab,  listArticleParent.transform);
        articlesList.Add(instance);
        
        ArticleUnit uiArticle = instance.GetComponent<ArticleUnit>();
        uiArticle.titleTxt.text = article.title;
        uiArticle.priceTxt.text = article.price.ToString();
        uiArticle.logoImage.sprite = article.logo;
        
        uiArticle.buyBtn.onClick.AddListener(() => BuyArticle());
    }

    public void BuyArticle()
    {
        Debug.Log("Un article a ete acheter");
    }
}
