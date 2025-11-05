using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public Transform root;
    [SerializeField] public Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float positionSmoothTime = 0.2f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Smooth position
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);
        transform.position = smoothedPosition;

        if (root != null)
            root.position = smoothedPosition;

        // 2. Smooth rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothTime);
    }
}