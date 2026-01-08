using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorShopManager : ShopManager
{
    public GameObject EquipéPannel;
    public Button buttonEquip;
    ArticleActivableUnit selectedUIArticle;

    public Text TextTitleSelect;

    new void Start()
    {
        base.Start();
        EquipéPannel.SetActive(false);
    }

    private void Update()
    {
        if (selectedUIArticle == null || buttonEquip == null) 
            return;

        buttonEquip.GetComponent<Image>().color = selectedUIArticle.isActive ? Color.red : Color.green;
    }

    public void Activate()
    {
        if (selectedUIArticle == null)
            return;

        selectedUIArticle.isActive = !selectedUIArticle.isActive;

        SkinAgency.instance.SetSkinInterior(selectedUIArticle.id);

        UpdatePrice(selectedArticle.price, selectedArticle.title);
    }

    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform.parent);
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);
            
            AudioManager.instance.PlaySound(3);

            selectedUIArticle.isBuying = true;

            Debug.Log("Buy");

            Activate();
        }
        else
        {
            if (!isShowingCantBuy)
            {
                AudioManager.instance.PlaySound(5);
                StartCoroutine(CantBuyIt());
            }
        }
    }

    protected override void AddItem(Article article)
    {
        GameObject instance = Instantiate(articlePrefab, listArticleParent);
        articlesList.Add(instance);

        ArticleActivableUnit uiArticle = instance.GetComponent<ArticleActivableUnit>();
        uiArticle.logoImage.sprite = article.logo;
        uiArticle.id = article.id;
        uiArticle.backGround.sprite = ChangeBackGroundRarete(article.Rarete);
        uiArticle.articleType = article.type;

        uiArticle.buyBtn.onClick.AddListener(() => selectedArticle = article);
        uiArticle.buyBtn.onClick.AddListener(() => selectedUIArticle = uiArticle);
        uiArticle.buyBtn.onClick.AddListener(() => UpdatePrice(article.price, article.title));
    }

    protected new void UpdatePrice(int articlePrice, string articleTitle)
    {
        EquipéPannel.SetActive(false);
        buyPannel.SetActive(false);

        if (selectedUIArticle.isBuying)
        {
            EquipéPannel.SetActive(true);
            TextTitleSelect.text = articleTitle;
        }
        else
        {
            buyPannel.SetActive(true);
            buyPannel.transform.GetChild(2).GetComponent<Text>().text = articlePrice.ToString();
            buyPannel.transform.GetChild(3).GetComponent<Text>().text = articleTitle;
        }
    }
}
