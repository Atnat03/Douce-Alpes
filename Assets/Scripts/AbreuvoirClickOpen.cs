using UnityEngine;

public class AbreuvoirClickOpen : TouchableObject
{
    [SerializeField] private Abreuvoir abreuvoir;
    [SerializeField] private Transform camPos; 
    
    public Transform targetTransiPos;

    [SerializeField] private CameraControl cameraControl;

    public override void TouchEvent()
    {
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;

        if (cameraControl.IsCameraMoving)
            return;
        
        if(TutoManager.instance != null)
            TutoManager.instance.Abreuvoir();
        
        ActivateAbreuvoir();
    }

    public void ActivateAbreuvoir()
    {
        GameManager.instance.ChangeCameraState(CamState.Drink);
        GameManager.instance.ChangeCameraPos(camPos.transform.position, camPos.transform.localEulerAngles, targetTransiPos);
        abreuvoir.EnableEau();
    }
}
