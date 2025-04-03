using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    private Rigidbody rb;

    public Animator animator;

    public Transform[] destinations;
    private bool isRotating = false;

    public float distanceToFollowPath = 2f;

    private int i = 0;

    private bool isWaiting = false;
    private bool isAttacking = false;

    [Header("----------Follow Player?----------")]
    public bool followPlayer;

    private GameObject player;

    public float distanceToFollowPlayer = 10f;

    public float attackRange = 0.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public Transform playerTransform;
    private float lastAttackTime;
    private bool playerInAttackZone = false;

    public GameObject objectToActivate = null;

    [SerializeField]
    private GameObject freezeEffectPrefab;
    public bool isFrozen = false;
    private float frozenTime = 3f;

    [SerializeField]
    private GameObject frozenOrbePrefab;

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = true;

        if (destinations == null || destinations.Length == 0)
        {
            enabled = false;
            return;
        }

        navMeshAgent.destination = destinations[i].transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerTransform = player.transform;

        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (isDead || isFrozen || playerTransform == null) return;

        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("isWalking", speed > 0 && !isRotating);
        animator.SetBool("isIdle", speed == 0 && !isRotating);

        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (playerInAttackZone && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (!playerInAttackZone && followPlayer && distanceToPlayer <= distanceToFollowPlayer)
        {
            FollowPlayer();
        }
        else if (!isWaiting && isDead == false)
        {
            EnemyPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInAttackZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInAttackZone = false;
    }

    private IEnumerator AttackPlayer()
    {
        if (!playerInAttackZone) yield break;

        isAttacking = true;
        navMeshAgent.isStopped = true;

        animator.SetTrigger("Attack");
        SoundManager.Instance.PlaySound2D("EnemyAttack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (playerInAttackZone)
        {
            GameManager.Instance.LoseHealth(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        navMeshAgent.isStopped = false;

        if (playerInAttackZone)
        {
            yield return StartCoroutine(SmoothLookAt(playerTransform.position));
            FollowPlayer();
        }
        else
        {
            yield return StartCoroutine(SmoothLookAt(destinations[i].position));
            EnemyPath();
        }
    }

    public void FollowPlayer()
    {
        if (playerTransform != null)
        {
            navMeshAgent.destination = playerTransform.position;
            StartCoroutine(SmoothLookAt(playerTransform.position));
        }
        else
        {
            EnemyPath();
        }
    }

    private IEnumerator SmoothLookAt(Vector3 targetPosition)
    {
        isRotating = true;
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }

    public void EnemyPath()
    {
        navMeshAgent.destination = destinations[i].position;
        StartCoroutine(SmoothLookAt(destinations[i].position));

        if (!isWaiting && Vector3.Distance(transform.position, destinations[i].position) <= distanceToFollowPath)
        {
            StartCoroutine(WaitAtDestination(2f));
        }
    }

    private IEnumerator WaitAtDestination(float waitTime)
    {
        isWaiting = true;
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        navMeshAgent.isStopped = false;

        i = (i + 1) % destinations.Length;
        navMeshAgent.destination = destinations[i].position;

        if (!followPlayer)
        {
            EnemyPath();
        }
    }

    public void GrenadeImpact()
    {
        Destroy(gameObject);
    }

    public void Death()
    {
        if (objectToActivate != null) objectToActivate.SetActive(true);

        StartCoroutine(TimeDeathAnimation());
        SoundManager.Instance.PlaySound2D("EnemyHit");
    }

    private IEnumerator TimeDeathAnimation()
    {
        isDead = true;
        followPlayer = false;
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        navMeshAgent.isStopped = true;
        animator.SetTrigger("isDeath");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 3);

        Destroy(gameObject);
    }

    public void Frozen()
    {
        if (!isFrozen) StartCoroutine(FreezeCoroutine());
    }

    private IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        followPlayer = false;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        navMeshAgent.isStopped = true;
        animator.enabled = false;

        Transform freezePoint = transform.Find("FreezePoint");
        Vector3 effectPosition = freezePoint != null ? freezePoint.position : transform.position;
        GameObject freezeEffect = Instantiate(freezeEffectPrefab, effectPosition, Quaternion.identity, freezePoint);
        Destroy(freezeEffect, frozenTime);

        yield return new WaitForSeconds(frozenTime);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        navMeshAgent.isStopped = false;
        animator.enabled = true;
        followPlayer = true;
        isFrozen = false;
    }
}
