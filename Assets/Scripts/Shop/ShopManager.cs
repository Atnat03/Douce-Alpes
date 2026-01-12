using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] protected Transform listArticleParent;
    [SerializeField] protected GameObject articlePrefab;
    [SerializeField] protected ArticleScriptable data;
    [SerializeField] protected List<GameObject> articlesList = new List<GameObject>();
    [SerializeField] protected Sprite[] rareteSprite;
    protected ArticleSkinUnit selectedArticleUI;
    protected Article selectedArticle;
    [SerializeField] public GameObject buyPannel;
    public Article currentArticle;
    
    [Header("Buy")]
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    protected bool isShowingCantBuy = false;

    protected void Start()
    {
        RefreshShop();
    }
    
    void RefreshShop()
    {
        foreach (Transform child in listArticleParent)
            Destroy(child.gameObject);

        articlesList.Clear();

        foreach (Article article in data.articles)
            AddItem(article);
    }
    
    protected virtual void AddItem(Article article)
    { }

    protected void UpdatePrice(int articlePrice, string articleTitle, int id)
    {
        Debug.Log("Update ui price");
        
        buyPannel.SetActive(true);

        foreach (Transform child in listArticleParent)
        {
            if (child.GetComponent<ArticleUnit>().id == id)
            {
                child.GetComponent<ArticleUnit>().outline.gameObject.SetActive(true);
            }
            else
            {
                child.GetComponent<ArticleUnit>().outline.gameObject.SetActive(false);
            }
        }
        
        buyPannel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = articleTitle;
    }
    
    public void HideBarInfo()
    {
        buyPannel.SetActive(false);
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
