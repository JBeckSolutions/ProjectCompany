using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Jobs;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using static UnityEditor.Progress;

public class PlayerController : NetworkBehaviour
{
    public bool controllsEnabled = true;
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float sensitivity = 0.2f;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject playerCamera;

    [SerializeField] private PlayerInventoryManager playerInventory;
    [SerializeField] private bool jumpedThisFrame = false;

    [Header("Stamina")]
    [SerializeField] private float stamina = 5f;
    [SerializeField] private float currentStamina = 0;
    [SerializeField] private float MaxTimeUntilStaminaRefresh = 3;
    [SerializeField] private float timeUntilStaminaRefresh = 0;

    private Vector2 moveInput;
    private float yAxisVelocity;
    private Vector2 lookInput;
    private float xRotation = 0;

    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private InteractableUi interactablUi;
    [SerializeField] private GameUi gameUi;

    private bool sprinting = false;

    public override void OnNetworkSpawn()
    {

        currentStamina = stamina;
        gameUi = GameObject.Find("CanvasGameUi").GetComponent<GameUi>();
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerInput>().enabled = false;
        }

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        if (!IsOwner) return;
        HandleLook();
        HandleMovementAndAnimation();


        //Handle Interactable Ui
        interactablUi.InteractSquare.SetActive(false);
        interactablUi.Value.text = "";
        interactablUi.Weight.text = "";
        interactablUi.Name.text = "";
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, 5f))
        {
            if (hitinfo.collider.GetComponent<Item>() is Item item)
            {
                interactablUi.InteractSquare.SetActive(true);
                interactablUi.Value.text = "Value: " + item.itemValue;
                interactablUi.Weight.text = "Weight: " + item.ItemWeight;
                interactablUi.Name.text = item.itemName;
            }
            if (hitinfo.collider.GetComponent<InteractableObject>() is InteractableObject interactableObject)
            {
                interactablUi.InteractSquare.SetActive(true);
                interactablUi.Name.text = interactableObject.ObjectName;

            }
        }
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
    private void HandleMovementAndAnimation()
    {
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        Vector2 _moveInput = moveInput; //Helper variable so moveInput doesnt multiply infinitely

        timeUntilStaminaRefresh -= Time.deltaTime;

        float weightMultiplier = 1f - (playerInventory.PlayerWeight * 0.1f);

        if (sprinting && currentStamina > 0)
        {
            timeUntilStaminaRefresh = MaxTimeUntilStaminaRefresh;
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
            _moveInput *= sprintSpeed * weightMultiplier;
        }
        else
        {
            if (timeUntilStaminaRefresh <= 0)
            {
                currentStamina += Time.deltaTime * 1.5f;
                currentStamina = Mathf.Min(currentStamina, stamina);
            }

            _moveInput *= movementSpeed * weightMultiplier;
        }

        inventoryUi.StaminaBar.localScale = new Vector3(Mathf.Clamp(currentStamina / stamina, 0, 1), 1, 1);

        //Set animation for client
        animator.SetFloat("Speed", _moveInput.magnitude);
        if (jumpedThisFrame)
        {
            animator.SetTrigger("JumpTrigger");
        }

        //Set animation for everyone else
        HandleAnimationServerRpc(_moveInput.magnitude, jumpedThisFrame);
        jumpedThisFrame = false;

        Vector3 moveDirection = (forward.normalized * _moveInput.y) + (right.normalized * _moveInput.x);
        yAxisVelocity += gravityValue * Time.deltaTime;
        moveDirection.y = yAxisVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
        if (characterController.isGrounded && yAxisVelocity < 0)
        {
            yAxisVelocity = 0f;
        }
    }
    [ServerRpc]
    private void HandleAnimationServerRpc(float speed, bool jumpedThisFrame)
    {
        HandleAnimationClientRpc(speed, jumpedThisFrame);
    }
    [ClientRpc]
    private void HandleAnimationClientRpc(float speed, bool jumpedThisFrame)
    {
        if (IsOwner) return;
        animator.SetFloat("Speed", speed);

        if (jumpedThisFrame)
        {
            animator.SetTrigger("JumpTrigger");
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!controllsEnabled)
        {
            moveInput = new Vector2(0, 0);
            return;
        }
        moveInput = context.ReadValue<Vector2>().normalized;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (characterController.isGrounded && context.started)
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
        if (!controllsEnabled) return;
        if (characterController.isGrounded && context.started && playerInventory.PlayerWeight < 4)
        {
            yAxisVelocity = 0;
            yAxisVelocity += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            jumpedThisFrame = true;
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!controllsEnabled)
        {
            lookInput = new Vector2(0, 0);
            return;
        }
        lookInput = context.ReadValue<Vector2>();
        //Debug.Log(lookInput);
    }
    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                gameUi.OpenMenu("PauseMenu");
                controllsEnabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                gameUi.CloseAllMenus();
                controllsEnabled = true;            }
        }
    }
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.DropItem();
        }
    }
    #region InventorySlotControls

    public void OnPreviousInventorySlot(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(null, -1);
        }
    }

    public void OnNextInventorySlot(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(null, 1);
        }
    }

    public void OnInventorySlotOne(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(0);
        }
    }
    public void OnInventorySlotTwo(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(1);
        }
    }
    public void OnInventorySlotThree(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(2);
        }
    }
    public void OnInventorySlotFour(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerInventory.ChangeActiveInventorySlot(3);
        }
    }
    #endregion
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        //Debug.Log("Interact");
        if (context.started)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(screenCenter);
            Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 5f))
            {
                //Debug.Log(hitinfo.transform.name);
                if (hitinfo.collider.GetComponent<Item>())
                {
                    playerInventory.AddItem(hitinfo.collider.GetComponent<Item>());
                }
                if (hitinfo.collider.GetComponent<InteractableObject>())
                {
                    hitinfo.collider.GetComponent<InteractableObject>().Use();
                }
            }
        }
    }

}
