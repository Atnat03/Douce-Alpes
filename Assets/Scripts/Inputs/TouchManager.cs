using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[DefaultExecutionOrder(-1)]
public class TouchManager : MonoBehaviour
{
    public static TouchManager instance;

    public Action<Vector3> OnCleanTouch;

    public PlayerInput playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private GameObject currentTouchedObject;
    private Swipe swipeInput;

    public GameObject sphereSheepLeak;
    
    [SerializeField] private Vector2 delockAreaSize = new Vector2(600, 600); 
    private Rect delockArea;

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
        } else
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
        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {    
            if (GameManager.instance.isLock && !delockArea.Contains(screenPos))
            {
                Debug.Log("Force delock");
                GameManager.instance.DelockSheep();
            }
            
            currentTouchedObject = hit.transform.gameObject;
            
            Debug.Log(currentTouchedObject.transform.name);
        }
    }


    private void OnTouchReleased(InputAction.CallbackContext context)
    {
        if (currentTouchedObject == null) return;

        TouchableObject touchable = currentTouchedObject.GetComponent<TouchableObject>();
        if (touchable != null)
            touchable.TouchEvent();

        currentTouchedObject = null;
    }

    private void Update()
    {
        if (GameManager.instance.currentCameraState == CamState.MiniGame)
        {
            Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Field"))
                {
                    sphereSheepLeak.SetActive(true);
                    sphereSheepLeak.transform.position = hit.point;
                }
            }
        }
        else
        {
            if(sphereSheepLeak != null)
                sphereSheepLeak.SetActive(false);
        }
    }
    
    private void Start()
    {
        float screenW = Screen.width;
        float screenH = Screen.height;
        delockArea = new Rect(
            (screenW - delockAreaSize.x) / 2f,
            (screenH - delockAreaSize.y) / 2f,
            delockAreaSize.x,
            delockAreaSize.y
        );
        
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
