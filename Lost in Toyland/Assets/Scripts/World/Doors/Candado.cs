using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candado : MonoBehaviour
{
    public string playerBulletLayer = "PlayerBullet"; // Capa con la que debe colisionar
    public string animationTrigger = "Hit"; // El nombre del trigger de animación
    private Animator animator;

    void Start()
    {
        // Obtener el componente Animator
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar si la colisión es con un objeto de la capa "PlayerBullet"
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerBulletLayer))
        {
            // Reproducir la animación
            if (animator != null)
            {
                animator.SetTrigger(animationTrigger);
            }

            // Destruir este objeto
            Destroy(gameObject);

            // Activar el script del objeto padre
            if (transform.parent != null)
            {
                var parentScript = transform.parent.GetComponent<SystemDoor>(); // Reemplaza "YourScript" con el script que deseas activar
                if (parentScript != null)
                {
                    parentScript.enabled = true; // Activar el script
                }
            }
        }
    }
}
