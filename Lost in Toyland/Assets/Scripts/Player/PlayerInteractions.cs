using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public Transform startPosition;

    //Objetos Interactivos
    public LayerMask maskDoorOpen;
    public LayerMask maskDoorClose;
    public float distancia = 5f;

    public GameObject infoTextOpenDoor;
    public GameObject infoTextCloseDoor;
    public GameObject lastDetected = null;

    public LockedDoor door;

    private void Awake()
    {

        maskDoorClose = LayerMask.GetMask("RaycastDetectDoorClose");
        maskDoorOpen = LayerMask.GetMask("RaycastDetectDoorOpen");
        infoTextOpenDoor.SetActive(false);
        infoTextCloseDoor.SetActive(false);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distancia, maskDoorOpen))
        {
            Deselect();
            SelectedObject(hit.transform);

            if (hit.collider.tag == "Door")
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    hit.collider.transform.GetComponentInParent<SystemDoor>().ChangeDoorState();
                }
            }

            if (hit.collider.tag == "DoorLocked" && door != null && door.isUnloocked)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    door.ChangeDoorState();
                }
            }
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distancia, maskDoorClose))
        {
            Deselect();
            SelectedObject(hit.transform);

            if (hit.collider.tag == "Door")
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    hit.collider.transform.GetComponentInParent<SystemDoor>().ChangeDoorState();
                }
            }

            if (hit.collider.tag == "DoorLocked" && door != null && door.isUnloocked)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    door.ChangeDoorState();
                }
            }
        }
        else
        {
            Deselect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GunAmmo"))
        {
            GameManager.Instance.gunAmmo += other.gameObject.GetComponent<AmmoBox>().ammo;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Grenade"))
        {
            GameManager.Instance.grenadeAmmo += other.gameObject.GetComponent<GrenadeObject>().grenade;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("HealthObject"))
        {
            GameManager.Instance.AddHealth(other.gameObject.GetComponent<HealthObject>().health);
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("DeathFloor"))
        {
            //Perder vida, respawnear a nuestro personaje
            GameManager.Instance.LoseHealth(50);

            GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = startPosition.position;
            GetComponent<CharacterController>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.CompareTag("EnemyBullet"))
       {
            GameManager.Instance.LoseHealth(10);
       }
    }

    void SelectedObject(Transform transform)
    {
        if (transform.gameObject.layer == LayerMask.NameToLayer("RaycastDetectDoorOpen"))
        {
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                infoTextOpenDoor.SetActive(true);
                meshRenderer.material.color = Color.green;
                lastDetected = transform.gameObject;
            }
            else
            {
                meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
                infoTextOpenDoor.SetActive(true);
                meshRenderer.material.color = Color.green;
                lastDetected = transform.gameObject;
            }
        }
        else if (transform.gameObject.layer == LayerMask.NameToLayer("RaycastDetectDoorClose"))
        {
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                infoTextCloseDoor.SetActive(true);
                meshRenderer.material.color = Color.red;
                lastDetected = transform.gameObject;
            }
            else
            {
                meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
                infoTextCloseDoor.SetActive(true);
                meshRenderer.material.color = Color.red;
                lastDetected = transform.gameObject;
            }
        }
    }

    public void Deselect()
    {
        if (lastDetected)
        {
            MeshRenderer meshRenderer = lastDetected.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                meshRenderer.material.color = Color.white;
            }
            else
            {
                meshRenderer = lastDetected.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material.color = Color.white;
            }

            infoTextOpenDoor.SetActive(false);
            infoTextCloseDoor.SetActive(false);
            lastDetected = null;
        }
    }
}
