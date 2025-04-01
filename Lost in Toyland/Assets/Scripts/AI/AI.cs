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

    float distanceToPlayer;
    public float attackRange = 0.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public Transform playerTransform;
    private float lastAttackTime;

    public GameObject objectToActivate = null;

    [SerializeField]
    private GameObject freezeEffectPrefab;
    public bool isFrozen = false;
    private float frozenTime = 3f;

    [SerializeField]
    private GameObject frozenOrbePrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
        }

        if (destinations == null || destinations.Length == 0)
        {
            enabled = false;
            return;
        }

        navMeshAgent.destination = destinations[i].transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            playerTransform = player.transform;
        }

        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (isFrozen) return;
        if (playerTransform == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        animator.SetBool("isWalking", speed > 0);
        animator.SetBool("isWalking", speed > 0 && !isRotating);
        animator.SetBool("isIdle", speed == 0 && !isRotating);

        if (isAttacking) return;

        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (distanceToPlayer <= distanceToFollowPlayer && followPlayer)
        {
            FollowPlayer();
        }
        else
        {
            EnemyPath();
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (distanceToPlayer <= attackRange)
        {
            GameManager.Instance.LoseHealth(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        navMeshAgent.isStopped = false;
    }

    public void EnemyPath()
    {
        if (isWaiting || isRotating) return;

        Vector3 directionToDestination = (destinations[i].position - transform.position).normalized;
        directionToDestination.y = 0;

        if (Vector3.Dot(transform.forward, directionToDestination) < 0.99f)
        {
            StartCoroutine(RotateTowardsDestination(directionToDestination, () => {navMeshAgent.destination = destinations[i].position;}));
            return;
        }

        navMeshAgent.destination = destinations[i].position;

        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToFollowPath)
        {
            StartCoroutine(WaitAtDestination(2f));
        }
    }

    private IEnumerator RotateTowardsDestination(Vector3 targetDirection, System.Action onRotationComplete)
    {
        isRotating = true;
        navMeshAgent.isStopped = true;

        animator.SetBool("isRotating", true);

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null;
        }

        transform.rotation = targetRotation;

        animator.SetBool("isRotating", false);
        navMeshAgent.isStopped = false;
        isRotating = false;

        onRotationComplete?.Invoke();
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

        yield return null; 
        
        EnemyPath();
    }

    public void FollowPlayer()
    {
        if (playerTransform != null)
        {
            navMeshAgent.destination = playerTransform.position;
        }
    }

    public void GrenadeImpact()
    {
        Destroy(gameObject);
    }

    public void Death()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (frozenOrbePrefab != null)
        {
            Instantiate(frozenOrbePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void Frozen()
    {
        GameObject freezeEffect = Instantiate(freezeEffectPrefab, transform.position, Quaternion.identity);
        Destroy(freezeEffect, frozenTime);
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine());
        }
    }

    private IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        followPlayer = false;

        navMeshAgent.enabled = false;

        animator.enabled = false;

        if (rb != null)
        {
            rb.useGravity = true;  
            rb.isKinematic = true; 
        }

        animator.SetBool("isIdle", true);

        yield return new WaitForSeconds(frozenTime);

        if (rb != null)
        {
            rb.isKinematic = false;  
            rb.useGravity = true;    
        }

        navMeshAgent.enabled = true;
        animator.enabled = true;
        followPlayer = true;
        isFrozen = false;
    }
}
