using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    public static TouchManager instance;

    public PlayerInput playerInput;
    public float holdThreshold = 1f;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private GameObject currentTouchedObject;
    private float pressStartTime;
    private bool isHolding = false;

    private void Awake()
    {
        instance = this;
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += OnTouchPressed;
        touchPressAction.canceled += OnTouchReleased;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= OnTouchPressed;
        touchPressAction.canceled -= OnTouchReleased;
    }

    private void OnTouchPressed(InputAction.CallbackContext context)
    {
        pressStartTime = Time.time;
        isHolding = false;

        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentTouchedObject = hit.transform.gameObject;
        }
    }

    private void OnTouchReleased(InputAction.CallbackContext context)
    {
        if (currentTouchedObject == null) return;

        float pressDuration = Time.time - pressStartTime;

        if (pressDuration < holdThreshold)
        {
            TouchableObject touchable = currentTouchedObject.GetComponent<TouchableObject>();
            if (touchable != null)
                touchable.TouchEvent();
        }
        else if (isHolding)
        {
            Sheep sheep = currentTouchedObject.GetComponent<Sheep>();
            if (sheep != null)
                sheep.OnTouchEnd();
        }

        currentTouchedObject = null;
        isHolding = false;
    }

    private void Update()
    {
        if (currentTouchedObject == null && GameManager.instance.currentCameraState == CamState.Default) return;

        float pressDuration = Time.time - pressStartTime;

        if (!isHolding && pressDuration >= holdThreshold)
        {
            Sheep sheep = currentTouchedObject.GetComponent<Sheep>();
            if (sheep != null)
            {
                sheep.OnTouchStart();
                isHolding = true;
            }
        }

        if (isHolding)
        {
            Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                currentTouchedObject.transform.position = hit.point + Vector3.up * 0.5f;
            }
        }
    }
}
