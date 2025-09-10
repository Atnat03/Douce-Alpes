using UnityEngine;

public class TouchableObject : MonoBehaviour
{
    public virtual void TouchEvent()
    {
        Debug.Log("Touch");
    }
}
