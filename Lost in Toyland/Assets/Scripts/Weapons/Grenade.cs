using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    float countdown;
    public float radius = 5f;
    public float explosionForce = 70;
    bool exploded = false;
    private bool hasTouchedGround = false;

    public GameObject explosionEffect;

    private AudioSource audioSource;
    public AudioClip explosionSound;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTouchedGround)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !exploded)
            {
                Explode();
                exploded = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasTouchedGround && collision.gameObject.CompareTag("Ground")) // Si toca el suelo
        {
            hasTouchedGround = true;
            countdown = delay; // Reinicia el temporizador al tocar el suelo
            StartCoroutine(StopRolling());
        }
    }

    System.Collections.IEnumerator StopRolling()
    {
        yield return new WaitForSeconds(0.5f); // Espera 1.5 segundos antes de detenerse

        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Detiene el movimiento
            rb.angularVelocity = Vector3.zero; // Detiene la rotación
            rb.isKinematic = true; // Fija la granada en su lugar
        }
    }


    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var rangeObjects in colliders)
        {
            AI ai = rangeObjects.GetComponent<AI>();

            if (ai != null)
            {
                ai.GrenadeImpact();
            }

            Rigidbody rb = rangeObjects.GetComponent<Rigidbody>();

            if (rb!= null)
            {
                rb.AddExplosionForce(explosionForce * 10, transform.position, radius);
            }

            if (rangeObjects.CompareTag("Player"))
            {
                // Reducir la vida del jugador desde el GameManager
                GameManager.Instance.health -= 20; // Ajusta el daño según sea necesario

                // Si el jugador tiene un Rigidbody, aplicar una pequeña fuerza de impacto
                Rigidbody playerRb = rangeObjects.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.AddExplosionForce(explosionForce * 5, transform.position, radius);
                }
            }
        }

        audioSource.PlayOneShot(explosionSound);

        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        Destroy(gameObject, delay * 2);
    }
}
