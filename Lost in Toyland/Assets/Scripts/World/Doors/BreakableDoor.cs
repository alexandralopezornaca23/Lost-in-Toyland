using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableDoor : MonoBehaviour
{
    public GameObject doorPieces;
    public GameObject doorAll;
    public GameObject explosion;

    public GameObject enemyBreack;

    public AudioClip explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == enemyBreack)
        {
            GameObject instantiatedPieces = Instantiate(doorPieces, transform.position, transform.rotation);
            Instantiate(explosion, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1);
            Destroy(doorAll);
            Destroy(instantiatedPieces, 3f);
            Destroy(this.gameObject);
        }
    }
}
