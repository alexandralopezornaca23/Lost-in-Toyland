using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    public Animator animator;

    public Transform[] destinations;
    private bool isRotating = false; // Para evitar que se mueva mientras rota

    public float distanceToFollowPath = 2f;

    private int i = 0; // posicion de inicio del array

    private bool isWaiting = false;
    private bool isAttacking = false;

    [Header("----------Follow Player?----------")]
    public bool followPlayer;

    private GameObject player;

    public float distanceToFollowPlayer = 10f;

    float distanceToPlayer;
    public float attackRange = 0.5f; // Rango de ataque del enemigo
    public float attackCooldown = 1f; // Tiempo entre ataques
    public int attackDamage = 10; // Daño que inflige el enemigo
    public Transform playerTransform; // Referencia al jugador
    private float lastAttackTime; // Control de tiempo para ataques

    public GameObject objectToActivate = null;


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

    void Update()
    {
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
        navMeshAgent.velocity = Vector3.zero; // Asegura que se detenga
        navMeshAgent.isStopped = true;

        // Girar el enemigo hacia el jugador antes de atacar
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0; // Evita que el enemigo se incline
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
        directionToDestination.y = 0; // Evitar inclinaciones

        // Si el enemigo aún no está mirando en la dirección correcta, gira antes de moverse
        if (Vector3.Dot(transform.forward, directionToDestination) < 0.99f)
        {
            StartCoroutine(RotateTowardsDestination(directionToDestination, () => {navMeshAgent.destination = destinations[i].position;}));
            return; // No mueve al enemigo hasta que termine de girar
        }

        navMeshAgent.destination = destinations[i].position;

        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToFollowPath)
        {
            StartCoroutine(WaitAtDestination(2f));
        }
    }

    // Corrutina para girar suavemente antes de moverse
    private IEnumerator RotateTowardsDestination(Vector3 targetDirection, System.Action onRotationComplete)
    {
        isRotating = true;
        navMeshAgent.isStopped = true; // Detener el movimiento mientras rota

        animator.SetBool("isRotating", true); // Activar animación de giro

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null;
        }

        //Asegurar que la rotación finaliza exactamente en el ángulo correcto
        transform.rotation = targetRotation;

        animator.SetBool("isRotating", false);
        navMeshAgent.isStopped = false;
        isRotating = false;

        onRotationComplete?.Invoke();
    }

    // Corrutina para esperar antes de ir al siguiente punto
    private IEnumerator WaitAtDestination(float waitTime)
    {
        isWaiting = true;
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        navMeshAgent.isStopped = false;

        // Mover al siguiente destino después de esperar
        i = (i + 1) % destinations.Length;

        // Asegurar que el nuevo destino se asigna correctamente
        navMeshAgent.destination = destinations[i].position;

        yield return null; // Esperar un frame antes de verificar el movimiento

        // Llamar nuevamente a EnemyPath para reanudar el recorrido
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

        Destroy(gameObject);
    }
}
