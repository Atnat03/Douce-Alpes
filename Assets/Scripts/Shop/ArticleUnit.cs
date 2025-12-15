using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ArticleType
{
    Hat, Clothe, Grange, Amelioration, None
}

public class ArticleUnit : MonoBehaviour
{
    public Image logoImage;
    public Button buyBtn;
    public Image backGround;
    public Text stack;
    public ArticleType articleType;
    public int id;

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
