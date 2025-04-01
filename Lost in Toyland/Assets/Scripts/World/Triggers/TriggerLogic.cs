using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLogic : MonoBehaviour
{
    public GameObject enemyToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Comparación corregida
        {
            if (enemyToEnable != null)
            {
                enemyToEnable.SetActive(true); // Activa el enemigo
            }
            Destroy(gameObject); // Destruye este objeto
        }
    }
}
