using System;
using UnityEngine;

public enum CamState
{
    Default,
    Sheep,
    MiniGame
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event Action<Vector3> SheepClicked;
    
    public CamState currentCameraState = CamState.Default;
    
    CameraFollow cameraFollow;

    private void Awake()
    {
        instance = this;
        
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void ChangeCameraState(CamState newState)
    {
        currentCameraState = newState;
        cameraFollow.enabled = false;
    }

    public void ResetCamera()
    {
        currentCameraState = CamState.Default;
        cameraFollow.enabled = true;
    }

    public void ChangeCameraPos(Vector3 pos)
    {
        if (currentCameraState == CamState.Default)
        {
            ChangeCameraState(CamState.Sheep);
            SheepClicked?.Invoke(pos);
        }
    }
}
