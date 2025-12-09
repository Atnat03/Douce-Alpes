using System.Collections.Generic;
using UnityEngine;
using System;

public class SkinAgency : MonoBehaviour
{
    public static SkinAgency instance;

    [SerializeField] private SkinScriptable hatSkinData;
    [SerializeField] private SkinScriptable clotheSkinData;

    public Dictionary<int, int> dicoHatSkinStack = new Dictionary<int, int>();
    public Dictionary<int, int> dicoClotheSkinStack = new Dictionary<int, int>();

    public Dictionary<int, int> hatSkinEquippedOnSheep = new Dictionary<int, int>();
    public Dictionary<int, int> clotheSkinEquippedOnSheep = new Dictionary<int, int>();
    
    public event Action OnStacksChanged;

    private void Awake()
    {
        instance = this;

        foreach (var s in hatSkinData.skins)
            dicoHatSkinStack[s.id] = 0;

        foreach (var s in clotheSkinData.skins)
            dicoClotheSkinStack[s.id] = 0;
    }

    [ContextMenu("AddSkin")]
    public void AddSkin()
    {
        AddHatSkinInstance(1);
        AddHatSkinInstance(0);
        AddHatSkinInstance(1);
        AddHatSkinInstance(0);
    }

    public void AddHatSkinInstance(int id) => dicoHatSkinStack[id]++;
    public void AddClotheSkinInstance(int id) => dicoClotheSkinStack[id]++;

    public bool CanEquipHat(int skinId) => dicoHatSkinStack.ContainsKey(skinId) && dicoHatSkinStack[skinId] > 0;
    public bool CanEquipClothe(int skinId) => dicoClotheSkinStack.ContainsKey(skinId) && dicoClotheSkinStack[skinId] > 0;

    // ⚡ Équipe le skin sur le mouton et réserve le stock
    public void EquipHat(int sheepId, int skinId) 
    {
        Debug.Log("Hat");
        if (!CanEquipHat(skinId)) return;
        if (hatSkinEquippedOnSheep.TryGetValue(sheepId, out int prev)) 
            dicoHatSkinStack[prev]++;  // Libère l'ancien
        hatSkinEquippedOnSheep[sheepId] = skinId;
        dicoHatSkinStack[skinId]--;  // Réserve le nouveau
        OnStacksChanged?.Invoke();  // Ajout
    }

    public void UnequipHat(int sheepId) 
    {
        if (hatSkinEquippedOnSheep.TryGetValue(sheepId, out int skinId)) 
        {
            dicoHatSkinStack[skinId]++;
            hatSkinEquippedOnSheep.Remove(sheepId);
            OnStacksChanged?.Invoke();  // Ajout
        }
    }

    public void EquipClothe(int sheepId, int skinId) 
    {
        if (!CanEquipClothe(skinId)) return;
        if (clotheSkinEquippedOnSheep.TryGetValue(sheepId, out int prev)) 
            dicoClotheSkinStack[prev]++;
        clotheSkinEquippedOnSheep[sheepId] = skinId;
        dicoClotheSkinStack[skinId]--;
        OnStacksChanged?.Invoke();  // Ajout
    }

    public void UnequipClothe(int sheepId) 
    {
        if (clotheSkinEquippedOnSheep.TryGetValue(sheepId, out int skinId)) 
        {
            dicoClotheSkinStack[skinId]++;
            clotheSkinEquippedOnSheep.Remove(sheepId);
            OnStacksChanged?.Invoke();  // Ajout
        }
    }
}
