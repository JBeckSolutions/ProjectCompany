using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Jobs;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private GameObject playerCamera;

    private Vector2 moveInput;
    private float yAxisVelocity;
    private Vector2 lookInput;
    private float xRotation = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerInput>().enabled = false;
            playerCamera.SetActive(false);
        }
    }
    private void Update()
    {
        HandleLook();
        HandleMovement();
    }
   
    //Disable PlayerInput component the other Player gameobjects to avoid conflicts
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

    private void HandleMovement()
    {
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        Vector3 moveDirection = (forward.normalized * moveInput.y) + (right.normalized * moveInput.x);
        yAxisVelocity += gravityValue * Time.deltaTime;
        moveDirection.y = yAxisVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
        if (characterController.isGrounded && yAxisVelocity < 0)
        {
            yAxisVelocity = 0f;
        }
    }
    //Move when the Move input event is triggered
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized * movementSpeed;
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

}
