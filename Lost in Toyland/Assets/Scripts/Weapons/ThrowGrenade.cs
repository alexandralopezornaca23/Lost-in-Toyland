using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    public float throwForce = 500f;

    public GameObject grenadPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Throw();
        }
    }

    public void Throw()
    {
        GameObject newGrenade = Instantiate(grenadPrefab, transform.position, transform.rotation);

        newGrenade.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);
    }
}
