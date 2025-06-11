using UnityEngine;
using UnityEngine.AI;

public class GActionWander : GAction
{
    public float wanderRadius = 10f;

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        effects["isWandering"] = 1;
    }

    public override bool PrePerform()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        if(navHit.position == Vector3.positiveInfinity || navHit.position == Vector3.negativeInfinity)
        {
            target = new GameObject("WanderTarget");
            target.transform.position=agent.transform.position;
            return true;
        }
        else
        {
            target = new GameObject("WanderTarget");
            target.transform.position = navHit.position;
            return true;
        }
        
    }

    public override bool PostPerform()
    {

        if (target != null) Destroy(target);
        return true;
    }
}
