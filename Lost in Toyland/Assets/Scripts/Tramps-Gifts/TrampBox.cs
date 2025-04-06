using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampBox : MonoBehaviour
{
    public GameObject explosionPrefab;

    public Animator animator;

    public void OpenBox()
    {
        StartCoroutine(AnimOpenBox());
        
        TrampBox trampBox = GetComponent<TrampBox>();
        trampBox.enabled = false;
        gameObject.tag = "Untagged";
    }

    IEnumerator AnimOpenBox()
    {
        animator.SetTrigger("Open");

        if (explosionPrefab != null)
        {
            SoundManager.Instance.PlaySound2D("Tramp");
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionPrefab, 2f);
        }

        GameManager.Instance.LoseHealth(30);
        yield return new WaitForSeconds(0.5f);
    }
}
