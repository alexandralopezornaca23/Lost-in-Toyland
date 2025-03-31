using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    public Animator animator;

    public Transform[] destinations;

    public float distanceToFollowPath = 2f;

    private int i = 0; // posicion de inicio del array

    private bool isWaiting = false;
    private bool isAttacking = false;

    [Header("----------Follow Player?----------")]
    public bool followPlayer;

    private GameObject player;

    public float distanceToFollowPlayer = 10f;

    public float attackRange = 2f; // Rango de ataque del enemigo
    public float attackCooldown = 1f; // Tiempo entre ataques
    public int attackDamage = 10; // Daño que inflige el enemigo
    public Transform playerTransform; // Referencia al jugador
    private float lastAttackTime; // Control de tiempo para ataques


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        animator.SetBool("isWalking", speed > 0);
        animator.SetBool("isIdle", speed == 0);

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
        navMeshAgent.velocity = Vector3.zero; // Asegura que se detenga
        navMeshAgent.isStopped = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        GameManager.Instance.LoseHealth(attackDamage);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        navMeshAgent.isStopped = false;
    }

    public void EnemyPath()
    {
        if (isWaiting) return;

        navMeshAgent.destination = destinations[i].position;

        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToFollowPath)
        {
            StartCoroutine(WaitAtDestination(2f));

            if (i < destinations.Length - 1) i++;
            else i = 0;
        }
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

    private IEnumerator WaitAtDestination(float waitTime)
    {
        isWaiting = true; // Activa el estado de espera
        navMeshAgent.isStopped = true; // Detiene al agente

        yield return new WaitForSeconds(waitTime); // Espera los segundos indicados

        navMeshAgent.isStopped = false; // Reanuda el movimiento
        isWaiting = false; // Vuelve a permitir que el enemigo se mueva
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
