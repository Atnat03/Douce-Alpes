using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float gravity = -9.8f;
    private PlayerInput inputs;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputs = GetComponent<PlayerInput>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector3 move =  new Vector3(moveInput.x, 0, moveInput.y);
        characterController.Move(move * speed * Time.deltaTime);
        
        velocity.y = gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
