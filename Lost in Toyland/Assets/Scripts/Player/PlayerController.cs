using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public Transform spawnPoint;

    [SerializeField] private float playerSpeed = 4.5f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private float animationPlayTransition = 0.15f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    public Animator animator;
    int moveXAnimationParametrerID;
    int moveZAnimationParametrerID;
    public int jumpAnimation;
    public int sprintAnimation;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    public bool isSprinting;
    private float sprintSpeed = 1f;
    public float sprintingSpeedMultiplier = 1.5f;
    [SerializeField] private float staminaDrainRate = 40f;
    public float staminaRecoveryRate = 5f;
    private StaminaBar staminaSlider;

    //GunsWeapons
    public bool hasNonGun = true;
    public bool hasPistol = false;
    public bool hasRifle = false;

    public Transform bulletParent;

    public GameObject nearItem;
    public GameObject[] itemPrefab;
    public GameObject[] itemSlot;

    private void Awake()
    {
        hasNonGun = true;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();

        jumpAnimation = Animator.StringToHash("PlayerJump");
        moveXAnimationParametrerID = Animator.StringToHash("MoveX");
        moveZAnimationParametrerID = Animator.StringToHash("MoveZ");
    }

    private void Start()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation; // Hace que mire en la dirección del SpawnPoint
        }

        staminaSlider = FindFirstObjectByType<StaminaBar>();

        GameObject instantiatedNonGun;
        instantiatedNonGun = Instantiate(itemPrefab[0], itemSlot[0].transform.position, itemSlot[0].transform.rotation);
        instantiatedNonGun.transform.parent = itemSlot[0].transform;
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
            animator.SetBool("isJumping", false);

            // Verifica si el jugador sigue moviéndose
            if (currentAnimationBlendVector.magnitude > 0.1f)
            {
                animator.SetFloat(moveXAnimationParametrerID, currentAnimationBlendVector.x);
                animator.SetFloat(moveZAnimationParametrerID, currentAnimationBlendVector.y);
            }
            else
            {
                animator.SetFloat(moveXAnimationParametrerID, 0);
                animator.SetFloat(moveZAnimationParametrerID, 0);
            }
        }

        Vector2 input =moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed * sprintSpeed);

        //Blend Strafe Animation
        animator.SetFloat(moveXAnimationParametrerID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParametrerID, currentAnimationBlendVector.y);

        // Makes the player jump
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetBool("isJumping", true);

            if (isSprinting && groundedPlayer)
            {
                animator.SetBool("isSprinting", false);  // Desactivar la animación de sprint
                animator.SetTrigger("jumpWhileSprinting");  // Activar el trigger de salto mientras esprinta
            }
        }

        // Al aterrizar, se debe asegurar que la animación de salto se desactive correctamente
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;  // Resetear la velocidad vertical
            animator.SetBool("isJumping", false);  // Desactivar animación de salto

            // Reanudar animación de movimiento o esprint si es necesario
            if (currentAnimationBlendVector.magnitude > 0.1f)
            {
                animator.SetFloat(moveXAnimationParametrerID, currentAnimationBlendVector.x);
                animator.SetFloat(moveZAnimationParametrerID, currentAnimationBlendVector.y);
            }
            else
            {
                animator.SetFloat(moveXAnimationParametrerID, 0);
                animator.SetFloat(moveZAnimationParametrerID, 0);
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //rotate towards camera direction
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void RunCheck()
    {
        if (Keyboard.current.leftShiftKey.isPressed && staminaSlider.currentStamina > 1 && (currentAnimationBlendVector.x != 0 || currentAnimationBlendVector.y != 0 || !groundedPlayer))
        {
            isSprinting = true;
            staminaSlider.UseStamina(staminaDrainRate * Time.deltaTime);
            animator.SetBool("isSprinting", true);
            animator.SetTrigger("jumpWhileSprinting");
        }
        else
        {
            isSprinting = false;
            animator.SetBool("isSprinting", false);
        }

        if (!isSprinting && staminaSlider.currentStamina < staminaSlider.maxStamina)
        {
            staminaSlider.RecoverStamina(staminaRecoveryRate * Time.deltaTime);
        }

        sprintSpeed = isSprinting ? sprintingSpeedMultiplier : 1f;
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