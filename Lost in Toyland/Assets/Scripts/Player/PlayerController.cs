using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    public float speed = 6f;

    private Vector2 movementInput;

    private float gravity = -14.81f;

    public Transform groundCheck;
    public float sphereRadius = 0.3f;
    public LayerMask groundMask;

    bool isGrounded;
    
    Vector3 velocity;

    public float jumpHeight = 3f;

    public bool isSprinting;
    public float sprintingSpeedMultiplier = 1.5f;
    private float sprintSpeed = 1f;

    public float staminaUseAmount = 5f;
    private StaminaBar staminaSlider;

    public bool nonGun = false;
    public bool hasPistol = false;
    public bool hasRifle = false;

    //Item
    public GameObject nearItem;
    public GameObject[] itemPrefab;
    public GameObject[] itemSlot;

    private void Start()
    {
        staminaSlider = FindFirstObjectByType<StaminaBar>();

        GameObject instantiatedNonGun;
        instantiatedNonGun = Instantiate(itemPrefab[0], itemSlot[0].transform.position, itemSlot[0].transform.rotation);
        instantiatedNonGun.transform.parent = itemSlot[0].transform;
        nonGun = true;
        nearItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        MoveLogic();

        GunLogic();
    }

    public void MoveLogic()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Keyboard.current == null) return;

        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 move = transform.right * x + transform.forward * z;

        JumpCheck();

        RunCheck();

        characterController.Move(move * speed * Time.deltaTime * sprintSpeed);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void JumpCheck()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
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

    public void CameraLogic()
    {

    }

    public void GunLogic()
    {
        if (nearItem != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject instantiatedItem;
            if (nearItem.gameObject.CompareTag("ShortGun"))
            {
                instantiatedItem = Instantiate(itemPrefab[1], itemSlot[1].transform.position, itemSlot[1].transform.rotation);
                Destroy(nearItem.gameObject);
                instantiatedItem.transform.parent = itemSlot[1].transform;
                hasPistol = true;
                nearItem = null;
            }
            else if (nearItem.gameObject.CompareTag("Rifle"))
            {
                instantiatedItem = Instantiate(itemPrefab[2], itemSlot[2].transform.position, itemSlot[2].transform.rotation);
                Destroy(nearItem.gameObject);
                instantiatedItem.transform.parent = itemSlot[2].transform;
                hasRifle = true;
                nearItem = null;
            }
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

    public void AnimLogic()
    {

    }
}
