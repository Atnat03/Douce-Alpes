using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaucheBoutique : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image[] articles;
    [SerializeField] private ArticleScriptable data;
    
    [Header("Buy")]
    [SerializeField] public GameObject buyPannel;
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    protected bool isShowingCantBuy = false;
    
    public Article currentArticle;
    private IsSelectGauchBoutique selectedArticle;
    public ExterieurShopManager exterieurShopManager;
    
    [HideInInspector]public ArticleActivableUnit currentEquippedGrange;
    [HideInInspector]public ArticleActivableUnit currentEquippedBarriere;
    [HideInInspector]public ArticleActivableUnit currentEquippedNiche;
    [HideInInspector]public ArticleActivableUnit currentEquippedShop;
    [HideInInspector]public ArticleActivableUnit currentEquippedTricot;

    void Start()
    {
        currentEquippedGrange = new ArticleActivableUnit();
        currentEquippedGrange.id = 0;
        currentEquippedGrange.articleType = ArticleType.Grange;
        
        currentEquippedBarriere = new ArticleActivableUnit();
        currentEquippedBarriere.id = 3;
        currentEquippedBarriere.articleType = ArticleType.Barriere;
        
        currentEquippedNiche = new ArticleActivableUnit();
        currentEquippedNiche.id = 6;
        currentEquippedNiche.articleType = ArticleType.Niche;
        
        currentEquippedShop = new ArticleActivableUnit();
        currentEquippedShop.id = 9;
        currentEquippedShop.articleType = ArticleType.Shop;
        
        currentEquippedTricot = new ArticleActivableUnit();
        currentEquippedTricot.id = 12;
        currentEquippedTricot.articleType = ArticleType.Tricot;
        
        ChangeArticles();
    }

    public void ChangeCurrentArticle(
        ArticleActivableUnit grange, ArticleActivableUnit barrier, ArticleActivableUnit  niche, ArticleActivableUnit shop, ArticleActivableUnit tricot)
    {
        currentEquippedGrange = grange;
        currentEquippedBarriere = barrier;
        currentEquippedNiche = niche;
        currentEquippedShop = shop;
        currentEquippedTricot = tricot;
    }

    public void HideBarInfo()
    {
        buyPannel.SetActive(false);
    }

    private void ChangeArticles()
    {
        for (int i = 0; i < articles.Length; i++)
        {
            Article randomArticle = data.articles[Random.Range(0, data.articles.Count)];
            articles[i].sprite = randomArticle.logo;
            
            string numberStr = new string(articles[i].transform.name.Where(char.IsDigit).ToArray());
            int number = int.Parse(numberStr);
            
            if(currentEquippedGrange.id == randomArticle.id && currentEquippedGrange.articleType == randomArticle.type
               || currentEquippedBarriere.id == randomArticle.id && currentEquippedBarriere.articleType == randomArticle.type
                || currentEquippedNiche.id == randomArticle.id && currentEquippedNiche.articleType == randomArticle.type
                || currentEquippedShop.id == randomArticle.id && currentEquippedShop.articleType == randomArticle.type
                || currentEquippedTricot.id == randomArticle.id && currentEquippedTricot.articleType == randomArticle.type)
            {
                articles[i].GetComponent<IsSelectGauchBoutique>().isActive = true;
                return;
            }

            articles[i].GetComponent<Button>().onClick.AddListener(() => 
                ChangeSprite(randomArticle.id,
                    number-1));
        }
    }

    public void ChangeSprite(int id, int idName)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sprites[idName];
        currentArticle = data.articles.Find(x => x.id == id);
        UpdatePrice(currentArticle.price, currentArticle.title);
        
        selectedArticle = articles[id].GetComponent<IsSelectGauchBoutique>();
    }
    
    
    public void BuyArticle()
    {
        if (selectedArticle.isActive)
            return;
        
        if (PlayerMoney.instance.isEnoughtMoney(currentArticle.price))
        {
            Transform t = Instantiate(buyInfo, transform).transform;
            t.localScale = Vector3.one;
            
            selectedArticle.isActive = true;
            t.GetComponent<Button>().interactable = false;
        }
        else
        {
            if (!isShowingCantBuy)
            {
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

    protected void UpdatePrice(int articlePrice, string articleTitle)
    {
        Debug.Log("Update ui price");
        
        buyPannel.SetActive(true);
        
        buyPannel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = articleTitle;
    }
}
