using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExterieurShopManager : ShopManager
{
    public GameObject EquipéPannel;
    public Button buttonEquip;
    ArticleActivableUnit selectedUIArticle;

    // Deux variables pour gérer le skin équipé par type
    ArticleActivableUnit currentEquippedGrange;
    ArticleActivableUnit currentEquippedBarriere;

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

        // On choisit la référence du skin actuel selon le type
        ArticleActivableUnit currentEquipped = selectedUIArticle.articleType switch
        {
            ArticleType.Grange => currentEquippedGrange,
            ArticleType.Barriere => currentEquippedBarriere,
            _ => null
        };

        // Si c'est déjà l'article équipé, on ne fait rien
        if (selectedUIArticle == currentEquipped)
            return;

        // Déséquipe l'ancien skin du même type
        if (currentEquipped != null)
            currentEquipped.isActive = false;

        // Équipe le nouveau skin
        selectedUIArticle.isActive = true;

        // Applique le skin
        if (selectedUIArticle.articleType == ArticleType.Grange)
        {
            SkinAgency.instance.SetSkinGrange(selectedUIArticle.id);
            currentEquippedGrange = selectedUIArticle;
        }
        else if (selectedUIArticle.articleType == ArticleType.Barriere)
        {
            SkinAgency.instance.SetSkinBarriere(selectedUIArticle.id);
            currentEquippedBarriere = selectedUIArticle;
        }

        UpdatePrice(selectedArticle.price, selectedArticle.title);
    }

    private new void Buy()
    {
        if (PlayerMoney.instance.isEnoughtMoney(selectedArticle.price))
        {
            Instantiate(buyInfo, transform.parent);
            
            PlayerMoney.instance.RemoveMoney(selectedArticle.price);

            selectedUIArticle.isBuying = true;

            Debug.Log("Buy");

            // Active le skin après achat
            Activate();
        }
        else
        {
            if (!isShowingCantBuy)
            {
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

        // Si id = 0, on équipe ce skin par défaut
        if (uiArticle.id == 0)
        {
            uiArticle.isBuying = true;
            uiArticle.isActive = true;

            if (uiArticle.articleType == ArticleType.Grange)
                currentEquippedGrange = uiArticle;
            else if (uiArticle.articleType == ArticleType.Barriere)
                currentEquippedBarriere = uiArticle;
        }
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
