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
    
    public void LaunchMiniGame()
    {
        GameManager.instance.ChangeCameraState(CamState.MiniGame);

        if (!gateState)
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
        sheepDestroyer.SetActive(true);
        gateState = true;
    }
    
    public void CloseDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Close);
        gate2.transform.rotation = Quaternion.Euler(gate2_Close);
        sheepDestroyer.SetActive(false);
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
}
