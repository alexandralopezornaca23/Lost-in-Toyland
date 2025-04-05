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

        if (!doorOpen && candado != null)
        {
            Destroy(candado);
        }

        doorOpen = !doorOpen;
    }

    public void ChangeLayerWhenUnlocked()
    {
        gameObject.layer = LayerMask.NameToLayer("RaycastDetectDoorOpen");
    }
}
