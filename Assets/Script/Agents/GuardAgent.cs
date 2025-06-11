using UnityEngine;

public class GuardAgent : MonoBehaviour
{
    private GAgent agent;
    private GActionTakeTool takeTool;
    private GActionPatrolCity patrol;
    private GActionFarmResource farm;
    private GActionReturnTool ret;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        takeTool = GetComponent<GActionTakeTool>();
        patrol = GetComponent<GActionPatrolCity>();
        farm = GetComponent<GActionFarmResource>();
        ret = GetComponent<GActionReturnTool>();
    }

    public void ConfigureGuard()
    {
        agent.ResetPlan();
        agent.goals.Clear();

        takeTool.toolTag = "Shield";
        takeTool.collectStateKey = "collected_DEFEND";
        int desiredAmount = patrol.patrolPoints.Count; 

        ret.toolTag = "Shield";

        agent.goals.Add(new SubGoal("collected_DEFEND", 1, true), 3);
        agent.goals.Add(new SubGoal("returnedTool_Shield", 1, true), 2);
        
       
    }
}
