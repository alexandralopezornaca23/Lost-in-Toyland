using System.Collections;
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
    private bool playerInRange = false;  // Saber si el jugador está dentro del trigger
    private Collider playerCollider;     // Guardar referencia al jugador

    [Header("----------Follow Player?----------")]
    public bool followPlayer;

    private GameObject player;

    private float distanceToPlayer;
    public float distanceToFollowPlayer = 10f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (destinations == null || destinations.Length == 0)
        {
            transform.gameObject.GetComponent<AI>().enabled = false;
        }
        else
        {
            navMeshAgent.destination = destinations[i].transform.position;
            player = Object.FindFirstObjectByType<PlayerController>().gameObject;
        }

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (navMeshAgent.velocity.magnitude > 0.1f) // Si el enemigo se mueve
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else // Si el enemigo está quieto
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }

        if (distanceToPlayer <= distanceToFollowPlayer && followPlayer)
        {
            FollowPlayer();
        }
        else
        {
            EnemyPath();
        }

        // Verificar si el enemigo está cerca del jugador para iniciar el ataque
        if (playerInRange && !isAttacking)  // Puedes ajustar la distancia de ataque
        {
            StartAttack();
        }

        //GrenadeImpact();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerCollider = other;  // Guardamos la referencia del jugador
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerCollider = null;  // Quitamos la referencia del jugador
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        navMeshAgent.isStopped = true;
        animator.SetTrigger("Attack");  // Activar la animación de ataque

        // Aplicar daño solo si el jugador sigue dentro del trigger
        if (playerInRange && playerCollider != null)
        {
            GameManager.Instance.LoseHealth(5);
        }

        // Llamamos a una función después de un breve retraso (duración del ataque)
        Invoke("ApplyDamage", 2.667f); // Ajusta el tiempo según la animación del ataque
        Invoke("FinishAttack", 2.667f);
    }

    void ApplyDamage()
    {
        if (playerInRange && playerCollider != null) // Solo si el jugador sigue en el rango
        {
            GameManager.Instance.LoseHealth(5);
        }
    }

    void FinishAttack()
    {
        isAttacking = false;
        navMeshAgent.isStopped = false;

        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
    }

    public void EnemyPath()
    {
        if (isWaiting) return;

        navMeshAgent.destination = destinations[i].position;

        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToFollowPath)
        {
            StartCoroutine(WaitAtDestination(2f)); // Espera 2 segundos al llegar a la destinación

            if (destinations[i] != destinations[destinations.Length -1])
            {
                i++;
            }
            else
            {
                i = 0;
            }
        }
    }

    public void FollowPlayer()
    {
        if (!isAttacking) // No seguir al jugador mientras ataca
        {
            navMeshAgent.destination = player.transform.position;
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
}
