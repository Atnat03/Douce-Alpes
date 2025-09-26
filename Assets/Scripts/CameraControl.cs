using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float moveSmooth = 5f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomSmooth = 5f;

    private Movements inputs;

    private float boundUp, boundDown, boundLeft, boundRight;
    private float angle;
    private float zoom;
    private float zoomMax;
    private float zoomMin;
    private bool isZooming = false;
    
    private Vector3 center = Vector3.zero;

    public Transform centerPoint;
    public Transform root;
    public Transform pivot;
    public Transform target;
    
    private void Awake()
    {
        inputs = new Movements();
    }

    private void Start()
    {
        Initialize(20, 10, 20, 10, 30, 60, 30, 80);
    }

    private void Initialize(float right, float up, float left, float down, float angle, float zoom, float zoomMin, float zoomMax)
    {
        center = centerPoint.position;
        boundRight = right;
        boundUp = up;
        boundLeft = left;
        boundDown = down;
        this.angle = angle;
        this.zoom = zoom;
        this.zoomMax = zoomMax;
        this.zoomMin = zoomMin;

        cam.fieldOfView = zoom;

        pivot.SetParent(root);
        target.SetParent(pivot);

        root.position = center;
        root.localEulerAngles = Vector3.zero;

        pivot.localPosition = Vector3.zero;
        pivot.localEulerAngles = new Vector3(angle, 0, 0);

        target.localPosition = new Vector3(0, 5, -20);
        target.localEulerAngles = Vector3.zero;
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Update()
    {
        cam.fieldOfView = zoom;
        cam.transform.position = root.position + target.localPosition;
        cam.transform.rotation = target.rotation;

        if (GameManager.instance.currentCameraState != CamState.Default) return;
        if (GameManager.instance.shopOpen) return;

        HandleGestures();
        ApplyBounds();
    }

    public void ResetFOV()
    {
        zoom = 60;
    }

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
            Vector3 moveDelta = new Vector3(-primaryDelta.x * moveSpeed * Time.deltaTime, 0, -primaryDelta.y * moveSpeed * Time.deltaTime);
            root.position += moveDelta;
        }
    }

    private void ApplyBounds()
    {
        root.position = new Vector3(
            Mathf.Clamp(root.position.x, center.x - boundLeft, center.x + boundRight),
            root.position.y,
            Mathf.Clamp(root.position.z, center.z - boundDown, center.z + boundUp)
        );
    }
}
