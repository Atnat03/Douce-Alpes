using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] protected Transform listArticleParent;
    [SerializeField] protected GameObject articlePrefab;
    [SerializeField] protected ArticleScriptable data;
    [SerializeField] protected List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] protected Sprite[] rareteSprite;
    protected Article selectedArticle;
    [SerializeField] public GameObject buyPannel;
    public Article currentArticle;
    
    [Header("Buy")]
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    protected bool isShowingCantBuy = false;

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
        
        uiArticle.articleType = article.type;
    }

    protected void UpdatePrice(int articlePrice, string articleTitle)
    {
        Debug.Log("Update ui price");
        
        buyPannel.SetActive(true);
        
        buyPannel.transform.GetChild(2).GetComponent<Text>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<Text>().text = articleTitle;
    }

    public void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform.parent);
            Debug.Log("Buy");
        }
        else
        {
            if (!isShowingCantBuy)
            {
                StartCoroutine(CantBuyIt());
            }
        }
    }

    protected IEnumerator CantBuyIt()
    {
        isShowingCantBuy = true;
    
        cantBuyInfo.SetActive(true);
    
        yield return new WaitForSeconds(1.5f);
    
        cantBuyInfo.SetActive(false);
    
        isShowingCantBuy = false;
    }    
    
    
    protected Sprite ChangeBackGroundRarete(RareteItem articleRarete)
    {
        return articleRarete switch
        {
            RareteItem.Commum => rareteSprite[0],
            RareteItem.Rare => rareteSprite[1],
            RareteItem.Legendaire => rareteSprite[2],
        };
    }

}
