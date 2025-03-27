using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public LockedDoor doorToOpen;
    private static int keysCollected = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keysCollected++;

            if (keysCollected >= 3)
            {
                doorToOpen.isUnloocked = true;
                Debug.Log("La puerta ha sido desbloqueada.");
            }

            Destroy(gameObject);
        }
    }
}
