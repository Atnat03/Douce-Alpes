using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [HideInInspector] public List<SheepData> sheepDestroyData = new ();
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;
    
    [Header("Saving data")]
    public double timeWhenLoad;
    public double timeSinceLastLoad;
    
    public List<int> unlockedSkinIDs = new ();
    public event Action OnSkinsUpdated;
    
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

}
