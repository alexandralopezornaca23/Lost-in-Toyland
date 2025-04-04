using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private Transform cameraTransform;

    [SerializeField]
    private Transform shootSpawn;

    public bool shooting = false;
    public float shootDelay = 0f;
    public float lastShootTime = 0f;

    [SerializeField]
    private GameObject bulletPrefab;
    private GameObject bulletFrozenPrefab;

    private PlayerController playerControllerBulletParent;

    [SerializeField]
    private float bulletHitMissDistance = 25f;

    public PlayerInput playerInput;
    private InputAction shootAction;

    public LayerMask layersToIgnore;

    public enum ShootMode
    {
        Single,
        Auto,
        Frozen
    }
    public ShootMode currentShotMode = ShootMode.Single;

    private float shootRateTime = 0f;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip shootSound;

    private void Start()
    {
        cameraTransform = Camera.main?.transform;
        audioSource = GetComponent<AudioSource>();
        playerControllerBulletParent = Object.FindFirstObjectByType<PlayerController>();

        if (playerInput == null)
        {
            return;
        }

        shootAction = playerInput.actions["Shoot"];
    }

    private void OnEnable()
    {
        if (shootAction != null)
        {
            shootAction.performed += OnShootAction;
        }
    }

    private void OnDisable()
    {
        if (shootAction != null)
        {
            shootAction.performed -= OnShootAction;
        }
    }

    private void OnShootAction(InputAction.CallbackContext context)
    {
        if (playerControllerBulletParent.isPaused) return;

        shooting = true;
        Shoot();
    }

    private void Update()
    {
        if (playerControllerBulletParent.isPaused) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            shooting = true;
            Shoot();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            shooting = false;
        }
    }

    public void Shoot()
    {
        if (Time.time - lastShootTime > shootDelay)
        {
            if (shooting)
            {
                switch (currentShotMode)
                {
                    case ShootMode.Single:
                        ShootInfinityGun();
                        break;
                    case ShootMode.Auto:
                        StartCoroutine(AutomaticShoot());
                        break;
                    case ShootMode.Frozen:
                        ShootFrozenGun();
                        break;
                }
            }
        }
    }

    private void ShootGun()
    {
        if (Time.time > shootRateTime && GameManager.Instance.gunAmmo > 0)
        {
            audioSource.PlayOneShot(shootSound);
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletPrefab, shootSpawn.position, Quaternion.identity, playerControllerBulletParent.bulletParent);
            Bullet bulletController = bullet.GetComponent<Bullet>();

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ~layersToIgnore))
            {
                if (hit.collider != null && hit.collider.CompareTag("Player") || hit.collider.gameObject.CompareTag("Item")
                || hit.collider.gameObject.CompareTag("ShortGun") || hit.collider.gameObject.CompareTag("Rifle") || hit.collider.gameObject.CompareTag("FrozzenGun")
                || hit.collider.gameObject.CompareTag("GunAmmo") || hit.collider.gameObject.CompareTag("FrozenGunAmmo") || hit.collider.gameObject.CompareTag("Grenade")
                || hit.collider.gameObject.CompareTag("HealthObject")
                || hit.collider.gameObject.CompareTag("DoorSound") || hit.collider.gameObject.CompareTag("DoorSoundOpen") || hit.collider.gameObject.CompareTag("DoorSoundClose"))
                {
                    bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance; 
                    bulletController.hit = false;
                    return;
                }
                bulletController.target = hit.point;
                bulletController.hit = true;
            }
            else
            {
                bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                bulletController.hit = false;
            }

            GameManager.Instance.gunAmmo--;
        }
    }

    private void ShootInfinityGun()
    {
        if (Time.time > shootRateTime)
        {
            audioSource.PlayOneShot(shootSound);
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletPrefab, shootSpawn.position, Quaternion.identity, playerControllerBulletParent.bulletParent);
            Bullet bulletController = bullet.GetComponent<Bullet>();

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ~layersToIgnore))
            {
                if (hit.collider != null && hit.collider.CompareTag("Player") || hit.collider.gameObject.CompareTag("Item")
                || hit.collider.gameObject.CompareTag("ShortGun") || hit.collider.gameObject.CompareTag("Rifle") || hit.collider.gameObject.CompareTag("FrozzenGun")
                || hit.collider.gameObject.CompareTag("GunAmmo") || hit.collider.gameObject.CompareTag("FrozenGunAmmo") || hit.collider.gameObject.CompareTag("Grenade")
                || hit.collider.gameObject.CompareTag("HealthObject")
                || hit.collider.gameObject.CompareTag("DoorSound") || hit.collider.gameObject.CompareTag("DoorSoundOpen") || hit.collider.gameObject.CompareTag("DoorSoundClose"))
                {
                    bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                    bulletController.hit = false;
                    return;
                }
                bulletController.target = hit.point;
                bulletController.hit = true;
            }
            else
            {
                bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                bulletController.hit = false;
            }
        }
    }

    private void ShootFrozenGun()
    {
        if (Time.time > shootRateTime && GameManager.Instance.frozenAmmo > 0)
        {
            audioSource.PlayOneShot(shootSound);
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletPrefab, shootSpawn.position, Quaternion.identity, playerControllerBulletParent.bulletParent);
            BulletFrozen bulletController = bullet.GetComponent<BulletFrozen>();

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ~layersToIgnore))
            {
                if (hit.collider != null && hit.collider.CompareTag("Player") || hit.collider.gameObject.CompareTag("Item")
                || hit.collider.gameObject.CompareTag("ShortGun") || hit.collider.gameObject.CompareTag("Rifle") || hit.collider.gameObject.CompareTag("FrozzenGun")
                || hit.collider.gameObject.CompareTag("GunAmmo") || hit.collider.gameObject.CompareTag("FrozenGunAmmo") || hit.collider.gameObject.CompareTag("Grenade")
                || hit.collider.gameObject.CompareTag("HealthObject")
                || hit.collider.gameObject.CompareTag("DoorSound") || hit.collider.gameObject.CompareTag("DoorSoundOpen") || hit.collider.gameObject.CompareTag("DoorSoundClose"))
                {
                    bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                    bulletController.hit = false;
                    return;
                }
                bulletController.target = hit.point;
                bulletController.hit = true;
            }
            else
            {
                bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
                bulletController.hit = false;
            }

            GameManager.Instance.frozenAmmo--;
        }
    }

    IEnumerator AutomaticShoot()
    {
        while (shooting)
        {
            ShootGun();
            yield return new WaitForSeconds(shootDelay);
        }
    }
}

