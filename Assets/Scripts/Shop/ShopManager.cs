using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    
    [SerializeField] private List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] private Transform listArticleParent;
    [SerializeField] private GameObject articlePrefab;
    [SerializeField] private ArticleScriptable sheepDataArticles;


    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        SwapCategorie(sheepDataArticles);
    }

    public void SwapCategorie(ArticleScriptable articleData)
    {
        listArticleParent.GetComponent<ContentScaleModifier>().ResetSize();
        
        foreach (Transform child in listArticleParent)
        {
            Destroy(child.gameObject);
        }
        
        UpdateArticleList(articleData);
    }
    
    public void UpdateArticleList(ArticleScriptable articleData)
    {
        foreach (Article article in articleData.articles)
        {
            AddItem(article);
        }
        listArticleParent.GetComponent<ContentScaleModifier>().SetSize(articleData.articles.Count);
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
