using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteriorShopManager : ShopManager
{
    public GameObject EquipéPannel;
    public Button buttonEquip;
    ArticleActivableUnit selectedUIArticle;

    public Text TextTitleSelect;
    
    [SerializeField] public Sprite[] spriteSelect;

    new void Start()
    {
        base.Start();
        EquipéPannel.SetActive(false);
    }

    private void Update()
    {
        if (selectedUIArticle == null || buttonEquip == null) 
            return;

        buttonEquip.GetComponent<Image>().sprite = selectedUIArticle.isActive ? spriteSelect[0] : spriteSelect[1];
    }

    public void Activate()
    {
        if (selectedUIArticle == null)
            return;

        selectedUIArticle.isActive = !selectedUIArticle.isActive;

        SkinAgency.instance.SetSkinInterior(selectedUIArticle.id);

        UpdatePrice(selectedArticle.price, selectedArticle.title, selectedUIArticle.id);
    }

    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform.parent);
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);

            AudioManager.instance.PlaySound(3, 1f, 0.25f); 

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
        uiArticle.buyBtn.onClick.AddListener(() => UpdatePrice(article.price, article.title, article.id));
        
        uiArticle.textPrice.text = article.price.ToString();
    }

    protected new void UpdatePrice(int articlePrice, string articleTitle, int id)
    {
        EquipéPannel.SetActive(false);
        buyPannel.SetActive(false);

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
        
        if (selectedUIArticle.isBuying)
        {
            EquipéPannel.SetActive(true);
            TextTitleSelect.text = articleTitle;
        }
        else
        {
            buyPannel.SetActive(true);
            buyPannel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = articlePrice.ToString();
            buyPannel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = articleTitle;
        }
    }
}
