using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterBoutique : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image[] articles;
    [SerializeField] private ArticleScriptable[] data;
    [SerializeField] private List<Article> allArticles;

    void Start()
    {
        GetAllArticles();
        ChangeSprite(0);
        ChangeArticles();
    }

    private void GetAllArticles()
    {
        foreach (ArticleScriptable item in data)
        {
            foreach (Article a in item.articles)
            {
                allArticles.Add(a);
            }
        }
    }

    private void ChangeArticles()
    {
        for (int i = 0; i < articles.Length; i++)
        {
            Article randomArticle = allArticles[Random.Range(0, allArticles.Count)];
            articles[i].sprite = randomArticle.logo;
            //articles[i].GetComponent<Button>().onClick.AddListener(() => BuyArticle(randomArticle));
        }
    }

    public void ChangeSprite(int id)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sprites[id];
    }
    
    
    private void BuyArticle(Article article)
    {
        InventoryManager.instance.AddItem(article);
        Debug.Log($"{article.title} a ete acheté !");
    }
}
