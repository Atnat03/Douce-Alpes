using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

public enum TypeAmelioration
{
    Rentree, Tonte, Nettoyage, Sortie, Abreuvoir, Overflow
}

[Serializable]
public class SheepDataList
{
    public List<SheepData> sheepList;
}

[DefaultExecutionOrder(-1)]
public class GameData : MonoBehaviour
{
    public static GameData instance;

    public bool IsStatGame = false;

    public Action<TypeAmelioration, float, bool> OnCooldownUpdated;
    public Action<TypeAmelioration> OnCooldownFinish;

    public List<SheepData> sheepDestroyData = new ();
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;
    public int numberDay = 0;

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
    
    [HideInInspector] public TimerManager timer;
    
    public TricotManager tricotManager;

    public bool isTuto = true;

    public WeatherManager dayMoment;
    public DayRecapManager dayRecap;
    
    public int currentMoneyDay = 0;
    public int currentWoolDay = 0;

    public SheepBoidManager BoidManager;

    public PanneauShop PanneauShop;
    public Build bergerie;
    public InteriorSceneManager interiorSceneManager;

    [Header("2e Tuto")] 
    public bool is2eTuto = false;
    public string[] messages;
    public GameObject papy;
    public TextMeshProUGUI message;
    private int idMessage = -1;
    public GameObject nextMessage;
    private bool isWritting = false;
    
    private void Awake()
    {
        instance = this;

        Saving.instance.savingEvent += SaveMyData;
        Saving.instance.loadingEvent += LoadMyData;
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        papy.SetActive(false);
        
        dicoAmélioration = new() {
            { TypeAmelioration.Tonte, (soUpgradeList[0], 0)},
            { TypeAmelioration.Sortie, (soUpgradeList[1], 0)},
            { TypeAmelioration.Rentree, (soUpgradeList[2], 0)},
            { TypeAmelioration.Nettoyage, (soUpgradeList[3], 0)},
            { TypeAmelioration.Abreuvoir, (soUpgradeList[4], 0)},
            //{ TypeAmelioration.Overflow, (soUpgradeList[5], 0)},
        };

        Array.Fill(coolDownTimers, 0);

        timer = GetComponent<TimerManager>();
    }

    private void SaveMyData()
    {
        Saving.instance.curTime = DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    private void LoadMyData(PlayerData data)
    {
        if (data == null) return;

        timeWhenLoad = data.lastTime;
        timeSinceLastLoad =
            DateTimeOffset.Now.ToUnixTimeSeconds() - timeWhenLoad;

        sheepDestroyData.Clear();
        GameManager.instance.sheepList.Clear();
        BoidManager.nbInstantSheep = 0;
        isSheepInside = data.isInGrange;
        nbSheep = data.nbSheep;

        if (isSheepInside)
        {
            sheepDestroyData.AddRange(data.sheepList);
        }
        else
        {
            foreach (SheepData sheep in data.sheepList)
            {
                BoidManager.SheepGetOffAndRecreate(sheep, sheep.position);
            }
        }
        
        SkinAgency.instance.ApplySaveData(data.skinAgency);
        
        PlayerMoney.instance.LoadStats(data.gold, data.wool);
        BonheurCalculator.instance.currentBonheur = data.happiness;
        
        timer.currentMiniJeuToDo = data.currentMiniJeuToDo;
        timer.canButtonG = data.canButtonG;
        timer.canButtonT = data.canButtonT;
        timer.canButtonC = data.canButtonC;
        timer.UpdateAllButton();
        int nextIndex = Array.IndexOf(Enum.GetValues(typeof(MiniGames)), timer.currentMiniJeuToDo);
        timer.StartCoroutine(timer.UpdateHorloge(nextIndex));

        numberDay = data.nbDay;
        
        LoadUpgrades(data.upgrades);
        
        tricotManager.LoadTricotState(data.tricotData);
        
        isTuto = data.isTuto;
        interiorSceneManager.isTutoInterior = data.isTutoInterior;
        
        Array.Fill(coolDownTimers, 0);
    }

    public UpgradesSaveData GetSaveData()
    {
        var save = new UpgradesSaveData();
    
        foreach (var kvp in dicoAmélioration)
        {
            save.niveaux[kvp.Key] = kvp.Value.Item2;
        }
    
        return save;
    }

    public void LoadUpgrades(UpgradesSaveData saveData)
    {
        if (saveData == null || saveData.niveaux == null)
            return;

        foreach (var kvp in saveData.niveaux)
        {
            if (dicoAmélioration.ContainsKey(kvp.Key))
            {
                var tuple = dicoAmélioration[kvp.Key];
                dicoAmélioration[kvp.Key] = (tuple.Item1, kvp.Value);
            }
        }
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

    public void AddLevelTonte() => AddLevelUpgrade(TypeAmelioration.Tonte);
    public void AddLevelClean() => AddLevelUpgrade(TypeAmelioration.Nettoyage);
    public void AddLevelSortie() => AddLevelUpgrade(TypeAmelioration.Sortie);
    public void AddLevelAbreuvoir() => AddLevelUpgrade(TypeAmelioration.Abreuvoir);

    public void AddLevelUpgrade(TypeAmelioration type)
    {
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

    public int GetLevel(TypeAmelioration type)
    {
        return dicoAmélioration[type].Item2;
    }

    public (AmeliorationValueSO, int) GetSOUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type];
    }
    
    public bool HasUpgrade(TypeAmelioration type)
    {
        return dicoAmélioration[type].Item2 > 0;
    }

    public bool IsUpgradeMax(TypeAmelioration type)
    {
        (AmeliorationValueSO so, int level) = dicoAmélioration[type];
        return level >= 2;
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

    public bool GetCurrentStateToNextMiniGame(TypeAmelioration type)
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

    [ContextMenu("Recap")]
    public void RecapOfTheDay()
    {
        int happyPercent = BonheurCalculator.instance.GetBonheurPercentage();
        
        dayRecap.Recap(
            numberDay,
            happyPercent,
            currentMoneyDay,
            currentWoolDay,
            numberDay + 1);
    }
    
    
    public void ResetDayStats()
    {
        if(numberDay == 2)
        {        
            PanneauShop.ActivateExclamation();
            bergerie.ActivateExclamation();
        }
        
        currentMoneyDay = 0;
        currentWoolDay = 0;

        if (is2eTuto)
        {
            IsStatGame = true;
            papy.SetActive(true);
            NextMessage(true);
        }
    }

    public void StartGame()
    {
        IsStatGame = true;
        GetComponent<ActivateStartUI>().ActivateUI();
        isTuto = false;
    }
    
    #region 2eTuto
    
    private void StopTuto()
    {
        IsStatGame = false;
        is2eTuto = false;
        papy.SetActive(false);
    }
    
    public void NextMessage(bool isFirst = false)
    {
        if (isWritting) return;
        
        nextMessage.SetActive(false);
        
        idMessage++;
        
        if (idMessage >= messages.Length)
        {
            StopTuto();
            
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(WriteSmooth(messages[idMessage], isFirst));
    }


    IEnumerator WriteSmooth(string fullMessage, bool isFirst = false, float charDelay = 0.025f)
    {
        isWritting = true;
        
        AudioManager.instance.PlaySound(32);
        
        if(!isFirst)
        {
            papy.GetComponent<Animator>().SetTrigger("NextMessage");

            yield return new WaitForSeconds(0.3f);
        }
        
        AudioManager.instance.PlaySound(43);
        
        message.text = "";
        foreach (char c in fullMessage)
        {
            message.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        
        nextMessage.SetActive(true);
        isWritting = false;
    }
    
    #endregion
}

public class UpgradesSaveData
{
    public Dictionary<TypeAmelioration, int> niveaux =  new Dictionary<TypeAmelioration, int>();
}
