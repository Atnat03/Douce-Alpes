using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraRotation : MonoBehaviour
{
    [SerializeField] private float dragSensitivity = 0.2f;
    [SerializeField] private float minY = -33f;
    [SerializeField] private float maxY = 33f;

    [Header("Bounce")]
    [SerializeField] private float bounceStrength = 8f;     
    [SerializeField] private float bounceReturnSpeed = 6f;

    private float currentY;

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
        currentY = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (currentY < minY)
            currentY = Mathf.Lerp(currentY, minY, Time.deltaTime * bounceReturnSpeed);

        if (currentY > maxY)
            currentY = Mathf.Lerp(currentY, maxY, Time.deltaTime * bounceReturnSpeed);

        transform.rotation = Quaternion.Euler(17f, currentY, 0f);
    }

    private void OnDragCamera(List<Vector2> points)
    {
        if (points.Count < 2) return;

        Vector2 last = points[points.Count - 1];
        Vector2 prev = points[points.Count - 2];

        float deltaX = last.x - prev.x;

        currentY += -deltaX * dragSensitivity;

        currentY = Mathf.Clamp(currentY, minY - bounceStrength, maxY + bounceStrength);
    }
}