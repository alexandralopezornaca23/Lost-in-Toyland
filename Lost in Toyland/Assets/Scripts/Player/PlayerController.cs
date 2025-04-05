using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public bool isPaused = false;

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

    //sounds
    private float stepTimer = 0f;
    [SerializeField] private float stepInterval = 0.5f;
    private bool wasGroundedLastFrame = true;

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
    public GameObject infoPickUpInfinityWeapon;
    public GameObject infoPickUpItemAutomaticWeapon;
    public GameObject infoPickUpItemFrozenWeapon;

    public bool hasNonGun = true;
    public bool hasPistol = false;
    public bool hasRifle = false;
    public bool hasFrozenGun = false;

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

        infoPickUpInfinityWeapon.SetActive(false);
        infoPickUpItemAutomaticWeapon.SetActive(false);
        infoPickUpItemFrozenWeapon.SetActive(false);
}

    private void Start()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        staminaSlider = FindFirstObjectByType<StaminaBar>();

        GameObject instantiatedNonGun;
        instantiatedNonGun = Instantiate(itemPrefab[0], itemSlot[0].transform.position, itemSlot[0].transform.rotation);
        instantiatedNonGun.transform.parent = itemSlot[0].transform;
        nearItem = null;
    }

    void Update()
    {
        if (isPaused) return;

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && !wasGroundedLastFrame)
        {
            StartCoroutine(PlayLandingSound());
        }

        wasGroundedLastFrame = groundedPlayer;

        GunLogic();
        RunCheck();

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("isJumping", false);

            if (currentAnimationBlendVector.magnitude > 0.1f)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepInterval)
                {
                    string soundToPlay = isSprinting ? "PlayerRunSteps" : "PlayerWalkSteps";
                    SoundManager.Instance.PlaySound2D(soundToPlay);
                    stepTimer = 0f;
                }

                animator.SetFloat(moveXAnimationParametrerID, currentAnimationBlendVector.x);
                animator.SetFloat(moveZAnimationParametrerID, currentAnimationBlendVector.y);
            }
            else
            {
                stepTimer = 0f;
                animator.SetFloat(moveXAnimationParametrerID, 0);
                animator.SetFloat(moveZAnimationParametrerID, 0);
            }
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed * sprintSpeed);

        animator.SetFloat(moveXAnimationParametrerID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParametrerID, currentAnimationBlendVector.y);

        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetBool("isJumping", true);

            if (isSprinting && groundedPlayer)
            {
                animator.SetBool("isSprinting", false);
                animator.SetTrigger("jumpWhileSprinting");
            }
            SoundManager.Instance.PlaySound2D("PlayerJump");
        }

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("isJumping", false);

            StartCoroutine(PlayLandingSound());

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

    IEnumerator PlayLandingSound()
    {
        SoundManager.Instance.PlaySound2D("PlayerLand");
        yield return new WaitForSeconds(0.2f);
    }

    public void GunLogic()
    {
        if (isPaused) return;

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
            else if (nearItem.gameObject.CompareTag("FrozenGun"))
            {
                instantiatedItem = Instantiate(itemPrefab[3], itemSlot[3].transform.position, itemSlot[3].transform.rotation);
                Destroy(nearItem.gameObject);
                instantiatedItem.transform.parent = itemSlot[3].transform;
                hasFrozenGun = true;
            }
            else
            {
                return;
            }

            weaponController = instantiatedItem.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.playerInput = playerInput;
            }

            nearItem = null;
            infoPickUpInfinityWeapon.SetActive(false);
            infoPickUpItemAutomaticWeapon.SetActive(false);
            infoPickUpItemFrozenWeapon.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            nearItem = other.gameObject;
            if (nearItem.gameObject.CompareTag("ShortGun"))
            {
                infoPickUpInfinityWeapon.SetActive(true);
            }
            else if (nearItem.gameObject.CompareTag("Rifle"))
            {
                infoPickUpItemAutomaticWeapon.SetActive(true);
            }
            else if (nearItem.gameObject.CompareTag("FrozenGun"))
            {
                infoPickUpItemFrozenWeapon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            nearItem = null;
            infoPickUpInfinityWeapon.SetActive(false);
            infoPickUpItemAutomaticWeapon.SetActive(false);
            infoPickUpItemFrozenWeapon.SetActive(false);
        }
    }
}