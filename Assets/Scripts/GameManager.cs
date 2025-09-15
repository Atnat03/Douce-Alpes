using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Rendering.LookDev;

public enum CamState
{
    Default,
    Sheep,
    MiniGame
}

public class SheepData
{
    public int id;
    public string name;
    public int skin;

    public SheepData(int id, string name, int skin)
    {
        this.id = id;
        this.name = name;
        this.skin = skin;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event Action<Vector3, Vector3> SheepClicked;
    public event Action<Vector3, Vector3> GrangeClicked;
    
    public CamState currentCameraState = CamState.Default;
    
    CameraFollow cameraFollow;

    [SerializeField] private GameObject[] UItoDisableWhenSheepIsOn;

    [SerializeField] public List<Sheep> sheepList;
    [SerializeField] private List<SheepData> sheepDestroyData;

    [Header("Bonheur")] 
    [SerializeField] private float currentBonheur;
    [SerializeField] private float maxBonheur;
    [SerializeField] Text txtBonheur;
    
    [Header("Sheep")]
    [SerializeField] private int SheepCount;
    [SerializeField] private float caresseValue = 10f;
    [SerializeField] private float saturationValue = 0.1f;
    [SerializeField] private float maxSaturation;
    [SerializeField] private GameObject sheepWidow;
    
    [Header("Grange Mini Game")]
    [SerializeField] private Transform miniGameCamPos;
    [SerializeField] public Grange grange;
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private Transform sheepSpawn;
    [SerializeField] private GameObject uiMiniGame;
    
    private void Awake()
    {
        instance = this;
        
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        sheepWidow.SetActive(false);
    }

    private void Start()
    {
        sheepDestroyData = new List<SheepData>();
    }

    public Sheep GetSheep(int idSheep)
    {
        return sheepList.FirstOrDefault(s => s.sheepId == idSheep);
    }
    
    public GameObject GetSheepWindow(){return  sheepWidow;}
    
    public void ChangeCameraState(CamState newState)
    {
        currentCameraState = newState;
        cameraFollow.enabled = false;
        ChangePlayerEnvironnement(false);
    }

    public void ResetCamera()
    {
        currentCameraState = CamState.Default;
        cameraFollow.enabled = true;
        
        ChangePlayerEnvironnement(true);

        Sheep sheep = GetSheep(SheepWindow.instance.GetCurrentSheepID());
        if(sheep != null) sheep.StopAgentAndDesactivateScript(false);
        
        sheepWidow.SetActive(false);
        SheepWindow.instance.ResetValue();
    }

    public Transform GetMiniGameCamPos() { return miniGameCamPos; }

    public void ChangePlayerEnvironnement(bool state)
    {
        foreach (GameObject obj in UItoDisableWhenSheepIsOn)
        {
            obj.SetActive(state);
        }
    }

    public void ChangeCameraPos(Vector3 pos, Vector3 rot)
    {
        switch (currentCameraState)
        {
            case CamState.Sheep:
                SheepClicked?.Invoke(pos, rot);
                break;
            case CamState.MiniGame:
                GrangeClicked?.Invoke(pos, rot);
                break;
            case CamState.Default:
                break;
        }
    }

    public void Caresse()
    {
        currentBonheur += caresseValue / saturationValue;
        saturationValue += 0.3f;
    }

    private void Update()
    {
        //txtBonheur.text = (int)((currentBonheur / maxBonheur) * 100) + " %";
        txtBonheur.text = currentBonheur.ToString();
        
        saturationValue -= Time.deltaTime;
        if (saturationValue >= maxSaturation) saturationValue = maxSaturation;
        if (saturationValue <= 0.1) saturationValue = 0.1f;
        
        uiMiniGame.SetActive(CamState.MiniGame == currentCameraState);
    }
    
    //GrangeMiniGame
    public void SheepEnterGrange(Sheep sheep)
    {
        if (!sheepList.Contains(sheep)) Debug.LogError("Le mouton n'existe pas");

        SheepData newDataSheep = new SheepData(sheep.sheepId, sheep.name, sheep.currentSkin);
        sheepDestroyData.Add(newDataSheep);

        sheepList.Remove(sheep);
        Destroy(sheep.gameObject);

        if (sheepList.Count == 0)
            sheepList = new List<Sheep>();
    }
    
    public void SheepGetOutGrange()
    {
        List<SheepData> toRemove = new List<SheepData>();

        foreach (SheepData sheepData in sheepDestroyData)
        {
            GameObject newSheep = Instantiate(sheepPrefab, sheepSpawn.position, sheepSpawn.rotation, null);
            Sheep sheep = newSheep.GetComponent<Sheep>();
        
            sheep.sheepId = sheepData.id;
            sheep.name = sheepData.name;
            sheep.currentSkin = sheepData.skin;
        
            sheepList.Add(sheep);
            toRemove.Add(sheepData);
        }

        sheepDestroyData = new List<SheepData>();
    }
}
