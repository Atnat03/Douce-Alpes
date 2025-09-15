using System;
using UnityEngine;

public class Grange : Build
{
    [SerializeField] private GameObject sheepDestroyer;
    
    [Header("Gates")]
    
    [SerializeField] private Vector3 gate1_Close, gate1_Open;
    [SerializeField] private GameObject gate1;
    
    [SerializeField] private Vector3 gate2_Close, gate2_Open;
    [SerializeField] private GameObject gate2;
    
    public void LaunchMiniGame()
    {
        GameManager.instance.ChangeCameraState(CamState.MiniGame);
        GameManager.instance.ChangeCameraPos(GameManager.instance.GetMiniGameCamPos().position, GameManager.instance.GetMiniGameCamPos().rotation.eulerAngles);
        DesactivateUI();

        OpenDoors();
    }

    public void EndMiniGame()
    {
        CloseDoors();
    }

    void OpenDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Open);
        gate2.transform.rotation = Quaternion.Euler(gate2_Open);
    }
    
    void CloseDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Close);
        gate2.transform.rotation = Quaternion.Euler(gate2_Close);
    }

    private void Update()
    {
        sheepDestroyer.SetActive(GameManager.instance.currentCameraState == CamState.MiniGame);
    }
}
