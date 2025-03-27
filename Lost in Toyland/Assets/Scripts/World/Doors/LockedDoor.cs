using UnityEngine;
using UnityEngine.Audio;

public class LockedDoor : MonoBehaviour
{
    public bool isUnloocked = false;
    public bool doorOpen = false;
    public float doorOpenAngle = 90f;
    public float doorCloseAngle = 0f;
    public float smooth = 5f;

    public GameObject candado;

    void Update()
    {
        if (!isUnloocked) return;

        Quaternion targetRotation = doorOpen ? Quaternion.Euler(0, doorOpenAngle, 0) : Quaternion.Euler(0, doorCloseAngle, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void ChangeDoorState()
    {
        if (!isUnloocked)
        {
            return;
        }

        // Si la puerta se va a abrir, destruimos el candado
        if (!doorOpen && candado != null)
        {
            Destroy(candado); // Destruir el candado cuando la puerta se abre
        }

        doorOpen = !doorOpen;
    }
}
