using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Jobs;

public class PlayerController : NetworkBehaviour
{
    public float MovementSpeed = 4f;
    private Vector2 moveInput;

    //Disable PlayerInput component the other Player gameobjects to avoid conflicts
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) gameObject.GetComponent<PlayerInput>().enabled = false;
    }
    //Move when the Move input event is triggered
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        //Debug.Log("Moving: " + moveInput);
    }
    private void FixedUpdate()
    {
        this.transform.position += new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * MovementSpeed;
    }
}
