using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public Transform shootSpawn;
    public bool shooting = false;
    public float shootDelay = 0f;
    public float lastShootTime = 0f;
    public GameObject bulletPrefab;

    public enum ShootMode
    {
        Single,
        Auto
    }
    public ShootMode currentShotMode = ShootMode.Single;




    public float shootForce = 2000f;
    public float shootRate = 0.2f;

    private float shootRateTime = 0f;

    private AudioSource audioSource;
    public AudioClip shootSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        Debug.DrawLine(shootSpawn.position, shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 10f, Color.blue);

        RaycastHit cameraHit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);            
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
                        InstantiateBullet();
                        break;
                    case ShootMode.Auto:
                        StartCoroutine(AutomaticShoot());
                        break;
                }
            }
        }
    }

    public void InstantiateBullet()
    {
        if (Time.time > shootRateTime && GameManager.Instance.gunAmmo > 0)
        {
            audioSource.PlayOneShot(shootSound);

            GameManager.Instance.gunAmmo--;

            GameObject newBullet = Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
            newBullet.GetComponent<Rigidbody>().AddForce(shootSpawn.forward * shootForce);

            shootRateTime = Time.time + shootRate;

            Destroy(newBullet, 4);
        }
    }

    IEnumerator AutomaticShoot()
    {
        while (shooting)
        {
            InstantiateBullet();
            yield return new WaitForSeconds(shootDelay);
        }
    }
}

