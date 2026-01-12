using UnityEngine;

public class AbreuvoirClickOpen : TouchableObject
{
    [SerializeField] private Abreuvoir abreuvoir;
    [SerializeField] private Transform camPos; 
    
    public Transform targetTransiPos;

    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject tropdoInfo;
    [SerializeField] private Transform TtropdoInfo;

    public override void TouchEvent()
    {
        if (!abreuvoir.CanAbreuvoir())
        {
            Instantiate(tropdoInfo, TtropdoInfo);
            return;
        }
        
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;

        if (cameraControl.IsCameraMoving)
            return;
        
        if (cameraControl.gameObject.GetComponent<ChangingCamera>().isInTransition)
            return;
        
        if(TutoManager.instance != null)
            TutoManager.instance.Abreuvoir();
        
        exclamation.SetActive(false);
        
        ActivateAbreuvoir();
    }

    public void ActivateAbreuvoir()
    {
        GameManager.instance.ChangeCameraState(CamState.Drink);
        GameManager.instance.ChangeCameraPos(camPos.transform.position, camPos.transform.localEulerAngles, targetTransiPos);
        abreuvoir.EnableEau();
    }
}
