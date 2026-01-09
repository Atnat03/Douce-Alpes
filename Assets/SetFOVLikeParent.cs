using UnityEngine;

public class SetFOVLikeParent : MonoBehaviour
{
    void Update()
    {
        GetComponent<Camera>().fieldOfView = transform.parent.parent.GetComponent<Camera>().fieldOfView;
    }
}
