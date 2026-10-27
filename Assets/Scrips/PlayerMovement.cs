using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 12f;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController characterController;
    private Animator animator;
    private Player_Controler player; 
    private Vector2 moveInput;
    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = new Player_Controler(); // Inicializa Player_Controller

        player.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        player.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => player.Enable();
    private void OnDisable() => player.Disable();

    private void FixedUpdate()
    {
        Move();
        RotateWithMouse();
    }

    private void RotateWithMouse()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void Move()
    {
        Vector3 move = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)) * moveSpeed;

        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.fixedDeltaTime;
        characterController.Move((move + velocity) * Time.fixedDeltaTime);

        animator.SetFloat("x", moveInput.x);
        animator.SetFloat("y", moveInput.y);
    }
}


