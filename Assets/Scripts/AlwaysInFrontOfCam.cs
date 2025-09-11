using UnityEngine;

public class AlwaysInFrontOfCam : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}