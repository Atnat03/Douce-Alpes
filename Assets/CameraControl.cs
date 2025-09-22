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

    private bool zooming = false;

    private float boundUp, boundDown, boundLeft, boundRight;
    private float angle;
    private float zoom;
    private float zoomMax;
    private float zoomMin;

    private Vector2 zoomPositionOnScreen = Vector2.zero;
    private Vector3 zoomPositionInWorld = Vector3.zero;
    private float zoomBaseValue = 0;
    private float zoomBaseDistance = 0;

    private Vector3 center = Vector3.zero;

    public Transform centerPoint;
    public Transform root;
    public Transform pivot;
    public Transform target;

    public Text nbInputText;

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
        inputs.Main.TouchZoom.started += _ => ZoomPressed();
        inputs.Main.TouchZoom.canceled += _ => zooming = false;
    }

    private void OnDisable()
    {
        inputs.Main.TouchZoom.started -= _ => ZoomPressed();
        inputs.Main.TouchZoom.canceled -= _ => zooming = false;
        inputs.Disable();
    }

    private void ZoomPressed()
    {
        Vector2 touch0 = inputs.Main.TouchPosition0.ReadValue<Vector2>();
        Vector2 touch1 = inputs.Main.TouchPosition1.ReadValue<Vector2>();

        if (touch0 == Vector2.zero || touch1 == Vector2.zero) 
            return;

        zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
        zoomPositionInWorld = ScreenToWorldPointOnPlane(zoomPositionOnScreen);
        zoomBaseValue = zoom;

        touch0 /= new Vector2(Screen.width, Screen.height);
        touch1 /= new Vector2(Screen.width, Screen.height);

        zoomBaseDistance = Vector2.Distance(touch0, touch1);
        zooming = true;
    }


    private void Update()
    {
        // Smooth follow
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, zoomSmooth * Time.deltaTime);
        cam.transform.position = Vector3.Lerp(cam.transform.position, root.position + target.localPosition, moveSmooth * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, target.rotation, moveSmooth * Time.deltaTime);

        if (GameManager.instance.currentCameraState != CamState.Default) return;
        if (GameManager.instance.shopOpen) return;

        HandleTwoFingerGestures();   // 👉 remplace HandleMove + HandleZoomTouch
        HandleZoomMouse();
        ApplyBounds();
    }

    private void HandleTwoFingerGestures()
    {
        if (!TwoFingersActive() || !zooming)
            return;

        Vector2 touch0 = inputs.Main.TouchPosition0.ReadValue<Vector2>();
        Vector2 touch1 = inputs.Main.TouchPosition1.ReadValue<Vector2>();

        if (touch0 == Vector2.zero || touch1 == Vector2.zero) 
            return;

        Vector2 t0Norm = touch0 / new Vector2(Screen.width, Screen.height);
        Vector2 t1Norm = touch1 / new Vector2(Screen.width, Screen.height);

        float currentDistance = Vector2.Distance(t0Norm, t1Norm);
        float deltaDistance = currentDistance - zoomBaseDistance;

        Vector2 delta0 = inputs.Main.PrimaryTouchDelta.ReadValue<Vector2>();
        Vector2 delta1 = inputs.Main.SecondaryTouchDelta.ReadValue<Vector2>();

        float dot = 0f;
        if (delta0.sqrMagnitude > 0.001f && delta1.sqrMagnitude > 0.001f)
            dot = Vector2.Dot(delta0.normalized, delta1.normalized);

        if (Mathf.Abs(deltaDistance) > 0.01f && dot < 0.7f)
        {
            zoom = Mathf.Clamp(zoomBaseValue - deltaDistance * zoomSpeed, zoomMin, zoomMax);
            Vector3 zoomCenter = ScreenToWorldPointOnPlane(Vector2.Lerp(touch0, touch1, 0.5f));
            root.position += (zoomPositionInWorld - zoomCenter);
        }
        else if (dot > 0.7f)
        {
            // → Déplacement caméra
            Vector2 avgDelta = (delta0 + delta1) / 2f;
            Vector3 moveDelta = new Vector3(-avgDelta.x * moveSpeed * Time.deltaTime, 0, -avgDelta.y * moveSpeed * Time.deltaTime);
            root.position += moveDelta;
        }
    }

    private void HandleZoomMouse()
    {
        float scroll = inputs.Main.MouseScroll.ReadValue<float>();
        if (scroll != 0)
        {
            zoom -= scroll * 300f * Time.deltaTime;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        }
    }

    private bool TwoFingersActive()
    {
        bool first = inputs.Main.PrimaryTouchPress.ReadValue<float>() > 0.5f;
        bool second = inputs.Main.SecondaryTouchPress.ReadValue<float>() > 0.5f;
        return first && second;
    }

    private bool OneFingerActive()
    {
        bool first = inputs.Main.PrimaryTouchPress.ReadValue<float>() > 0.5f;
        bool second = inputs.Main.SecondaryTouchPress.ReadValue<float>() > 0.5f;
        return first && !second;
    }

    private void ApplyBounds()
    {
        root.position = new Vector3(
            Mathf.Clamp(root.position.x, center.x - boundLeft, center.x + boundRight),
            root.position.y,
            Mathf.Clamp(root.position.z, center.z - boundDown, center.z + boundUp)
        );
    }

    private Vector3 ScreenToWorldPointOnPlane(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, root.position);
        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);
        return Vector3.zero;
    }
}
