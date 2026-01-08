using System;
using UnityEngine;
using UnityEngine.UI;

public class ArticleSkinUnit : ArticleUnit
{
    public Text stack;
    [SerializeField]private int n = 0;

    public void UpdateStack()
    {
        if (articleType == ArticleType.Hat)
            n = SkinAgency.instance.dicoHatSkinStack[id];
        if (articleType == ArticleType.Clothe)
            n = SkinAgency.instance.dicoClotheSkinStack[id];
        
        stack.gameObject.SetActive(n != 0);
        
        stack.text = "x" + n;
        
        print(n);
    }
}
