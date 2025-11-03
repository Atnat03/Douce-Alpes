using System;
using UnityEngine;

public class Grange : Build
{
    [SerializeField] private GameObject sheepDestroyer;

    private int nbSheepInGrange = 0;
    
    [Header("Gates")]
    public bool gateState = false;
    
    [SerializeField] private Vector3 gate1_Close, gate1_Open;
    [SerializeField] private GameObject gate1;
    
    [SerializeField] private Vector3 gate2_Close, gate2_Open;
    [SerializeField] private GameObject gate2;

    [SerializeField] private GameObject keyCloseGate;

    [SerializeField] private Poutre poutre;
    
    public void LaunchMiniGame()
    {
        GameManager.instance.ChangeCameraState(CamState.MiniGame);
        DesactivateUI();
        OpenDoors();
        UpdateCameraZoom();
    }

    public void OpenDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Open);
        gate2.transform.rotation = Quaternion.Euler(gate2_Open);
        gateState = true;
    }
    
    public void CloseDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Close);
        gate2.transform.rotation = Quaternion.Euler(gate2_Close);
        gateState = false;
    }

    public void AddSheepInGrange()
    {
        nbSheepInGrange++;
        UpdateCameraZoom();
    }
    
    private void UpdateCameraZoom()
    {
        int totalSheep = GameData.instance.nbSheep;
        Debug.Log("Total sheep: " + totalSheep);

        if (nbSheepInGrange >= totalSheep && totalSheep > 0)
        {
            BonheurCalculator.instance.AddBonheur(GameData.instance.GetLevelUpgrade(TypeAmelioration.Rentree));
            
            CloseDoors();
            ZoomCamera();
            keyCloseGate.SetActive(true);
        }
        else
        {
            OpenDoors();
            MiniGameCamera();
            keyCloseGate.SetActive(false);
        }
    }

    private void ZoomCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameZoomCamPos().position,
            GameManager.instance.GetMiniGameZoomCamPos().rotation.eulerAngles,
            transform
        );
    }

    private void MiniGameCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameCamPos().position,
            GameManager.instance.GetMiniGameCamPos().rotation.eulerAngles,
            transform
        );
    }

    private void Update()
    {
        sheepDestroyer.SetActive(GameManager.instance.currentCameraState == CamState.MiniGame);
    }
    
    public Poutre GetPoutre() { return poutre; }
    public Transform GetSheepDestroyer() { return sheepDestroyer.transform; }
}
