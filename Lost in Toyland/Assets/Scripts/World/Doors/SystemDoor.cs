using UnityEngine;
using UnityEngine.Audio;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;        //Verificar si la puerta esta abierta o cerrada.
    public float doorOpenAngle = 95f;    //Angulo de la puerta al estar abierta.
    public float doorCloseAngle = 0.0f;  //Angulo de la puerta al estar cerrada.
    public float smooth = 3.0f;          //Velocidad de rotacion d el puerta.

    public AudioClip openDoor;
    public AudioClip closeDoor;

    public void ChangeDoorState()
    {
        doorOpen = !doorOpen;
    }

    // Update is called once per frame
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
            AudioSource.PlayClipAtPoint(openDoor, transform.position, 1);
        }

        if (other.tag == "DoorSoundClose")
        {
            AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1);
        }
    }
}
