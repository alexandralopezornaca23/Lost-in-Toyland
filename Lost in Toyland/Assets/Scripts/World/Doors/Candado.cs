using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candado : MonoBehaviour
{
    public string playerBulletLayer = "PlayerBullet"; // Capa con la que debe colisionar
    public string animationTrigger = "Hit"; // El nombre del trigger de animación

    public GameObject openDoor; // Prefab de la puerta abierta
    public Transform destroyDoor; // Objeto de la puerta cerrada a destruir
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerBulletLayer))
        {
            if (animator != null)
            {
                animator.SetTrigger(animationTrigger);
            }

           if (destroyDoor != null)
            {
                Vector3 position = destroyDoor.position;
                Quaternion rotation = destroyDoor.rotation;

                Destroy(destroyDoor.gameObject);

                if (openDoor != null)
                {
                    Instantiate(openDoor, position, rotation);
                }
            }

            Destroy(gameObject);
        }
    }
}
