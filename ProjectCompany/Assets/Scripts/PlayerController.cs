using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Jobs;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private CharacterController characterController;

    private Vector2 moveInput;
    private Vector3 playerVelocity;
    private bool jumpThisFrame = false;

    private void Update()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }
   
    //Disable PlayerInput component the other Player gameobjects to avoid conflicts
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) gameObject.GetComponent<PlayerInput>().enabled = false;
    }
    //Move when the Move input event is triggered
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
        playerVelocity.x = moveInput.x;
        playerVelocity.z = moveInput.y;
        //Debug.Log("Moving: " + moveInput);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded && context.performed)
        {
            playerVelocity.y = 0;
            jumpThisFrame = true;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }

}
