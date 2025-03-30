using System.Collections;
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
    
    private PlayerController playerControllerBulletParent;

    [SerializeField]
    private float bulletHitMissDistance = 25f;

    public PlayerInput playerInput;
    private InputAction shootAction;

    public enum ShootMode
    {
        Single,
        Auto
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
        shooting = true;
        Shoot();
    }

    private void Update()
    {
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
                        ShootGun();
                        break;
                    case ShootMode.Auto:
                        StartCoroutine(AutomaticShoot());
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

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
            {
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

    IEnumerator AutomaticShoot()
    {
        while (shooting)
        {
            ShootGun();
            yield return new WaitForSeconds(shootDelay);
        }
    }
}

