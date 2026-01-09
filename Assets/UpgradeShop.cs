using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    public Dictionary<MiniGames, int> prices;
    public List<MiniGamesPrices> priceList;

    [SerializeField] private Text[] textPrices;
    [SerializeField] private Button[] buttonsPrices;
    [SerializeField] private Button[] buttonsTricotPrices;
    [SerializeField] private ModelDrawSO[] modelsTricot;
    
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    bool isShowingCantBuy = false;
    
    [SerializeField] GameObject barBuy;
    [SerializeField] TextMeshProUGUI buyPrice;
    [SerializeField] TextMeshProUGUI nameUpgrade;
    [SerializeField] Button buttonBuy;
    public TricotManager tricot;

    public void Awake()
    {
        prices = new Dictionary<MiniGames, int>();

        for (int i = 0; i < priceList.Count; i++)
        {
            prices.Add(priceList[i].game, priceList[i].price);
        }

        for (int i = 0; i < textPrices.Length; i++)
        {
            textPrices[i].text = priceList[i].price.ToString();
        }
    }
    
    public void AddLevelTonte() => UpdatePrice(prices[MiniGames.Tonte], nameof(MiniGames.Tonte),MiniGames.Tonte);
    public void AddLevelClean() => UpdatePrice(prices[MiniGames.Nettoyage], nameof(MiniGames.Nettoyage),MiniGames.Nettoyage);
    public void AddLevelSortie() => UpdatePrice(prices[MiniGames.Sortie], nameof(MiniGames.Sortie),MiniGames.Sortie);
    public void AddLevelRentree() => UpdatePrice(prices[MiniGames.Rentree], nameof(MiniGames.Rentree),MiniGames.Rentree);
    public void AddLevelAbreuvoir() => UpdatePrice(prices[MiniGames.Abreuvoir], nameof(MiniGames.Abreuvoir),MiniGames.Abreuvoir);

    public void AddTricot(int id)
    {
        print("buiy tricot");
        UpdatePrice(modelsTricot[id].unlockPrice, modelsTricot[id].name, MiniGames.None, id);
    }


    void UpdatePrice(int articlePrice, string articleTitle, MiniGames game = MiniGames.None, int id = 0)
    {
        articleTitle = "Améliorer " + articleTitle;
        
        Debug.Log("Update ui price");
        
        barBuy.SetActive(true);
        
        buyPrice.text = articlePrice.ToString();
        nameUpgrade.text = articleTitle;
        
        buttonBuy.onClick.RemoveAllListeners();
        
        buttonBuy.onClick.AddListener(() => Buy(articlePrice, game, id));
        buttonBuy.onClick.AddListener(() => AudioManager.instance.ButtonClick());
    }

    public void Buy(int cost, MiniGames game = MiniGames.None, int id = 0)
    {
        if (PlayerMoney.instance.isEnoughtMoney(cost))
        {
            PlayerMoney.instance.RemoveMoney(cost);
            
            switch (game)
            {
                case MiniGames.Rentree:
                    GameManager.instance.ActivatedDog();
                    break;
                case MiniGames.Tonte:
                    GameData.instance.AddLevelTonte();
                    break;
                case MiniGames.Nettoyage:
                    GameData.instance.AddLevelClean();
                    break;
                case MiniGames.Sortie:
                    GameData.instance.AddLevelSortie();
                    break;
                case MiniGames.Abreuvoir:
                    GameData.instance.AddLevelAbreuvoir();
                    break;
                default:
                    tricot.BuyNewPage(id);
                    AudioManager.instance.PlaySound(3);
            
                    buttonsTricotPrices[id].interactable = false;
                    buttonsTricotPrices[id].transform.parent.GetComponent<ArticleUpgradeUnit>().SetActive();

                    Instantiate(buyInfo, transform);
            
                    HideBarInfo();
                    return;
                    break;
            }
            
            AudioManager.instance.PlaySound(3);
            
            buttonsPrices[(int)game].interactable = false;
            buttonsPrices[(int)game].transform.parent.GetComponent<ArticleUpgradeUnit>().SetActive();

            Instantiate(buyInfo, transform);
            
            HideBarInfo();
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
    
    IEnumerator CantBuyIt()
    {
        isShowingCantBuy = true;
    
        cantBuyInfo.SetActive(true);
    
        yield return new WaitForSeconds(1.5f);
    
        cantBuyInfo.SetActive(false);
    
        isShowingCantBuy = false;
    }

    public void HideBarInfo()
    {
        barBuy.SetActive(false);
    }
}

[Serializable]
public class MiniGamesPrices
{
    public MiniGames game;
    public int price;
}
