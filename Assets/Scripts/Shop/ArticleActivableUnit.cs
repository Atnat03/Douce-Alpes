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
        IsActiveImage.gameObject.SetActive(isActive);
    }

    public bool SetActive() => isActive = !isActive;
}
