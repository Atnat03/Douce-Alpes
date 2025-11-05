using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public enum TypeAmelioration
{
    Tonte, Sortie, Rentree, Nettoyage, Abreuvoir, Overflow
}

[DefaultExecutionOrder(-1)]
public class GameData : MonoBehaviour
{
    public static GameData instance;

    public Action<TypeAmelioration, float, bool> OnCooldownUpdated;
    public Action<TypeAmelioration> OnCooldownFinish;

    [HideInInspector] public List<SheepData> sheepDestroyData = new ();
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;

    public bool hasTonte = false;
    public bool hasClean = false;

    [SerializeField] public int[] coolDownTimers;

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
        
        UpdateAllCooldownTimers();
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

    public bool HasSkin(int id) => unlockedSkinIDs.Contains(id);

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
            Debug.LogError("Can't upgrade, not enough sheep");
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

    public int GetCooldownUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type].Item1.levelsValue[dicoAmélioration[type].Item2].cooldown;
    }

    public (AmeliorationValueSO, int) GetSOUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type];
    }

    public int GetCurrentTimer(TypeAmelioration type)
    {
        switch (type)
        {
            case TypeAmelioration.Tonte : return coolDownTimers[0];
            case TypeAmelioration.Sortie : return coolDownTimers[1];
            case TypeAmelioration.Rentree : return coolDownTimers[2];
            case TypeAmelioration.Nettoyage : return coolDownTimers[3];
            case TypeAmelioration.Abreuvoir : return coolDownTimers[4];
            default: return 0;
        }
    }

    public void SetCooldownTimer(TypeAmelioration type, int value)
    {
        switch (type)
        {
            case TypeAmelioration.Tonte: coolDownTimers[0] = value; break;
            case TypeAmelioration.Sortie: coolDownTimers[1] = value; break;
            case TypeAmelioration.Rentree: coolDownTimers[2] = value; break;
            case TypeAmelioration.Nettoyage: coolDownTimers[3] = value; break;
            case TypeAmelioration.Abreuvoir: coolDownTimers[4] = value; break;
        }
    }

    public void UpdateAllCooldownTimers()
    {
        foreach (var type in dicoAmélioration.Keys)
        {
            SetCooldownTimer(type, GetCooldownUpgrade(type));
        }
    }

    #endregion

    #region Timer / Mini-jeux

    public void StartMiniGameCooldown(TypeAmelioration type)
    {
        float time = GetCooldownUpgrade(type);
        Debug.Log("Start Timer for " + type);

        bool state = GetCurrentStateToNextMiniGame(type);
        
        StartCoroutine(CooldownCoroutine(type, time, state));
    }

    private IEnumerator CooldownCoroutine(TypeAmelioration type, float remainingTime, bool state = true)
    {
        SetCooldownTimer(type, (int)remainingTime);

        while (remainingTime > 0)
        {
            OnCooldownUpdated?.Invoke(type, remainingTime, state);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;

            SetCooldownTimer(type, (int)remainingTime);
        }

        SetCooldownTimer(type, 0);
        OnCooldownFinish?.Invoke(type);
        Debug.Log($"{type} cooldown finished");
    }

    bool GetCurrentStateToNextMiniGame(TypeAmelioration type)
    {
        switch (type)
        {
            case TypeAmelioration.Tonte:
                return isSheepInside;
            case TypeAmelioration.Nettoyage:
                return hasTonte;
            case TypeAmelioration.Sortie:
                return hasClean;
        }

        return true;
    }

    #endregion
}
