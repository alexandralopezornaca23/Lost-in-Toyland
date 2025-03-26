using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowGrenade : MonoBehaviour
{
    public float throwForce = 500f;

    public GameObject grenadPrefab;

    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && GameManager.Instance.grenadeAmmo > 0)
        {
            Throw();
        }
    }

    public void Throw()
    {
        GameObject newGrenade = Instantiate(grenadPrefab, transform.position, transform.rotation);

        newGrenade.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);

        GameManager.Instance.grenadeAmmo--;
    }
}
