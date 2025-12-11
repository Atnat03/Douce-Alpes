using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraRotation : MonoBehaviour
{
    [SerializeField] private float dragSensitivity = 0.2f; 
    [SerializeField] private float minY = -33f;
    [SerializeField] private float maxY = 33f;

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

    private void OnDragCamera(List<Vector2> points)
    {
        if (points.Count < 2) return;

        Vector2 last = points[points.Count - 1];
        Vector2 prev = points[points.Count - 2];

        float deltaX = last.x - prev.x;

        currentY += -deltaX * dragSensitivity;

        currentY = Mathf.Clamp(currentY, minY, maxY);

        transform.rotation = Quaternion.Euler(30f, currentY, 0f);
    }
}