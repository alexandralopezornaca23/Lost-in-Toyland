using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public Transform startPosition;

    //Objetos Interactivos
    LayerMask mask;
    public float distancia = 3f;

    public Texture2D puntero;
    public GameObject infoText;
    GameObject lastDetected = null;

    public LockedDoor door;    

    private void Start()
    {
        mask = LayerMask.GetMask("RaycastDetect");
        infoText.SetActive(false);
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            Deselect();
            SelectedObject(hit.transform);

            if (hit.collider.tag == "Door")
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    hit.collider.transform.GetComponent<SystemDoor>().ChangeDoorState();
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
        transform.GetComponentInChildren<MeshRenderer>().material.color = Color.magenta;
        lastDetected = transform.gameObject;
    }

    void Deselect()
    {
        if (lastDetected)
        {
            lastDetected.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            lastDetected = null;
        }
    }

    void OnGUI()
    {
        Rect rect = new Rect(Screen.width / 2, Screen.height / 2, puntero.height, puntero.height);
        GUI.DrawTexture(rect, puntero);

        if (lastDetected)
        {
            infoText.SetActive(true);
        }
        else
        {
            infoText.SetActive(false);
        }
    }
}
