using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GameObject destination1;
    public GameObject destination2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent.destination = destination1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //navMeshAgent.destination = destination1.transform.position; //y creando una animacion en loop en el destination1 para que siga siempre los puntos

        float distance = Vector3.Distance(transform.position, destination1.transform.position);

        if (distance < 2)
        {
            navMeshAgent.destination = destination2.transform.position;
        }
    }
}
