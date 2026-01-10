using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public static void SavePlayer(Saving manager)
    {
        PlayerData data = new PlayerData(manager);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Save JSON OK : " + SavePath);
    }

    public static PlayerData LoadPlayer()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        Debug.Log("Load JSON OK");
        return data;
    }
    
    public static void ResetSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save reset: " + SavePath);
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

}

[System.Serializable]
public class PlayerData
{
    public double lastTime;
    public bool isInGrange;
    public int nbSheep;
    public List<SheepData> sheepList;
    public int nbDay;

    public int gold;
    public int wool;
    public float happiness;
    
    public int currentTrictotProduct;
    public int currentTricotPattern;

    public SkinAgencySaveData skinAgency;
    
    public MiniGames currentMiniJeuToDo;
    public bool canButtonG;
    public bool canButtonT;
    public bool canButtonC;
    
    public TricotSaveData tricotData;
    
    public UpgradesSaveData upgrades;
    
    public int timeBetweenSave;

    public bool isTuto;
    
    public PlayerData(Saving manager)
    {
        lastTime = manager.curTime;
        isInGrange = GameData.instance.isSheepInside;

        nbDay = GameData.instance.numberDay;
        
        sheepList = BuildSheepDataList();
        nbSheep = GameData.instance.nbSheep;
        
        skinAgency = SkinAgency.instance.BuildSaveData();

        gold = PlayerMoney.instance.currentMoney;
        wool = PlayerMoney.instance.currentWhool;

        happiness = BonheurCalculator.instance.currentBonheur;
        
        tricotData = TricotManager.instance?.GetCurrentTricotSaveData() 
                     ?? new TricotSaveData();
        
        upgrades = GameData.instance.GetSaveData()
                   ?? new UpgradesSaveData();
        
        TimerManager timer = GameData.instance.timer;
        if (timer != null)
        {
            currentMiniJeuToDo = timer.currentMiniJeuToDo;
            canButtonG = timer.canButtonG;
            canButtonT = timer.canButtonT;
            canButtonC = timer.canButtonC;
        }
        
        isTuto = GameData.instance.isTuto;
    }

    private List<SheepData> BuildSheepDataList()
    {
        if (GameData.instance.isSheepInside)
        {
            return new List<SheepData>(GameData.instance.sheepDestroyData);
        }

        List<SheepData> list = new();

        foreach (Sheep sheep in GameManager.instance.sheepList)
        {
            SheepData data = new SheepData(
                sheep.sheepId,
                sheep.sheepName,
                sheep.currentSkinHat,
                sheep.currentSkinClothe,
                sheep.hasLaine,
                sheep.GetComponent<SheepBoid>().natureType,
                sheep.currentColorID,
                sheep.birthDate
            );
            data.position = sheep.transform.position;
            list.Add(data);
        }

        return list;
    }
}

