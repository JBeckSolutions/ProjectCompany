using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Jobs;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private GameObject playerCamera;

    private Vector2 moveInput;
    private float yAxisVelocity;
    private Vector2 lookInput;
    private float xRotation = 0;

    private bool sprinting = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerInput>().enabled = false;
            playerCamera.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        HandleLook();
        HandleMovement();
    }
   
    //Handles Rotation based on the input of the player
    private void HandleLook()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Rotate up/down (clamping to prevent flipping)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate left/right
        transform.Rotate(Vector3.up * mouseX);
    }
    //Handles Movement based on the input of the player
    private void HandleMovement()
    {
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        Vector2 _moveInput = moveInput; //Helper variable so moveInput doesnt multiply infinitly

        if (sprinting)
        {
            _moveInput *= sprintSpeed;
        }
        else
        {
            _moveInput *= movementSpeed;
        }

        Vector3 moveDirection = (forward.normalized * _moveInput.y) + (right.normalized * _moveInput.x);
        yAxisVelocity += gravityValue * Time.deltaTime;
        moveDirection.y = yAxisVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
        if (characterController.isGrounded && yAxisVelocity < 0)
        {
            yAxisVelocity = 0f;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
            moveInput = context.ReadValue<Vector2>().normalized;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded && context.performed)
        {
            sprinting = true;
        }

        if (context.canceled)
        {
            sprinting = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded && context.performed)
        {
            yAxisVelocity = 0;
            yAxisVelocity += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        //Debug.Log(lookInput);
    }
    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    

}
