using UnityEngine;
using UnityEngine.Audio;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;
    public float doorOpenAngle = 95f;    
    public float doorCloseAngle = 0.0f;  
    public float smooth = 3.0f;

    public void ChangeDoorState()
    {
        doorOpen = !doorOpen;
    }

    void Update()
    {
        if (doorOpen)
        {            
            Quaternion targetRotation = Quaternion.Euler (0, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {            
            Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smooth * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "DoorSoundOpen")
        {
            SoundManager.Instance.PlaySound2D("DoorOpen");
        }

        if (other.tag == "DoorSoundClose")
        {
            SoundManager.Instance.PlaySound2D("DoorClose");
        }
    }
}
