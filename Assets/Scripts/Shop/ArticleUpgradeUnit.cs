using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ArticleUpgradeUnit : ArticleUnit
{
    public Sprite IsUpgradeImage;
    public Sprite IsUnupgradeImage;
    public bool isUpgrade = false;
    public GameObject price;
    public Image upgradeEndImage;

    public void Update()
    {
        logoImage.sprite = isUpgrade ?  IsUnupgradeImage : IsUpgradeImage;
        upgradeEndImage.gameObject.SetActive(isUpgrade);
        price.SetActive(!isUpgrade);
    }

    public void SetActive() => isUpgrade = true;
}
