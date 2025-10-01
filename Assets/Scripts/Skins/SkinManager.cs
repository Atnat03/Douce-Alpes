using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private SkinScriptable skinData;

    private List<SkinSkelete> skins = new List<SkinSkelete>();

    [Header("UI")]
    [SerializeField] private Image[] visibleImage; 

    [SerializeField] private int indexCurrentSkin = 0; 
    [SerializeField] private RectTransform swipeZone;

    void Start()
    {
        skins.AddRange(skinData.skins);

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
        SwipeLeft();
    }

    public void DoubleLeftArrow()
    {
        SwipeLeft();
        SwipeLeft();
    }

    public void RightArrow()
    {
        SwipeRight();
    }
    
    public void DoubleRightArrow()
    {
        SwipeRight();
        SwipeRight();
    }

    public void SwipeRight()
    {
        indexCurrentSkin--;
        if (indexCurrentSkin < 0)
            indexCurrentSkin = skins.Count - 1;

        UpdateUI();
    }

    public void SwipeLeft()
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

    public bool IsInsideSwipeArea(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(swipeZone, screenPosition);
    }
}
