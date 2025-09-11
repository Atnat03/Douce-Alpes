using System;
using UnityEngine;
using UnityEngine.UI;

public enum CamState
{
    Default,
    Sheep,
    MiniGame
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event Action<Vector3, Vector3> SheepClicked;
    
    public CamState currentCameraState = CamState.Default;
    
    CameraFollow cameraFollow;

    [SerializeField] private GameObject[] UItoDisableWhenSheepIsOn;

    [SerializeField] private Sheep[] sheepList;

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
    
    private void Awake()
    {
        instance = this;
        
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        sheepWidow.SetActive(false);
    }

    public Sheep GetSheep(int idSheep)
    {
        return sheepList[idSheep];
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
        sheepWidow.SetActive(false);
    }

    public void ChangePlayerEnvironnement(bool state)
    {
        foreach (GameObject obj in UItoDisableWhenSheepIsOn)
        {
            obj.SetActive(state);
        }
    }

    public void ChangeCameraPos(Vector3 pos, Vector3 rot)
    {
        if (currentCameraState == CamState.Default)
        {
            ChangeCameraState(CamState.Sheep);
            SheepClicked?.Invoke(pos, rot);
        }
    }

    public void Caresse()
    {
        currentBonheur += caresseValue / saturationValue;
        saturationValue += 0.3f;
    }

    private void Update()
    {
        txtBonheur.text = (int)((currentBonheur / maxBonheur) * 100) + " %";
        saturationValue -= Time.deltaTime;
        if (saturationValue >= maxSaturation) saturationValue = maxSaturation;
        if (saturationValue <= 0.1) saturationValue = 0.1f;
    }
}
