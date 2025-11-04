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
    [SerializeField] private float moveSmooth = 5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomSmooth = 5f;
    [SerializeField] private float zoomMin = 30f;
    [SerializeField] private float zoomMax = 60f;
    [SerializeField] private float zoomStart = 45f;

    [Header("Rotation / Angle")]
    [SerializeField] private float angle = 45f;

    [Header("Bounds (modifiable en jeu)")]
    [Tooltip("Limite vers la droite depuis le centre")]
    [SerializeField] private float boundRight = 20f;

    [Tooltip("Limite vers le haut depuis le centre (Z positif)")]
    [SerializeField] private float boundUp = 20f;

    [Tooltip("Limite vers la gauche depuis le centre")]
    [SerializeField] private float boundLeft = 20f;

    [Tooltip("Limite vers le bas depuis le centre (Z négatif)")]
    [SerializeField] private float boundDown = 20f;

    [Header("Debug Options")]
    [SerializeField] private bool showDebugBounds = true;
    [SerializeField] private Color debugColor = Color.green;

    private Movements inputs;
    private float zoom;
    private Vector3 center = Vector3.zero;

    private void Awake()
    {
        inputs = new Movements();
    }

    private void Start()
    {
        // Initialisation configurable directement dans l'inspecteur
        center = centerPoint.position;
        zoom = zoomStart;

        cam.fieldOfView = zoom;
        root.position = center + new Vector3(0, 7.5f, -40);
        root.localEulerAngles = new Vector3(angle, -60, 0);
    }

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

    private void Update()
    {
        // Mise à jour de la caméra
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * zoomSmooth);
        cam.transform.position = root.position;
        cam.transform.rotation = root.rotation;

        // Empêche les mouvements dans certains états
        if (GameManager.instance.currentCameraState != CamState.Default) return;
        if (GameManager.instance.shopOpen) return;

        HandleGestures();
        ApplyBounds();
    }

    public void ResetFOV() => zoom = zoomStart;

    private void HandleGestures()
    {
        Vector2 primaryDelta = inputs.Main.PrimaryTouchDelta.ReadValue<Vector2>();
        bool primaryPressed = inputs.Main.PrimaryTouchPress.ReadValue<float>() > 0.5f;

        Vector2 secondaryDelta = inputs.Main.SecondaryTouchDelta.ReadValue<Vector2>();
        bool secondaryPressed = inputs.Main.SecondaryTouchPress.ReadValue<float>() > 0.5f;

        // Zoom avec deux doigts
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
        // Déplacement avec un doigt
        else if (primaryPressed && primaryDelta != Vector2.zero)
        {
            Vector3 moveDelta = new Vector3(
                primaryDelta.y * moveSpeed * Time.deltaTime,
                0,
                -primaryDelta.x * moveSpeed * Time.deltaTime
            );

            root.position += moveDelta;
        }
    }

    private void ApplyBounds()
    {
        Vector3 clampedPos = new Vector3(
            Mathf.Clamp(root.position.x, center.x - boundLeft, center.x + boundRight),
            root.position.y,
            Mathf.Clamp(root.position.z, center.z - boundDown, center.z + boundUp)
        );
        
        root.position = clampedPos;
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
