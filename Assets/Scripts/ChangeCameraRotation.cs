using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraRotation : MonoBehaviour
{
    [SerializeField] private float dragSensitivity = 0.2f;
    [SerializeField] private float minY = -33f;
    [SerializeField] private float maxY = 33f;

    [Header("Bounce")]
    [SerializeField] private float bounceStrength = 8f;
    [SerializeField] private float bounceReturnSpeed = 0.15f;

    private float currentY;
    private float velocityY;

    private void OnEnable()
    {
        SwipeDetection.instance.OnSwipeUpdated += OnDragCamera;
    }

    private void OnDisable()
    {
        SwipeDetection.instance.OnSwipeUpdated -= OnDragCamera;
    }

    private void Start()
    {
        currentY = NormalizeAngle(transform.eulerAngles.y);
    }

    private void Update()
    {
        float targetY = currentY;

        if (currentY < minY)
            targetY = minY;
        else if (currentY > maxY)
            targetY = maxY;

        currentY = Mathf.SmoothDamp(currentY, targetY, ref velocityY, bounceReturnSpeed);

        transform.rotation = Quaternion.Euler(17f, currentY, 0f);
    }

    private void OnDragCamera(List<Vector2> points)
    {
        if (points.Count < 2) return;

        Vector2 last = points[^1];
        Vector2 prev = points[^2];

        float deltaX = last.x - prev.x;

        currentY -= deltaX * dragSensitivity;

        currentY = Mathf.Clamp(currentY, minY - bounceStrength, maxY + bounceStrength);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}