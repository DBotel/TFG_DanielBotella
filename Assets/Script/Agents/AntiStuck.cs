using UnityEngine;
using UnityEngine.AI;

public class AntiStuck : MonoBehaviour
{
    public float checkInterval = 1f;
    public float movementThreshold = 0.2f;
    public float stuckTime = 3f;
    public float unstuckRadius = 2f;

    private NavMeshAgent agent;
    private Vector3 lastPosition;
    private float checkTimer;
    private float stuckTimer;
    private GActionBuild actionBuild;
    private NPCRoleAssigner roleAssigner;
    void Start()
    {
        actionBuild = GetComponent<GActionBuild>();
        agent        = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        checkTimer   = 0f;
        stuckTimer   = 0f;
        roleAssigner = GetComponent<NPCRoleAssigner>();
    }

    void Update()
    {
        if(actionBuild.nowBuilding)return;
        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return;

        float dist = Vector3.Distance(transform.position, lastPosition);
        if (dist < movementThreshold)
        {
            stuckTimer += checkTimer;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckTime)
        {
            TryUnstuck();
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
        checkTimer   = 0f;
    }

    private void TryUnstuck()
    {
        Vector3 randomDir = Random.insideUnitSphere * unstuckRadius;
        randomDir += transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, unstuckRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            roleAssigner.ChangeRole();
        }
        else
        {
            Vector3 brute = transform.position + Random.insideUnitSphere * (unstuckRadius * 0.5f);
            agent.Warp(brute);
        }
    }
}
