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
        Debug.LogError("WANDER PREPERFORM");
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        target = new GameObject("WanderTarget");
        target.transform.position = navHit.position;
        return true;
    }

    public override bool PostPerform()
    {
        Debug.LogError("WANDER POSTPERFORM");

        if (target != null) Destroy(target);
        return true;
    }
}
