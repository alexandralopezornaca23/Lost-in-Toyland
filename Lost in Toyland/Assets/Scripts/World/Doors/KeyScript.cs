using UnityEngine;
using TMPro;

public class KeyScript : MonoBehaviour
{
    public LockedDoor doorToOpen;
    public GameObject textToActivate;
    public TMP_Text keyCounterText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddKey();

            if (keyCounterText != null)
            {
                keyCounterText.text = GameManager.Instance.keysCollected.ToString();
            }

            if (GameManager.Instance.keysCollected >= 3)
            {
                doorToOpen.isUnloocked = true;
                doorToOpen.ChangeLayerWhenUnlocked();
            }

            if (textToActivate != null)
            {
                textToActivate.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
