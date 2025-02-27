using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    float countdown;
    public float radius = 5f;
    public float explosionForce = 70;
    bool exploded = false;

    public GameObject explosionEffect;

    private AudioSource audioSource;
    public AudioClip explosionSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0 && exploded == false) 
        {
            Explode();
            exploded = true;
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var rangeObjects in colliders)
        {
            Rigidbody rb = rangeObjects.GetComponent<Rigidbody>();

            if (rb!= null)
            {
                rb.AddExplosionForce(explosionForce * 10, transform.position, radius);
            }
        }

        audioSource.PlayOneShot(explosionSound);

        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        Destroy(gameObject, delay * 2);
    }
}
