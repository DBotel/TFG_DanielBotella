using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GActionPatrolCity : GAction
{
    public List<Transform> patrolPoints;
    private int currentPoint = 0;

    void Start()
    {
        patrolPoints = FindObjectOfType<TownHall>().patrolPoints;
    }

    public override void SetupAction()
    {
        Debug.Log("GActionPatrolCity: SetupAction");
        preconditions.Clear();
        effects.Clear();
        // Requiere escudo
        preconditions["hasTool_Shield"] = 1;
        effects["defend"] = patrolPoints.Count;
    }

    public override bool PrePerform()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("GActionPatrolCity: no hay puntos de patrulla.");
            return false;
        }

        target = patrolPoints[currentPoint].gameObject;
        agent.GetComponent<NavMeshAgent>()
            .SetDestination(target.transform.position);
        StartCoroutine(WaitToArrive());
        Debug.Log("GActionPatrolCity: PrePerform - Patrullando a " + target.name);
        return true;
    }


    IEnumerator WaitToArrive()
    {
        // Espera hasta que el agente llegue al destino
        yield return new WaitUntil(() => Vector3.Distance(agent.transform.position, target.transform.position) < 1f);
        StartCoroutine(WaitALit());
    }

    IEnumerator WaitALit()
    {
        int waitTime = Random.Range(1, 4);
        yield return new WaitForSeconds(waitTime); 
        PostPerform();
    }

    public override bool PostPerform()
    {
        Debug.Log("GActionPatrolCity: PostPerform - Llegado a " + target.name);
        // Avanzamos al siguiente punto
        currentPoint = (currentPoint + 1);
        G_Agent.beliefs.ModifyState("defend", 1);
        if(currentPoint==patrolPoints.Count)
        {
            G_Agent.beliefs.SetState("defend", 0);
            currentPoint = 0;
            NPCRoleAssigner npcRoleAssigner = GetComponent<NPCRoleAssigner>();
            npcRoleAssigner.ChangeRole();
  
        }
        // Actualizamos creencia
        return true;
    }
}