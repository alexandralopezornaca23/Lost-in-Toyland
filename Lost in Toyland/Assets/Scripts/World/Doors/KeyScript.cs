using UnityEngine;
using TMPro;

public class KeyScript : MonoBehaviour
{
    public LockedDoor doorToOpen;
    public GameObject textToActivate;
    public TMP_Text keyCounterText;
    private static int keysCollected = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keysCollected++;

            if (keyCounterText != null)
            {
                keyCounterText.text = keysCollected.ToString();
            }

            if (keysCollected >= 3)
            {
                doorToOpen.isUnloocked = true;
            }

            if (textToActivate != null)
            {
                textToActivate.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
