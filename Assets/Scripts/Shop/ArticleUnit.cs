using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ArticleType
{
    Hat, Clothe, Grange, Barriere,Niche, Shop, Tricot, Interior,Amelioration, None
}

public class ArticleUnit : MonoBehaviour
{
    public Image logoImage;
    public Button buyBtn;
    public Image backGround;
    public ArticleType articleType;
    public int id;
    public Image outline;
}
