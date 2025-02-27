using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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

    private void Start()
    {
        staminaSlider = FindFirstObjectByType<StaminaBar>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");

        //Vector3 move = transform.right * x + transform.forward * z;

        //characterController.Move(move);

        if (Keyboard.current == null) return;

        // Leer el input con el nuevo sistema
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    isSprinting = !isSprinting; //cambiar a correr o no.
        //}

        //if (isSprinting == true)
        //{
        //    sprintSpeed = sprintingSpeedMultiplier;
        //}
        //else
        //{
        //    sprintSpeed = 1f;
        //}

        if (isSprinting == true) // Mientras se mantiene presionado
        {
            sprintSpeed = sprintingSpeedMultiplier;
        }
        else
        {
            sprintSpeed = 1f;
        }
    }
}
