using UnityEngine;

public class ChangeCameraRotation : MonoBehaviour
{
    private float[] allowedY = { -33f, 0f, 33f };
    private int currentIndex = 1;

    [SerializeField] private float rotationSpeed = 120f;
    private Quaternion targetRotation;

    private void Start()
    {
        SwipeDetection.instance.OnSwipeDetected += SwapCameraRotation;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void SwapCameraRotation(SwipeType direction)
    {
        if (direction == SwipeType.Left)
        {
            if (currentIndex < allowedY.Length - 1)
                currentIndex++;
        }
        else if (direction == SwipeType.Right)
        {
            if (currentIndex > 0)
                currentIndex--;
        }

        Vector3 currentEuler = transform.rotation.eulerAngles;
        currentEuler.y = allowedY[currentIndex];
        targetRotation = Quaternion.Euler(currentEuler);
    }
}