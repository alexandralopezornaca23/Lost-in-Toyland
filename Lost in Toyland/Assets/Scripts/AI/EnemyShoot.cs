using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject enemyBullet;

    public Transform spawnBulletPoint;
    private Transform playerPosition;

    public float bulletVelocity = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerPosition = FindObjectOfType<PlayerMovement>().transform;

        //Invoke("ShootPlayer", 3);

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            playerPosition = player.transform;
        }

        Invoke(nameof(ShootPlayer), 3);
    }

    void ShootPlayer()
    {
        //Vector3 playerDirection = playerPosition.position - transform.position;

        //GameObject newBullet;

        //newBullet = Instantiate(enemyBullet, spawnBulletPoint.position, spawnBulletPoint.rotation);

        //newBullet.GetComponent<Rigidbody>().AddForce(playerDirection, ForceMode.Force);

        //Invoke("ShootPlayer", 3);

        if (playerPosition == null) return; // Evita errores si el jugador no está en la escena

        Vector3 playerDirection = playerPosition.position - transform.position;

        GameObject newBullet = Instantiate(enemyBullet, spawnBulletPoint.position, spawnBulletPoint.rotation);

        newBullet.GetComponent<Rigidbody>().AddForce(playerDirection * bulletVelocity, ForceMode.Force);

        Invoke(nameof(ShootPlayer), 3);
    }
}
