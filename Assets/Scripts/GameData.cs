using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [HideInInspector] public List<SheepData> sheepDestroyData = new List<SheepData>();
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;
    
    [Header("Saving data")]
    public double timeWhenLoad;
    public double timeSinceLastLoad;
    
    private void Awake()
    {
        instance = this;

        Saving.instance.savingEvent += SaveMyData;
        Saving.instance.loadingEvent += LoadMyData;
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

}
