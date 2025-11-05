using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public enum TypeAmelioration
{
    Tonte, Sortie, Rentree, Nettoyage, Abreuvoir, Overflow
}

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [HideInInspector] public List<SheepData> sheepDestroyData = new ();
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;
    
    [SerializeField] 
    
    [Header("Saving data")]
    public double timeWhenLoad;
    public double timeSinceLastLoad;
    
    public List<int> unlockedSkinIDs = new ();
    public event Action OnSkinsUpdated;

    public AmeliorationValueSO[] soUpgradeList;
    
    public Dictionary<TypeAmelioration, (AmeliorationValueSO, int)> dicoAmélioration = new();
    
    private void Awake()
    {
        instance = this;

        Saving.instance.savingEvent += SaveMyData;
        Saving.instance.loadingEvent += LoadMyData;
    }

    private void Start()
    {
        dicoAmélioration = new()
        {
            { TypeAmelioration.Tonte, (soUpgradeList[0], 0)},
            { TypeAmelioration.Sortie, (soUpgradeList[1], 0)},
            { TypeAmelioration.Rentree, (soUpgradeList[2], 0)},
            { TypeAmelioration.Nettoyage, (soUpgradeList[3], 0)},
            { TypeAmelioration.Abreuvoir, (soUpgradeList[4], 0)},
            //{ TypeAmelioration.Overflow, (soUpgradeList[5], 0)},
        };
    }

    private void SaveMyData()
    {
        Saving.instance.curTime = DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    private void LoadMyData(PlayerData data)
    {
        if (data == null) return;

        timeWhenLoad = data.lastTime;

        double now = DateTimeOffset.Now.ToUnixTimeSeconds();
        timeSinceLastLoad = now - timeWhenLoad;
    }

    private void Update()
    {
        isSheepInside = sheepDestroyData.Count == nbSheep;
    }
    
    #region SKIN
    
    public bool HasSkin(int id)
    {
        return unlockedSkinIDs.Contains(id);
    }

    [ContextMenu("UnlockSkin")]
    public void UnlockSkin(int id)
    {
        if (!unlockedSkinIDs.Contains(id))
        {
            unlockedSkinIDs.Add(id);
            OnSkinsUpdated?.Invoke();
        }
    }

    public void LockSkin(int id)
    {
        if (unlockedSkinIDs.Remove(id))
        {
            OnSkinsUpdated?.Invoke();
        }
    }
    
    #endregion

    #region AMELIORATION

    public void AddLevelUpgrade(TypeAmelioration type)
    {
        if (!MiniGameParent.CheckIfCanUpgrade(type))
        {
            Debug.LogError("Cant upgrade, not enought sheep");
            return;
        }
        var oldTuple = dicoAmélioration[type];
        var newTuple = (oldTuple.Item1, oldTuple.Item2 + 1);
        dicoAmélioration[type] = newTuple;
        
        Debug.Log($"{dicoAmélioration[type].Item1} + {dicoAmélioration[type].Item2}");
    }

    public int GetLevelUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type].Item1.levelsValue[dicoAmélioration[type].Item2].value;
    }

    public (AmeliorationValueSO, int) GetSOUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type];
    }

    #endregion
}
