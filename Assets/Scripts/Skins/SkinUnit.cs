using System;
using UnityEngine;

public enum SkinCombo
{
    Default,
    Sherif,
    Leader,
    Chill,
    Artiste,
    Intelo,
}


public class SkinUnit : MonoBehaviour
{
    public int id;
    public SkinCombo combo;
    public ArticleType type;
    public GameObject unlockSkin;
    public bool canPutThisSkin = false;

    private void Update()
    {
        if(type == ArticleType.Hat)
        {
            bool b = SkinAgency.instance.CheckIfEnoughtInstanceHat(id);
            unlockSkin.SetActive(b);
            canPutThisSkin = b;

        }else if(type == ArticleType.Clothe)
        {
            bool b = SkinAgency.instance.CheckIfEnoughtInstanceClothe(id);
            unlockSkin.SetActive(b);
            canPutThisSkin = b;
        }
    }
}