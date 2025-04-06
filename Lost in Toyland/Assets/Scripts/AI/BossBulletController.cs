using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletController : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;

    private Vector3 moveDirection;

    void Start()
    {
        CharacterController playerController = FindFirstObjectByType<CharacterController>();

        if (playerController != null)
        {
            Vector3 targetPos = playerController.transform.position;
            moveDirection = (targetPos - transform.position).normalized;
        }
        else
        {
            moveDirection = transform.forward;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
