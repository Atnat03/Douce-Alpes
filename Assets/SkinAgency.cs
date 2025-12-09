using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinAgency : MonoBehaviour
{
    public static SkinAgency instance;
    
    [SerializeField] private int[] unlockSkinList;
    
    public Dictionary<int, int> dicoHatSkinStack = new Dictionary<int, int>();
    public Dictionary<int, int> dicoClotheSkinStack = new Dictionary<int, int>();

    [SerializeField] private SkinScriptable hatSkinData;
    [SerializeField] private SkinScriptable clotheSkinData;

    private void Awake()
    {
        instance = this;

        foreach (SkinSkelete s in hatSkinData.skins)
        {
            dicoHatSkinStack.Add(s.id, 0);
        }
        
        foreach (SkinSkelete s in clotheSkinData.skins)
        {
            dicoClotheSkinStack.Add(s.id, 0);
        }
    }

    [ContextMenu("AddItemHat")]
    public void AddItem()
    {
        AddHatSkinInstance(0);
    }
    
    public void AddHatSkinInstance(int id)
    {
        if (dicoHatSkinStack.TryGetValue(id, out int currentCount))
        {
            dicoHatSkinStack[id] = currentCount + 1;
        }
        else
        {
            Debug.LogWarning($"Skin ID {id} n'existe pas dans dicoHatSkinStack.");
        }
    }

    public void AddClotheSkinInstance(int id)
    {
        if (dicoClotheSkinStack.TryGetValue(id, out int currentCount))
        {
            dicoClotheSkinStack[id] = currentCount + 1;
        }
        else
        {
            Debug.LogWarning($"Skin ID {id} n'existe pas dans dicoHatSkinStack.");
        }
    }
    
    public void RemoveHatSkinInstance(int id)
    {
        if (dicoHatSkinStack.TryGetValue(id, out int currentCount))
        {
            if(currentCount > 0)
                dicoHatSkinStack[id] = currentCount - 1;
        }
        else
        {
            Debug.LogWarning($"Skin ID {id} n'existe pas dans dicoHatSkinStack.");
        }
    }

    public void RemoveClotheSkinInstance(int id)
    {
        
        if (dicoClotheSkinStack.TryGetValue(id, out int currentCount))
        {
            if(currentCount > 0)
                dicoClotheSkinStack[id] = currentCount - 1;
        }
        else
        {
            Debug.LogWarning($"Skin ID {id} n'existe pas dans dicoHatSkinStack.");
        }
    }

    public bool CheckIfEnoughtInstanceHat(int id)
    {
        if (dicoHatSkinStack.TryGetValue(id, out int currentCount))
        {
            if (currentCount >= 1)
                return true;
        }

        return false;
    }
    
    public bool CheckIfEnoughtInstanceClothe(int id)
    {
        if (dicoClotheSkinStack.TryGetValue(id, out int currentCount))
        {
            if (currentCount >= 1)
                return true;
        }

        return false;
    }
}
