using System;
using UnityEngine;
using UnityEngine.UI;

public class ArticleActivableUnit : ArticleUnit
{
    public Image IsActiveImage;
    public bool isActive;
    public bool isBuying = false;
    public Text textPrice;
    public Image Price;
    public Image Selected;
    public Image Check;
    public Sprite[] spriteSelect;

    public void Update()
    {        
        IsActiveImage.gameObject.SetActive(isActive);
        
        Price.gameObject.SetActive(!isBuying);
        Selected.gameObject.SetActive(isBuying);
        Check.gameObject.SetActive(isActive);
        
        Selected.sprite = isActive ? spriteSelect[0] : spriteSelect[1];
    }

    public bool SetActive() => isActive = !isActive;
}
