using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CenterBoutique : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image[] articles;
    [SerializeField] private ArticleScriptable data;
    [SerializeField] private List<Article> hats;
    [SerializeField] private List<Article> clothe;
    
    [Header("Buy")]
    [SerializeField] public GameObject buyPannel;
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    protected bool isShowingCantBuy = false;
    
    public Article currentArticle;

    void Start()
    {
        ChangeArticles();
    }

    public void HideBarInfo()
    {
        buyPannel.SetActive(false);
    }

    private void ChangeArticles()
    {
        foreach (Article article in data.articles)
        {
            if(article.type == ArticleType.Hat)
                hats.Add(article);
            else if (article.type == ArticleType.Clothe)
            {
                clothe.Add(article);
            }
        }
        
        for (int i = 0; i < articles.Length; i++)
        {
            bool pickHat = Random.value < 0.5f;

            List<Article> l = pickHat ? hats : clothe;

            if (l.Count == 0)
                continue;

            Article randomArticle = l[Random.Range(0, l.Count)];

            articles[i].sprite = randomArticle.logo;

            string numberStr = new string(articles[i].transform.name.Where(char.IsDigit).ToArray());
            int number = int.Parse(numberStr);

            Article captured = randomArticle;

            articles[i].GetComponent<Button>().onClick.AddListener(() =>
                ChangeSprite(captured.id, number - 1, captured.type == ArticleType.Hat)
            );
        }

    }

    public void ChangeSprite(int id, int idName, bool hat = false)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sprites[idName];

        if (hat)
        {
            currentArticle = hats[id];
        }
        else
        {
            currentArticle = clothe[id];
        }
        
        UpdatePrice(currentArticle.price, currentArticle.title);
    }
    
    
    public void BuyArticle()
    {
        if (PlayerMoney.instance.isEnoughtMoney(currentArticle.price))
        {
            Instantiate(buyInfo, transform);
            Debug.Log("Buy");
            
            PlayerMoney.instance.RemoveMoney(currentArticle.price);

            AudioManager.instance.PlaySound(3, 1f, 0.25f); 
            
            if(currentArticle.type == ArticleType.Hat)
                SkinAgency.instance.AddHatSkinInstance(currentArticle.id);
            else if(currentArticle.type == ArticleType.Clothe)
                SkinAgency.instance.AddClotheSkinInstance(currentArticle.id);
        }
        else
        {
            AudioManager.instance.PlaySound(5);
            StartCoroutine(CantBuyIt());
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

    protected void UpdatePrice(int articlePrice, string articleTitle)
    {
        Debug.Log("Update ui price");
        
        buyPannel.SetActive(true);
        
        buyPannel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = articleTitle;
    }
}
