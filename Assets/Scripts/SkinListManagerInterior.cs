using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManagerInterior : MonoBehaviour
{
    List<SkinUnit> skinListHat = new();
    private List<SkinUnit> skinListClothe = new ();

    private SheepSkinManager parentSheep;
    
    [SerializeField] private Transform withWoolSkins;
    [SerializeField] private Transform withoutWoolSkins;
    private Transform t;
    
    private void Update()
    {
        withoutWoolSkins.gameObject.SetActive(!parentSheep.hasLaine);
        withWoolSkins.gameObject.SetActive(parentSheep.hasLaine);
    }
    
    private void Start()
    {
        parentSheep =  transform.parent.GetComponent<SheepSkinManager>();
        
        t = parentSheep.hasLaine ?  withWoolSkins : withoutWoolSkins;

        for (int i = 0; i < t.GetChild(0).childCount; i++)
        {
            skinListHat.Add(t.GetChild(0).GetChild(i).GetComponent<SkinUnit>());
        }
        
        for (int i = 0; i < t.GetChild(1).childCount; i++)
        {
            skinListClothe.Add(t.GetChild(1).GetChild(i).GetComponent<SkinUnit>());
        }
        
        UpdateSkinListHat(parentSheep.GetCurrentSkinHat());
        UpdateSkinListClothe(parentSheep.GetCurrentSkinClothe());
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