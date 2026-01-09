using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinListManager : MonoBehaviour
{
    private List<SkinUnit> skinListHatWithWool = new();
    private List<SkinUnit> skinListClotheWithWool = new();
    private List<SkinUnit> skinListHatWithoutWool = new();
    private List<SkinUnit> skinListClotheWithoutWool = new();

    [SerializeField]private Sheep parentSheep;

    [SerializeField] private Transform withWoolSkins;
    [SerializeField] private Transform withoutWoolSkins;
    
    public void Initialize()
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
        UpdateSkinListHat(parentSheep.currentSkinHat);
        UpdateSkinListClothe(parentSheep.currentSkinClothe);
    }

    public void CutLaine()
    {
        parentSheep.hasLaine = false;
        UpdateAll();
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

    // Combo checking
    public bool HasCombo(int hatId, int clotheId)
    {
        var hatCombo = GetCombo(hatId, skinListHatWithoutWool);
        var clotheCombo = GetCombo(clotheId, skinListClotheWithoutWool);

        if (hatCombo == SkinCombo.Default || clotheCombo == SkinCombo.Default)
            return false;

        return hatCombo == clotheCombo;
    }

    public SkinCombo GetCombo(int id, List<SkinUnit> list)
    {
        foreach (SkinUnit s in list)
            if (s.id == id)
                return s.combo;

        return SkinCombo.Default;
    }
    
    public NatureType GetNatureFromCombo(int hatId)
    {
        SkinCombo combo = GetCombo(hatId, skinListHatWithoutWool);

        return combo switch
        {
            SkinCombo.Sherif   => NatureType.Solitaire,
            SkinCombo.Leader   => NatureType.Dominant,
            SkinCombo.Chill    => NatureType.Standard,
            SkinCombo.Artiste  => NatureType.Solitaire,
            SkinCombo.Intelo   => NatureType.Peureux,
            _                  => NatureType.Standard
        };
    }
}
