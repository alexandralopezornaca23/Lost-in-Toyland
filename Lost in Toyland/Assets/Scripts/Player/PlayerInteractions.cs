using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public Transform startPosition;

    public LayerMask maskDoorOpen;
    public LayerMask maskDoorClose;
    public float distancia = 5f;

    public GameObject infoPickUpItem;
    private GameObject nearbyObject = null;

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
        infoPickUpItem.SetActive(false);
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

        if (nearbyObject != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUpItem(nearbyObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunAmmo") || other.CompareTag("FrozenGunAmmo") || other.CompareTag("Grenade") || other.CompareTag("HealthObject"))
        {
            infoPickUpItem.SetActive(true);
            nearbyObject = other.gameObject;
        }

        if (other.gameObject.CompareTag("DeathFloor"))
        {
            GameManager.Instance.LoseHealth(50);

            GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = startPosition.position;
            GetComponent<CharacterController>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GunAmmo"))
        {
            infoPickUpItem.SetActive(false);
        }

        if (other.gameObject.CompareTag("FrozenGunAmmo"))
        {           
            infoPickUpItem.SetActive(false);
        }

        if (other.gameObject.CompareTag("Grenade"))
        {
            infoPickUpItem.SetActive(false);
        }

        if (other.gameObject.CompareTag("HealthObject"))
        {
            infoPickUpItem.SetActive(false);
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

    void PickUpItem(GameObject item)
    {
        if (item.CompareTag("GunAmmo"))
        {
            GameManager.Instance.gunAmmo += item.GetComponent<AmmoBox>().ammo;
        }
        else if (item.CompareTag("FrozenGunAmmo"))
        {
            GameManager.Instance.frozenAmmo += item.GetComponent<FrozenOrbe>().frozenAmmo;
        }
        else if (item.CompareTag("Grenade"))
        {
            GameManager.Instance.grenadeAmmo += item.GetComponent<GrenadeObject>().grenade;
        }
        else if (item.CompareTag("HealthObject"))
        {
            GameManager.Instance.AddHealth(item.GetComponent<HealthObject>().health);
        }

        Destroy(item);
        infoPickUpItem.SetActive(false);
        nearbyObject = null;
    }
}
