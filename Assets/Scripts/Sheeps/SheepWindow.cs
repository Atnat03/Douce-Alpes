using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SheepWindow : MonoBehaviour
{
    public static SheepWindow instance;
    
    [SerializeField] SkinManager skinManager;
    
    [Header("UI")] 
    [SerializeField] private InputField nameText;
    [SerializeField] private TextMeshProUGUI birthDateText;
    public int currentSkinHat;
    public int currentSkinClothe;
    private int sheepId;

    public bool isOpen = false;
    
    [Header("Skin Selectors")]
    [SerializeField] private AddSkins hatSkinSelector;
    [SerializeField] private AddSkins clotheSkinSelector;

    private void Awake()
    {
        instance = this;
    }

    public void SetName()
    {
        foreach (Sheep s in GameManager.instance.sheepList)
        {
            if (s.sheepId == sheepId)
            {
                s.sheepName = nameText.text;
            }
        }
    }

    public void SetColor(int idColor)
    {
        foreach (Sheep s in GameManager.instance.sheepList)
        {
            if (s.sheepId == sheepId)
            {
                s.SetNewWoolColor(idColor);
            }
        }
    }

    public void SetNewCurrentSkinHat(int id)
    {
        SkinAgency.instance.EquipHat(sheepId, id);
        Sheep sheep = GameManager.instance.GetSheep(sheepId);
        sheep.SetCurrentSkinHat(id);
    }

    public void SetNewCurrentSkinClothe(int id) 
    {
        SkinAgency.instance.EquipClothe(sheepId, id);
        Sheep sheep = GameManager.instance.GetSheep(sheepId);
        sheep.SetCurrentSkinClothe(id);
    }
    
    public InputField GetInputField(){return nameText;}

    public int GetCurrentSheepID()
    {
        return sheepId;
    }

    public void Initialize(string name, int currentSkinHat, int currentSkinClothe, int sheepId, string sheepBirthDate) 
    {
        isOpen = true;
        nameText.text = name;
        this.currentSkinHat = currentSkinHat;
        this.currentSkinClothe = currentSkinClothe;
        this.sheepId = sheepId;

        birthDateText.text = sheepBirthDate;

        SkinAgency.instance.InitializeSheepSkin(sheepId, currentSkinHat, currentSkinClothe);

        if (hatSkinSelector != null)
        {
            hatSkinSelector.SetStartingPanelToCurrent();
            hatSkinSelector.SelectPanelVisual(currentSkinHat); 
            hatSkinSelector.UpdateStackDisplays();             
        }

        if (clotheSkinSelector != null)
        {
            clotheSkinSelector.SetStartingPanelToCurrent();
            clotheSkinSelector.SelectPanelVisual(currentSkinClothe);
            clotheSkinSelector.UpdateStackDisplays();
        }
    }
    
    public void CloseSheepAndOpenShop()
    {
        GameManager.instance.CloseWindowShopAndGoToShop();
    }

    public void PlayBheee()
    {
        Sheep sheep = GameManager.instance.GetSheep(sheepId);
        sheep.PlaySoundBehhh(); 
    }
    
    public void ResetValue()
    {
        isOpen = false;
        currentSkinHat = -1;
        currentSkinClothe = -1;
        sheepId = -1;
    }
}
