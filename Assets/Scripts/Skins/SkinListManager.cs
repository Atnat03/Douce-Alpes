using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManager : MonoBehaviour
{
    List<SkinUnit> skinListHat = new();
    private List<SkinUnit> skinListClothe = new ();

    private Sheep parentSheep;
    
    public void Initalize()
    {
        parentSheep =  transform.parent.GetComponent<Sheep>();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            skinListHat.Add(transform.GetChild(0).GetChild(i).GetComponent<SkinUnit>());
        }
        
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            skinListClothe.Add(transform.GetChild(1).GetChild(i).GetComponent<SkinUnit>());
        }
        
        UpdateSkinListHat(parentSheep.currentSkinHat);
        UpdateSkinListClothe(parentSheep.currentSkinClothe);
    }

    public void UpdateSkinListHat(int currentSkinHat)
    {
        foreach (SkinUnit skinUnit in skinListHat)
        {
            skinUnit.gameObject.SetActive(false);

            if (currentSkinHat == skinUnit.id)
            {
                skinUnit.gameObject.SetActive(true);
            }
        }
    }
    
    public void UpdateSkinListClothe(int currentSkinClothe)
    {
        foreach (SkinUnit skinUnit in skinListClothe)
        {
            skinUnit.gameObject.SetActive(false);

            if (currentSkinClothe == skinUnit.id)
            {
                skinUnit.gameObject.SetActive(true);
            }
        }
    }

    public bool HasCombo(int hatId, int clotheId)
    {
        SkinCombo sH = GetCombo(hatId, skinListHat);
        SkinCombo sC = GetCombo(clotheId, skinListClothe);

        if (sH == SkinCombo.Default || sC == SkinCombo.Default)
            return false;
        
        return sH == sC;
    }

    public SkinCombo GetCombo(int i, List<SkinUnit> list)
    {
        foreach (SkinUnit s in list)
        {
            if (s.id == i)
                return s.combo;
        }

        return SkinCombo.Default;
    }

    public NatureType GetNatureFromCombo(int hatId)
    {
        SkinCombo combo = GetCombo(hatId, skinListHat);
        NatureType type = NatureType.Standard;
        
        switch (combo)
        {
            case SkinCombo.Sherif:
                type =  NatureType.Solitaire;
                break;
            case SkinCombo.Leader:
                type = NatureType.Dominant;
                break;
            case SkinCombo.Chill:
                type = NatureType.Standard;
                break;
            case SkinCombo.Artiste:
                type = NatureType.Solitaire;
                break;
            case SkinCombo.Intelo:
                type = NatureType.Peureux;
                break;
        }

        return type;
    }
}
