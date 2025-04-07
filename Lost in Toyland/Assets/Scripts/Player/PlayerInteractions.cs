using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerInteractions : MonoBehaviour
{
    public Transform startPosition;

    public LayerMask maskDoorOpen;
    public LayerMask maskDoorClose;
    public LayerMask maskDoorLocked;
    public float distancia = 5f;

    public GameObject infoPickUpItemGunAmmo;
    public GameObject infoPickUpItemFrozenGunAmmo;
    public GameObject infoPickUpItemGrenade;
    public GameObject infoPickUpItemHealthObject;

    private GameObject nearbyObject = null;

    public GameObject infoTrampBox;
    public GameObject infoGiftBox;

    public GameObject infoTextOpenDoor;
    public GameObject infoTextCloseDoor;
    public GameObject infoTextLockedDoor;
    public GameObject lastDetected = null;

    public LockedDoor door;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    private void Awake()
    {

        maskDoorClose = LayerMask.GetMask("RaycastDetectDoorClose");
        maskDoorOpen = LayerMask.GetMask("RaycastDetectDoorOpen");
        infoTextOpenDoor.SetActive(false);
        infoTextCloseDoor.SetActive(false);
        infoTextLockedDoor.SetActive(false);
        infoPickUpItemGunAmmo.SetActive(false);
        infoPickUpItemFrozenGunAmmo.SetActive(false);
        infoPickUpItemGrenade.SetActive(false);
        infoPickUpItemHealthObject.SetActive(false);
        infoTrampBox.SetActive(false);
        infoGiftBox.SetActive(false);

        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
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
            if (nearbyObject.CompareTag("GiftBox"))
            {
                GiftBox giftOpen = nearbyObject.GetComponent<GiftBox>();
                if (giftOpen != null && giftOpen.giftObjects.Length > 0)
                {
                    giftOpen.OpenBox();
                    SoundManager.Instance.PlaySound2D("Gift");
                    infoGiftBox.SetActive(false);
                    nearbyObject = null;
                }
            }
            else if (nearbyObject.CompareTag("TrampBox"))
            {
                TrampBox trampOpen = nearbyObject.GetComponent<TrampBox>();
                if (trampOpen != null)
                {
                    trampOpen.OpenBox();
                    SoundManager.Instance.PlaySound2D("PlayerHit");
                    infoTrampBox.SetActive(false);
                    nearbyObject = null;
                }
            }
            else
            {
                PickUpItem(nearbyObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunAmmo"))
        {
            infoPickUpItemGunAmmo.SetActive(true);
            nearbyObject = other.gameObject;
        }
        else if (other.CompareTag("Grenade"))
        {
            infoPickUpItemGrenade.SetActive(true);
            nearbyObject = other.gameObject;
        }
        else if (other.CompareTag("FrozenGunAmmo"))
        {
            infoPickUpItemFrozenGunAmmo.SetActive(true);
            nearbyObject = other.gameObject;
        }
        else if (other.CompareTag("HealthObject"))
        {
            infoPickUpItemHealthObject.SetActive(true);
            nearbyObject = other.gameObject;
        }
        else if (other.CompareTag("TrampBox"))
        {
            infoTrampBox.SetActive(true);
            nearbyObject = other.gameObject;
        }
        else if (other.CompareTag("GiftBox"))
        {
            infoGiftBox.SetActive(true);
            nearbyObject = other.gameObject;
        }

        if (other.gameObject.CompareTag("DeathFloor"))
        {
            GameManager.Instance.LoseHealth(50);

            GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = startPosition.position;
            GetComponent<CharacterController>().enabled = true;
        }

        if (other.CompareTag("Level2Trigger"))
        {
            GetComponent<CharacterController>().enabled = false;

            MusicManager.Instance.PlayMusic("Level2", 0.5f);
            LevelManager.Instance.LoadScene("Level_2", "CrossFade");
        }

        if (other.CompareTag("Level3Trigger"))
        {
            GetComponent<CharacterController>().enabled = false;
            MusicManager.Instance.PlayMusic("Level3", 0.5f);
            LevelManager.Instance.LoadScene("Level_3", "CrossFade");            
        }

        if (other.CompareTag("FinalGameTrigger"))
        {
            GetComponent<CharacterController>().enabled = false;
            MusicManager.Instance.PlayMusic("FinalCredits", 0.5f);
            LevelManager.Instance.LoadScene("Final_Credits", "CrossFade");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GunAmmo"))
        {
            infoPickUpItemGunAmmo.SetActive(false);
        }
        else if (other.gameObject.CompareTag("FrozenGunAmmo"))
        {
            infoPickUpItemFrozenGunAmmo.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Grenade"))
        {
            infoPickUpItemGrenade.SetActive(false);
        }
        else if (other.gameObject.CompareTag("HealthObject"))
        {
            infoPickUpItemHealthObject.SetActive(false);
        }
        else if (other.CompareTag("TrampBox"))
        {
            infoTrampBox.SetActive(false);
        }
        else if (other.CompareTag("GiftBox"))
        {
            infoGiftBox.SetActive(false);
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
            if (transform.gameObject.tag == "Door") 
            {
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
            else if (transform.gameObject.tag == "DoorLocked")
            {
                if (meshRenderer != null)
                {
                    infoTextLockedDoor.SetActive(true);
                    meshRenderer.material.color = Color.red;
                    lastDetected = transform.gameObject;
                }
                else
                {
                    meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
                    infoTextLockedDoor.SetActive(true);
                    meshRenderer.material.color = Color.red;
                    lastDetected = transform.gameObject;
                }

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
            infoTextLockedDoor.SetActive(false);
            lastDetected = null;
        }
    }

    void PickUpItem(GameObject item)
    {
        if (item.CompareTag("GunAmmo"))
        {
            SoundManager.Instance.PlaySound2D("Pickup");
            GameManager.Instance.gunAmmo += item.GetComponent<AmmoBox>().ammo;
        }
        else if (item.CompareTag("FrozenGunAmmo"))
        {
            SoundManager.Instance.PlaySound2D("Pickup");
            GameManager.Instance.frozenAmmo += item.GetComponent<FrozenOrbe>().frozenAmmo;
        }
        else if (item.CompareTag("Grenade"))
        {
            SoundManager.Instance.PlaySound2D("Pickup");
            GameManager.Instance.grenadeAmmo += item.GetComponent<GrenadeObject>().grenade;
        }
        else if (item.CompareTag("HealthObject"))
        {
            SoundManager.Instance.PlaySound2D("Pickup");
            GameManager.Instance.AddHealth(item.GetComponent<HealthObject>().health);
        }

        Destroy(item);
        infoPickUpItemGunAmmo.SetActive(false);
        infoPickUpItemFrozenGunAmmo.SetActive(false);
        infoPickUpItemGrenade.SetActive(false);
        infoPickUpItemHealthObject.SetActive(false);
        
        nearbyObject = null;
    }
}
