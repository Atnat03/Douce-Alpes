using UnityEngine;

public class AbreuvoirClickOpen : TouchableObject
{
    [SerializeField] private Abreuvoir abreuvoir;
    [SerializeField] private Transform camPos; 

    public override void TouchEvent()
    {
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;
        
        if(TutoManager.instance != null)
            TutoManager.instance.Abreuvoir();
        
        ActivateAbreuvoir();
    }

    private void ActivateAbreuvoir()
    {
        GameManager.instance.ChangeCameraState(CamState.Drink);
        GameManager.instance.ChangeCameraPos(camPos.transform.position, camPos.transform.localEulerAngles, abreuvoir.transform);
    }
}
