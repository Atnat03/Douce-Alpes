using UnityEngine;

public class SetFOVLikeParent : MonoBehaviour
{
    void Update()
    {
        GetComponent<Camera>().fieldOfView = transform.parent.GetComponent<Camera>().fieldOfView;
    }
}
