using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private SkinScriptable skinData;

    private List<SkinSkelete> skins = new List<SkinSkelete>();

    [Header("UI")]
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;
    [SerializeField] private Image[] visibleImage; 

    [SerializeField] private int indexCurrentSkin = 0; 

    void Start()
    {
        skins.AddRange(skinData.skins);

        leftArrow.onClick.AddListener(LeftArrow);
        rightArrow.onClick.AddListener(RightArrow);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (skins.Count == 0 || visibleImage.Length == 0)
            return;

        for (int i = 0; i < visibleImage.Length; i++)
        {
            int offset = i - (visibleImage.Length / 2);
            int skinIndex = (indexCurrentSkin + offset + skins.Count) % skins.Count;

            visibleImage[i].sprite = skins[skinIndex].logo;
        }
        
        SheepWindow.instance.SetNewCurrentSkin();
    }
    
    public void SetCurrentSkin(int skinId)
    {
        int index = skins.FindIndex(s => s.id == skinId);
        if (index != -1)
        {
            indexCurrentSkin = index;
            UpdateUI();
        }
    }

    public void LeftArrow()
    {
        indexCurrentSkin--;
        if (indexCurrentSkin < 0)
            indexCurrentSkin = skins.Count - 1;

        UpdateUI();
    }

    public void RightArrow()
    {
        indexCurrentSkin++;
        if (indexCurrentSkin >= skins.Count)
            indexCurrentSkin = 0;

        UpdateUI();
    }

    public int GetCurrentSkinID()
    {
        return skins[indexCurrentSkin].id;
    }

    public SkinSkelete GetCurrentSkin()
    {
        return skins[indexCurrentSkin];
    }
}