using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject bullet;

    public float shotForce = 2000f;
    public float shotRate = 0.2f;

    private float shotRateTime = 0f;

    private AudioSource audioSource;
    public AudioClip shotSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time > shotRateTime && GameManager.Instance.gunAmmo > 0)
            {
                audioSource.PlayOneShot(shotSound);

                GameManager.Instance.gunAmmo--;

                GameObject newBullet;
                newBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);

                newBullet.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * shotForce);

                shotRateTime = Time.time + shotRate;

                Destroy(newBullet, 5);
            }
        }
    }
}
