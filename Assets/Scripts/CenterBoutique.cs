using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CenterBoutique : MonoBehaviour
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
        for (int i = 0; i < articles.Length; i++)
        {
            Article randomArticle = data.articles[Random.Range(0, data.articles.Count)];
            articles[i].sprite = randomArticle.logo;
            
            string numberStr = new string(articles[i].transform.name.Where(char.IsDigit).ToArray());
            int number = int.Parse(numberStr);
            
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
    }
    
    
    public void BuyArticle()
    {
        if (PlayerMoney.instance.isEnoughtMoney(currentArticle.price))
        {
            Transform t = Instantiate(buyInfo, transform).transform;
            t.localScale = Vector3.one;
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
        
        buyPannel.transform.GetChild(2).GetComponent<Text>().text = articlePrice.ToString();
        buyPannel.transform.GetChild(3).GetComponent<Text>().text = articleTitle;
    }
}
