using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManager : MonoBehaviour
{
    List<SkinUnit> skinListHat = new();
    private List<SkinUnit> skinListClothe = new ();

    private Sheep parentSheep;
    
    private void Start()
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
}
