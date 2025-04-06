using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : MonoBehaviour
{
    public GameObject[] giftObjects;

    public Transform dropPoint;

    public GameObject explosionPrefab;

    public Animator animator;

    public void OpenBox()
    {
        StartCoroutine(AnimOpenBox());        
        GiftBox giftBox = GetComponent<GiftBox>();
        giftBox.enabled = false;
        gameObject.tag = "Untagged";
    }
    
    IEnumerator AnimOpenBox()
    {
        animator.SetTrigger("Open");

        if (explosionPrefab != null)
        {
            SoundManager.Instance.PlaySound2D("Gift");
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionPrefab, 2f);
        }
        yield return new WaitForSeconds(0.5f);

        if (giftObjects.Length > 0)
        {
            GameObject objetoAleatorio = giftObjects[Random.Range(0, giftObjects.Length)];
            Instantiate(objetoAleatorio, dropPoint.position, Quaternion.identity);
        }
    }
}
