using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController characterController;
    public PlayerInput playerInput;
    private InputAction moveAction;

    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        if (GameManager.instance.currentCameraState != CamState.Default) return;
        
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        characterController.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
    }
}