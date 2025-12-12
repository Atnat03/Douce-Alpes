using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinListManagerInterior : MonoBehaviour
{
    private List<SkinUnit> skinListHatWithWool = new();
    private List<SkinUnit> skinListClotheWithWool = new();
    private List<SkinUnit> skinListHatWithoutWool = new();
    private List<SkinUnit> skinListClotheWithoutWool = new();

    [SerializeField] SheepSkinManager parentSheep;
    
    [SerializeField] private Transform withWoolSkins;
    [SerializeField] private Transform withoutWoolSkins;
    
    private void Start()
    {
        skinListHatWithWool.Clear();
        skinListClotheWithWool.Clear();
        skinListHatWithoutWool.Clear();
        skinListClotheWithoutWool.Clear();

        for (int i = 0; i < withWoolSkins.GetChild(0).childCount; i++)
            skinListHatWithWool.Add(withWoolSkins.GetChild(0).GetChild(i).GetComponent<SkinUnit>());

        for (int i = 0; i < withWoolSkins.GetChild(1).childCount; i++)
            skinListClotheWithWool.Add(withWoolSkins.GetChild(1).GetChild(i).GetComponent<SkinUnit>());

        for (int i = 0; i < withoutWoolSkins.GetChild(0).childCount; i++)
            skinListHatWithoutWool.Add(withoutWoolSkins.GetChild(0).GetChild(i).GetComponent<SkinUnit>());

        for (int i = 0; i < withoutWoolSkins.GetChild(1).childCount; i++)
            skinListClotheWithoutWool.Add(withoutWoolSkins.GetChild(1).GetChild(i).GetComponent<SkinUnit>());

        UpdateAll();
    }

    private void Update()
    {
        withWoolSkins.gameObject.SetActive(parentSheep.hasLaine);
        withoutWoolSkins.gameObject.SetActive(!parentSheep.hasLaine);
    }

    public void UpdateAll()
    {
        UpdateSkinListHat(parentSheep.GetCurrentSkinHat());
        UpdateSkinListClothe(parentSheep.GetCurrentSkinClothe());
    }

    public void UpdateSkinListHat(int currentSkinHat)
    {
        var list = GetCurrentSkinHatList();

        foreach (var s in list)
            s.gameObject.SetActive(s.id == currentSkinHat);
    }

    public void UpdateSkinListClothe(int currentSkinClothe)
    {
        var list = GetCurrentSkinClotheList();

        foreach (var s in list)
            s.gameObject.SetActive(s.id == currentSkinClothe);
    }

    private List<SkinUnit> GetCurrentSkinHatList()
    {
        return parentSheep.hasLaine ? skinListHatWithWool : skinListHatWithoutWool;
    }

    private List<SkinUnit> GetCurrentSkinClotheList()
    {
        return parentSheep.hasLaine ? skinListClotheWithWool : skinListClotheWithoutWool;
    }
}