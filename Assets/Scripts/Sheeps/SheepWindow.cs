using System;
using UnityEngine;
using UnityEngine.UI;

public class SheepWindow : MonoBehaviour
{
    public static SheepWindow instance;
    
    [SerializeField] SkinManager skinManager;
    
    [Header("UI")] 
    [SerializeField] private InputField nameText;
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
        Debug.Log($"[SheepWindow] SetNewCurrentSkinHat appelée pour sheep {sheepId}, ID {id}");  // Log A : Arrivée ?

        if (SkinAgency.instance == null) 
        {
            Debug.LogError("[SheepWindow] SkinAgency null !");
            return;
        }

        SkinAgency.instance.EquipHat(sheepId, id);  // Ça appellera les logs de SkinAgency si tu les as ajoutés
        Debug.Log("[SheepWindow] EquipHat appelée");  // Log B : Après Equip

        Sheep sheep = GameManager.instance.GetSheep(sheepId);
        if (sheep == null) 
        {
            Debug.LogError($"[SheepWindow] Sheep {sheepId} null !");
            return;
        }
        sheep.SetCurrentSkinHat(id);
        Debug.Log("[SheepWindow] Sheep updated, fin");  // Log C : Fin
    }

    public void SetNewCurrentSkinClothe(int id) 
    {
        Debug.Log($"[SheepWindow] SetNewCurrentSkinClothe appelée pour sheep {sheepId}, ID {id}");
        if (SkinAgency.instance == null) 
        {
            Debug.LogError("[SheepWindow] SkinAgency null !");
            return;
        }
        SkinAgency.instance.EquipClothe(sheepId, id);
        Debug.Log("[SheepWindow] EquipClothe appelée");
        Sheep sheep = GameManager.instance.GetSheep(sheepId);
        if (sheep == null) 
        {
            Debug.LogError($"[SheepWindow] Sheep {sheepId} null !");
            return;
        }
        sheep.SetCurrentSkinClothe(id);
        Debug.Log("[SheepWindow] Sheep updated, fin");
    }
    
    public InputField GetInputField(){return nameText;}

    public int GetCurrentSheepID()
    {
        return sheepId;
    }

    public void Initialize(string name, int currentSkinHat, int currentSkinClothe, int sheepId) 
    {
        isOpen = true;
        nameText.text = name;
        this.currentSkinHat = currentSkinHat;
        this.currentSkinClothe = currentSkinClothe;
        this.sheepId = sheepId;

        // Met à jour les stacks dans SkinAgency
        SkinAgency.instance.InitializeSheepSkin(sheepId, currentSkinHat, currentSkinClothe);

        // Centre les scrolls sur les skins actuels
        if (hatSkinSelector != null)
        {
            hatSkinSelector.SetStartingPanelToCurrent();
            hatSkinSelector.SelectPanelVisual(currentSkinHat); // <-- nouveau
            hatSkinSelector.UpdateStackDisplays();             // met à jour stacks et grisage
        }

        if (clotheSkinSelector != null)
        {
            clotheSkinSelector.SetStartingPanelToCurrent();
            clotheSkinSelector.SelectPanelVisual(currentSkinClothe); // <-- nouveau
            clotheSkinSelector.UpdateStackDisplays();
        }
    }


    public void ResetValue()
    {
        isOpen = false;
        currentSkinHat = -1;
        currentSkinClothe = -1;
        sheepId = -1;
    }
}
