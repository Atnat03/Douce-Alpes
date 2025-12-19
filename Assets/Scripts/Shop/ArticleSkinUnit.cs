using System;
using UnityEngine;
using UnityEngine.UI;

public class ArticleSkinUnit : ArticleUnit
{
    public Text stack;

    public void Update()
    {
        int n = 0;
        
        if (articleType == ArticleType.Hat)
            n = SkinAgency.instance.dicoHatSkinStack[id];
        if (articleType == ArticleType.Clothe)
            n = SkinAgency.instance.dicoClotheSkinStack[id];
        
        stack.gameObject.SetActive(n != 0);

        stack.text = "x" + n;
    }
}
