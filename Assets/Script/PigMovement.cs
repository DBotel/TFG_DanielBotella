using UnityEngine;
using UnityEngine.AI;

public class PigMovement : MonoBehaviour
{
    [Tooltip("Radio en el que el cerdo buscará puntos aleatorios")]
    public float wanderRadius = 10f;
    [Tooltip("Tiempo (s) entre cada nuevo destino")]
    public float wanderTimer = 5f;
    
    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;  // forzar primera búsqueda inmediata
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavMeshPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0f;
        }
    }

    /// <summary>
    /// Devuelve una posición aleatoria sobre el NavMesh
    /// dentro de un radio alrededor de un punto dado.
    /// </summary>
    private Vector3 RandomNavMeshPoint(Vector3 center, float radius)
    {
        // Generar un punto aleatorio en esfera
        Vector3 randomPos = center + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        // Muestrear la posición más cercana en el NavMesh
        if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            return hit.position;

        // Si falla, devuelve la posición original
        return center;
    }
}
