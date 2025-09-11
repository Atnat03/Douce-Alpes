using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[DefaultExecutionOrder(-1)]
public class TouchManager : MonoBehaviour
{
    public static TouchManager instance;

    public PlayerInput playerInput;
    public float holdThreshold = 1f;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private GameObject currentTouchedObject;
    private float pressStartTime;
    public bool isHolding = false;

    private Swipe swipeInput;
    
    #region Events
    public delegate void StartTouch(Vector2 position, float timer);
    public event StartTouch OnStartEvent;
    public delegate void EndTouch(Vector2 position, float timer);
    public event EndTouch OnEndEvent;
    #endregion
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
        
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];

        swipeInput = new Swipe();
    }

    private void OnEnable()
    {
        touchPressAction.performed += OnTouchPressed;
        touchPressAction.canceled += OnTouchReleased;
        swipeInput.Enable();
    }

    private void OnDisable()
    {
        touchPressAction.performed -= OnTouchPressed;
        touchPressAction.canceled -= OnTouchReleased;
        swipeInput.Disable();
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

        if (!isHolding && pressDuration >= holdThreshold && currentTouchedObject != null)
        {
            Sheep sheep = currentTouchedObject.GetComponent<Sheep>();
            if (sheep != null)
            {
                sheep.StartHolding();
                isHolding = true;
            }
        }

        if (isHolding)
        {
            Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                currentTouchedObject.GetComponent<Sheep>().WidowOpen();
            }
        }
    }
    
    private void Start()
    {
        swipeInput.Swiping.PrimaryTouch.started += ctx => StartTouchPrimary(ctx);
        swipeInput.Swiping.PrimaryTouch.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext ctx)
    {
        OnStartEvent?.Invoke(swipeInput.Swiping.PrimaryPosition.ReadValue<Vector2>(), (float)ctx.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext ctx)
    {
        OnEndEvent?.Invoke(swipeInput.Swiping.PrimaryPosition.ReadValue<Vector2>(), (float)ctx.time);
    }

    public Vector2 PrimaryPosition()
    {
        return swipeInput.Swiping.PrimaryPosition.ReadValue<Vector2>();
    }

}
