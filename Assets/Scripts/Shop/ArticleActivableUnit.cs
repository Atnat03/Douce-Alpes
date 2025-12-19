using System;
using UnityEngine;
using UnityEngine.UI;

public class ArticleActivableUnit : ArticleUnit
{
    public Image IsActiveImage;
    public bool isActive;
    public bool isBuying = false;

    public void Update()
    {
        int n = 0;
        
        if (articleType == ArticleType.Grange)
            n = SkinAgency.instance.dicoHatSkinStack[id];
        if (articleType == ArticleType.Barriere)
            n = SkinAgency.instance.dicoClotheSkinStack[id];
        
        IsActiveImage.gameObject.SetActive(isActive);
    }

    public bool SetActive() => isActive = !isActive;
}
