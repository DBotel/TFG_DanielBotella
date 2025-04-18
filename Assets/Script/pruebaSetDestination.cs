using UnityEngine;
using UnityEngine.AI;

public class pruebaSetDestination : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform destination;
    [ContextMenu("SetDestination")]
    public void SetDestination()
    {
        agent.SetDestination(destination.position);
    }
}
