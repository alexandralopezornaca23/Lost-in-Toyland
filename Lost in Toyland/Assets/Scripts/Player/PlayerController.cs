using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 4.5f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;

    public bool isSprinting;
    public float sprintingSpeedMultiplier = 1.5f;
    private float sprintSpeed = 1f;

    public float staminaUseAmount = 5f;
    private StaminaBar staminaSlider;

    public bool nonGun = false;
    public bool hasPistol = false;
    public bool hasRifle = false;

    public Transform bulletParent;

    //Item
    public GameObject nearItem;
    public GameObject[] itemPrefab;
    public GameObject[] itemSlot;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];        
    }

    private void Start()
    {
        staminaSlider = FindFirstObjectByType<StaminaBar>();

        GameObject instantiatedNonGun;
        instantiatedNonGun = Instantiate(itemPrefab[0], itemSlot[0].transform.position, itemSlot[0].transform.rotation);
        instantiatedNonGun.transform.parent = itemSlot[0].transform;
        nonGun = true;
        nearItem = null;
    }

    void Update()
    {
        GunLogic();
        RunCheck();

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input =moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Makes the player jump
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //rotate towards camera direction
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void RunCheck()
    {
        if (Keyboard.current.leftShiftKey.isPressed) // Mientras se mantiene presionado
        {
            isSprinting = !isSprinting;

            if (isSprinting == true)
            {
                staminaSlider.UseStamina(staminaUseAmount);
            }
            else
            {
                staminaSlider.UseStamina(0);
            }
        }

        if (isSprinting == true) // Mientras se mantiene presionado
        {
            sprintSpeed = sprintingSpeedMultiplier;
        }
        else
        {
            sprintSpeed = 1f;
        }
    }

    public void GunLogic()
    {
        if (nearItem != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject instantiatedItem;
            WeaponController weaponController;

            if (nearItem.gameObject.CompareTag("ShortGun"))
            {
                instantiatedItem = Instantiate(itemPrefab[1], itemSlot[1].transform.position, itemSlot[1].transform.rotation);
                Destroy(nearItem.gameObject);
                instantiatedItem.transform.parent = itemSlot[1].transform;
                hasPistol = true;
            }
            else if (nearItem.gameObject.CompareTag("Rifle"))
            {
                instantiatedItem = Instantiate(itemPrefab[2], itemSlot[2].transform.position, itemSlot[2].transform.rotation);
                Destroy(nearItem.gameObject);
                instantiatedItem.transform.parent = itemSlot[2].transform;
                hasRifle = true;
            }
            else
            {
                return;
            }

            // Asignar el PlayerInput al arma
            weaponController = instantiatedItem.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.playerInput = playerInput;
            }
            else
            {
                Debug.LogError("WeaponController no encontrado en el arma instanciada.");
            }

            nearItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            nearItem = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            nearItem = null;
        }
    }
}