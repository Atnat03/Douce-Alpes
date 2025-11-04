using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera cam;
    [SerializeField] public Transform root;
    [SerializeField] private Transform centerPoint;

    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float moveSmoothTime = 0.2f; 

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomSmooth = 5f;
    [SerializeField] private float zoomMin = 30f;
    [SerializeField] private float zoomMax = 60f;
    [SerializeField] private float zoomStart = 45f;

    [Header("Rotation / Angle")]
    [SerializeField] private float angle = 45f;

    [Header("Bounds (modifiable en jeu)")]
    [SerializeField] private float boundRight = 20f;
    [SerializeField] private float boundUp = 20f;
    [SerializeField] private float boundLeft = 20f;
    [SerializeField] private float boundDown = 20f;
    [SerializeField] private float bounceStrength = 5f; 
    [SerializeField] private float bounceDamping = 5f;  

    [Header("Debug Options")]
    [SerializeField] private bool showDebugBounds = true;
    [SerializeField] private Color debugColor = Color.green;

    private Movements inputs;
    private float zoom;
    private Vector3 center = Vector3.zero;

    // Pour l'inertie
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private void Awake() => inputs = new Movements();

    private void Start()
    {
        center = centerPoint.position;
        zoom = zoomStart;

        cam.fieldOfView = zoom;
        root.position = center + new Vector3(0, 7.5f, -40);
        root.localEulerAngles = new Vector3(angle, -60, 0);

        targetPosition = root.position;
    }

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

    private void Update()
    {
        // Zoom lissé
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * zoomSmooth);
        cam.transform.position = root.position;
        cam.transform.rotation = root.rotation;

        if (GameManager.instance.currentCameraState != CamState.Default) return;
        if (GameManager.instance.shopOpen) return;

        HandleGestures();
        ApplyBounds();

        root.position = Vector3.SmoothDamp(root.position, targetPosition, ref velocity, moveSmoothTime);
    }

    public void ResetFOV() => zoom = zoomStart;

    private void HandleGestures()
    {
        Vector2 primaryDelta = inputs.Main.PrimaryTouchDelta.ReadValue<Vector2>();
        bool primaryPressed = inputs.Main.PrimaryTouchPress.ReadValue<float>() > 0.5f;

        Vector2 secondaryDelta = inputs.Main.SecondaryTouchDelta.ReadValue<Vector2>();
        bool secondaryPressed = inputs.Main.SecondaryTouchPress.ReadValue<float>() > 0.5f;

        if (primaryPressed && secondaryPressed)
        {
            Vector2 p1 = inputs.Main.PrimaryTouchPosition.ReadValue<Vector2>();
            Vector2 p2 = inputs.Main.SecondaryTouchPosition.ReadValue<Vector2>();

            Vector2 prevP1 = p1 - primaryDelta;
            Vector2 prevP2 = p2 - secondaryDelta;

            float prevDist = (prevP1 - prevP2).magnitude;
            float currDist = (p1 - p2).magnitude;
            float delta = prevDist - currDist;

            zoom += delta * zoomSpeed * Time.deltaTime;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        }
        else if (primaryPressed && primaryDelta != Vector2.zero)
        {
            Vector3 moveRight = cam.transform.right * primaryDelta.x * moveSpeed * Time.deltaTime;
            Vector3 moveForward = cam.transform.forward * primaryDelta.y * moveSpeed * Time.deltaTime;

            moveRight.y = 0;
            moveForward.y = 0;

            targetPosition += (-moveRight) + (-moveForward);
        }
    }

    private void ApplyBounds()
    {
        Vector3 offset = Vector3.zero;

        if (targetPosition.x < center.x - boundLeft)
            offset.x = (center.x - boundLeft) - targetPosition.x;
        else if (targetPosition.x > center.x + boundRight)
            offset.x = (center.x + boundRight) - targetPosition.x;

        if (targetPosition.z < center.z - boundDown)
            offset.z = (center.z - boundDown) - targetPosition.z;
        else if (targetPosition.z > center.z + boundUp)
            offset.z = (center.z + boundUp) - targetPosition.z;

        targetPosition += offset * bounceStrength * Time.deltaTime;

        velocity *= Mathf.Exp(-bounceDamping * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        if (!showDebugBounds || centerPoint == null) return;

        Gizmos.color = debugColor;
        Vector3 centerPos = centerPoint.position;
        Vector3 size = new Vector3(boundLeft + boundRight, 0.1f, boundUp + boundDown);
        Gizmos.DrawWireCube(centerPos + new Vector3((boundRight - boundLeft) / 2f, 0, (boundUp - boundDown) / 2f), size);
    }
}
