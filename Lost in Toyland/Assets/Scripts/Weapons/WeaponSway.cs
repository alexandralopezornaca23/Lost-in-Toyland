using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    private Quaternion startRotation;

    public float swayAmount = 0.5f;
    public float swaySpeed = 3f;

    private Vector2 mouseDelta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
    }

    public void OnLook(InputAction.CallbackContext context)  // Para capturar el movimiento del ratón
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void Sway()
    {
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y");

        //Quaternion xAngle = Quaternion.AngleAxis(mouseX * -1.25f, Vector3.up);
        //Quaternion yAngle = Quaternion.AngleAxis(mouseY * 1.25f, Vector3.left);

        //Quaternion targetRotation = startRotation * xAngle * yAngle;

        //transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swayAmount);

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        Quaternion xAngle = Quaternion.AngleAxis(mouseX * -0.5f, Vector3.up);
        Quaternion yAngle = Quaternion.AngleAxis(mouseY * -0.5f, Vector3.left);

        Quaternion targetRotation = startRotation * xAngle * yAngle;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swaySpeed);
    }
}
