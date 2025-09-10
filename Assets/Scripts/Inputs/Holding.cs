using UnityEngine;
using UnityEngine.InputSystem;

public class Holding : MonoBehaviour
{
    public PlayerInput playerInput;

    private InputAction holdAction;
    private InputAction touchPositionAction;
    private GameObject currentHeldObject;
    
    private float distanceFromCamera;

    private void Awake()
    {
        holdAction = playerInput.actions["Holding"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        holdAction.performed += OnTouchPressed;
        holdAction.canceled += OnTouchReleased;
    }

    private void OnDisable()
    {
        holdAction.performed -= OnTouchPressed;
        holdAction.canceled -= OnTouchReleased;
    }

    private void OnTouchPressed(InputAction.CallbackContext context)
    {
        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Hold"))
        {
            currentHeldObject = hit.transform.gameObject;
            currentHeldObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            
            distanceFromCamera = Vector3.Distance(Camera.main.transform.position, currentHeldObject.transform.position);
        }
    }

    private void OnTouchReleased(InputAction.CallbackContext context)
    {
        if (currentHeldObject != null)
        {
            currentHeldObject.GetComponent<MeshRenderer>().material.color = Color.green;
            currentHeldObject = null;
        }
    }

    private void Update()
    {
        if (currentHeldObject != null)
        {
            Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                currentHeldObject.transform.position = hit.point + Vector3.up;
            }
        }
    }

}