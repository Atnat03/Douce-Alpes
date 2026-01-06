using System.Collections.Generic;
using UnityEngine;
using System;

public class SkinAgency : MonoBehaviour
{
    public static SkinAgency instance;

    [SerializeField] private SkinScriptable hatSkinData;
    [SerializeField] private SkinScriptable clotheSkinData;
    [SerializeField] private ArticleScriptable interiorSkinData;

    public Dictionary<int, int> dicoHatSkinStack = new Dictionary<int, int>();
    public Dictionary<int, int> dicoClotheSkinStack = new Dictionary<int, int>();

    public Dictionary<int, int> hatSkinEquippedOnSheep = new Dictionary<int, int>();
    public Dictionary<int, int> clotheSkinEquippedOnSheep = new Dictionary<int, int>();
    
    public event Action OnStacksChanged;

    public int skinGrangeId = 0;
    public int skinBarriereId = 0;

    public int skinShopId = 0;
    public int skinNicheId = 0;

    public int skinTricotId = 0;

    [SerializeField] private SkinBuildManager barrierSkinManager;
    [SerializeField] private SkinBuildManagerGrange grangeSkinManager;
    [SerializeField] private SkinBuildManager nicheSkinManager;
    [SerializeField] private SkinBuildManager shopSkinManager;
    [SerializeField] private SkinBuildManager tricotSkinManager;

    public Dictionary<int, bool> dicoInteriorSkin = new Dictionary<int, bool>();
    
    private void Awake()
    {
        instance = this;

        foreach (var s in hatSkinData.skins)
            dicoHatSkinStack[s.id] = 0;

        foreach (var s in clotheSkinData.skins)
            dicoClotheSkinStack[s.id] = 0;
        
        foreach (var s in interiorSkinData.articles)
            dicoInteriorSkin[s.id] = false;
    }

    [ContextMenu("Swap Skin")]
    public void ChangeSkin()
    {
        SetSkinBarriere(1);
        SetSkinGrange(2);
    }

    public void SetSkinGrange(int id)
    {
        skinGrangeId = id;
        grangeSkinManager.SwapSkin(id);
    }
    
    public void SetSkinBarriere(int id)
    {
        skinBarriereId = id;
        barrierSkinManager.SwapSkin(id);
    }
    public void SetSkinNiche(int id)
    {
        skinNicheId = id;
        nicheSkinManager.SwapSkin(id);
    }
    public void SetSkinShop(int id)
    {
        skinShopId = id;
        shopSkinManager.SwapSkin(id);
    }

    public void SetSkinTricot(int id)
    {
        skinTricotId = id;
        tricotSkinManager.SwapSkin(id);
    }

    public void SetSkinInterior(int id)
    {
        dicoInteriorSkin[id] = !dicoInteriorSkin[id];
    }

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            AddHatSkinInstance(13);
            AddClotheSkinInstance(10);
        }
        
        SetSkinGrange(skinGrangeId);
        SetSkinBarriere(skinGrangeId);
        SetSkinShop(skinShopId);
        SetSkinNiche(skinNicheId);
    }
    
    public void InitializeSheepSkin(int sheepId, int hatId, int clotheId)
    {
        if (hatId >= 0)
        {
            dicoHatSkinStack[hatId] = Mathf.Max(dicoHatSkinStack[hatId] - 1, 0);
            hatSkinEquippedOnSheep[sheepId] = hatId;
        }

        if (clotheId >= 0)
        {
            dicoClotheSkinStack[clotheId] = Mathf.Max(dicoClotheSkinStack[clotheId] - 1, 0);
            clotheSkinEquippedOnSheep[sheepId] = clotheId;
        }

        OnStacksChanged?.Invoke();
    }


    [ContextMenu("Add All skins")]
    public void AddSkinInstance()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 13; j++)
                AddHatSkinInstance(j);
            
            for (int j = 0; j < 10; j++)
                AddClotheSkinInstance(j);
        }
    }

    public void AddHatSkinInstance(int id)
    {
        dicoHatSkinStack[id]++;
    }

    public void AddClotheSkinInstance(int id)
    {
        dicoClotheSkinStack[id]++;
    }

    public bool CanEquipHat(int skinId) => dicoHatSkinStack.ContainsKey(skinId) && dicoHatSkinStack[skinId] > 0;
    public bool CanEquipClothe(int skinId) => dicoClotheSkinStack.ContainsKey(skinId) && dicoClotheSkinStack[skinId] > 0;

    public void EquipHat(int sheepId, int skinId) 
    {
        if (!CanEquipHat(skinId)) return;
        if (hatSkinEquippedOnSheep.TryGetValue(sheepId, out int prev)) 
            dicoHatSkinStack[prev]++;  
        hatSkinEquippedOnSheep[sheepId] = skinId;
        dicoHatSkinStack[skinId]--;  
        OnStacksChanged?.Invoke();  
    }

    public void UnequipHat(int sheepId) 
    {
        if (hatSkinEquippedOnSheep.TryGetValue(sheepId, out int skinId)) 
        {
            dicoHatSkinStack[skinId]++;
            hatSkinEquippedOnSheep.Remove(sheepId);
            OnStacksChanged?.Invoke();  
        }
    }

    public void EquipClothe(int sheepId, int skinId) 
    {
        if (!CanEquipClothe(skinId)) return;
        if (clotheSkinEquippedOnSheep.TryGetValue(sheepId, out int prev)) 
            dicoClotheSkinStack[prev]++;
        clotheSkinEquippedOnSheep[sheepId] = skinId;
        dicoClotheSkinStack[skinId]--;
        OnStacksChanged?.Invoke(); 
    }

    public void UnequipClothe(int sheepId) 
    {
        if (clotheSkinEquippedOnSheep.TryGetValue(sheepId, out int skinId)) 
        {
            dicoClotheSkinStack[skinId]++;
            clotheSkinEquippedOnSheep.Remove(sheepId);
            OnStacksChanged?.Invoke();  
        }
    }
    
    private List<IntIntPair> ToIntIntList(Dictionary<int,int> dico)
    {
        var list = new List<IntIntPair>();
        foreach (var kvp in dico)
            list.Add(new IntIntPair { key = kvp.Key, value = kvp.Value });
        return list;
    }
    private Dictionary<int,int> ToIntIntDico(List<IntIntPair> list)
    {
        var dico = new Dictionary<int,int>();
        foreach (var p in list)
            dico[p.key] = p.value;
        return dico;
    }

    private Dictionary<int,bool> ToIntBoolDico(List<IntBoolPair> list)
    {
        var dico = new Dictionary<int,bool>();
        foreach (var p in list)
            dico[p.key] = p.value;
        return dico;
    }

    
    private List<IntBoolPair> ToIntBoolList(Dictionary<int,bool> dico)
    {
        var list = new List<IntBoolPair>();
        foreach (var kvp in dico)
            list.Add(new IntBoolPair { key = kvp.Key, value = kvp.Value });
        return list;
    }

    public void ApplySaveData(SkinAgencySaveData data)
    {
        dicoClotheSkinStack.Clear();
        dicoHatSkinStack.Clear();
        hatSkinEquippedOnSheep.Clear();
        clotheSkinEquippedOnSheep.Clear();
        dicoInteriorSkin.Clear();
        
        dicoHatSkinStack = ToIntIntDico(data.hatStacks);
        dicoClotheSkinStack = ToIntIntDico(data.clotheStacks);

        hatSkinEquippedOnSheep = ToIntIntDico(data.hatEquipped);
        clotheSkinEquippedOnSheep = ToIntIntDico(data.clotheEquipped);

        dicoInteriorSkin = ToIntBoolDico(data.interiorSkins);

        SetSkinGrange(data.skinGrangeId);
        SetSkinBarriere(data.skinBarriereId);
        SetSkinShop(data.skinShopId);
        SetSkinNiche(data.skinNicheId);
        SetSkinTricot(data.skinTricotId);

        OnStacksChanged?.Invoke();
    }

    
    public SkinAgencySaveData BuildSaveData()
    {
        return new SkinAgencySaveData
        {
            hatStacks = ToIntIntList(dicoHatSkinStack),
            clotheStacks = ToIntIntList(dicoClotheSkinStack),

            hatEquipped = ToIntIntList(hatSkinEquippedOnSheep),
            clotheEquipped = ToIntIntList(clotheSkinEquippedOnSheep),

            interiorSkins = ToIntBoolList(dicoInteriorSkin),

            skinGrangeId = skinGrangeId,
            skinBarriereId = skinBarriereId,
            skinShopId = skinShopId,
            skinNicheId = skinNicheId,
            skinTricotId = skinTricotId
        };
    }
}

[Serializable]
public class IntIntPair
{
    public int key;
    public int value;
}

[Serializable]
public class IntBoolPair
{
    public int key;
    public bool value;
}

[Serializable]
public class SkinAgencySaveData
{
    public List<IntIntPair> hatStacks;
    public List<IntIntPair> clotheStacks;

    public List<IntIntPair> hatEquipped;
    public List<IntIntPair> clotheEquipped;

    public List<IntBoolPair> interiorSkins;

    public int skinGrangeId;
    public int skinBarriereId;
    public int skinShopId;
    public int skinNicheId;
    public int skinTricotId;
}

