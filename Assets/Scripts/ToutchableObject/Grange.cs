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

        if (!GameData.instance.isSheepInside)
        {
            GameManager.instance.ChangeCameraPos(GameManager.instance.GetMiniGameCamPos().position, GameManager.instance.GetMiniGameCamPos().rotation.eulerAngles);
            OpenDoors();
        }
        else
        {
            ZoomCamera();
        }
        
        DesactivateUI();
    }

    public void EndMiniGame()
    {
        //CloseDoors();
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
    }

    void ZoomCamera()
    {
        GameManager.instance.ChangeCameraPos(GameManager.instance.GetMiniGameZoomCamPos().position, GameManager.instance.GetMiniGameZoomCamPos().rotation.eulerAngles);
    }

    private void Update()
    {
        sheepDestroyer.SetActive(GameManager.instance.currentCameraState == CamState.MiniGame);
        
        if (GameManager.instance.sheepList.Count == 0)
        {
            keyCloseGate.SetActive(true);
            ZoomCamera();
            CloseDoors();
        }
        else
        {
            keyCloseGate.SetActive(false);
        }
    }
    
    public Poutre GetPoutre(){return  poutre;}
    
    public Transform GetSheepDestroyer(){return sheepDestroyer.transform;}
}
