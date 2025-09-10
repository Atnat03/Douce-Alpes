using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    public static TouchManager instance;

    private PlayerInput inputs;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    private InputAction moveAction;
    
    private void Awake()
    {
        instance = this;
        
        inputs = GetComponent<PlayerInput>();
        touchPressAction = inputs.actions["TouchPress"];
        touchPositionAction = inputs.actions["TouchPosition"];
    }

    void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }
    
    private void TouchPressed(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPositionAction.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.tag == "Touchable")
            {
                hit.transform.gameObject.GetComponent<TouchableObject>().TouchEvent();
            }
        }
        
    }
    
}
