using UnityEngine;

public class TouchableObject : MonoBehaviour
{
    public virtual void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.Default)
            return;
        
        Debug.Log("Touch : " + gameObject.name);
    }
}
