using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    public Dictionary<MiniGames, int> prices;
    public List<MiniGamesPrices> priceList;

    [SerializeField] private Text[] textPrices;
    [SerializeField] private Button[] buttonsPrices;
    
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    bool isShowingCantBuy = false;

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
    
    public void AddLevelTonte() => Buy(MiniGames.Tonte);
    public void AddLevelClean() => Buy(MiniGames.Nettoyage);
    public void AddLevelSortie() => Buy(MiniGames.Sortie);
    
    public void AddLevelRentree() => Buy(MiniGames.Rentree);
    public void AddLevelAbreuvoir() => Buy(MiniGames.Abreuvoir);

    public void Buy(MiniGames game)
    {
        if (PlayerMoney.instance.isEnoughtMoney(prices[game]))
        {
            PlayerMoney.instance.RemoveMoney(prices[game]);
            
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
            }
            
            AudioManager.instance.PlaySound(3);
            
            buttonsPrices[(int)game].interactable = false;
            buttonsPrices[(int)game].transform.parent.GetComponent<ArticleUpgradeUnit>().SetActive();

            Instantiate(buyInfo, transform);
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
}

[Serializable]
public class MiniGamesPrices
{
    public MiniGames game;
    public int price;
}
