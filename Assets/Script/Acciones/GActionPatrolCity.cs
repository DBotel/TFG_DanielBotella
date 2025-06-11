using System;
using System.Collections.Generic;
using UnityEngine;

public class GActionPatrolCity : GAction
{
    public List<Transform> patrolPoints;
    private int currentPoint = 0;

    private void Start()
    {
        patrolPoints = FindObjectOfType<TownHall>().patrolPoints; 

    }

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();
        preconditions["hasTool_Shield"] = 1;
        effects["defend"] = 1;
        effects["collected_DEFEND"] = 1;
    }

    public override bool PrePerform()
    {
        if (patrolPoints == null || patrolPoints.Count == 0){Debug.Log("NO HAY PATROL POINTS"); return false;}
        target = patrolPoints[currentPoint].gameObject;
        Debug.Log($"PrePerform PatrolCity: Moving to patrol point {currentPoint + 1}/{patrolPoints.Count}");
        return true;
    }

    public override bool PostPerform()
    {
        currentPoint = (currentPoint + 1) % patrolPoints.Count;
        return true;
    }
}
