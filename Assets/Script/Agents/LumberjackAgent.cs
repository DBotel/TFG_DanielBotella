using UnityEngine;

public class LumberjackAgent : MonoBehaviour
{
    private GAgent agent;
    private GActionTakeTool takeTool;
    private GActionFarmResource farm;
    private GActionReturnTool ret;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        takeTool = GetComponent<GActionTakeTool>();
        farm = GetComponent<GActionFarmResource>();
        ret = GetComponent<GActionReturnTool>();
    }

    public void ConfigureLumberjack(int desiredAmount)
    {
        agent.ResetPlan();
        agent.goals.Clear();
        
        takeTool.toolTag = "Axe";
        takeTool.collectStateKey = "collected_WOOD";
        farm.resourceTag = "Tree";
        farm.resourceType = TownResourcesTypes.WOOD;
        farm.amountNeeded = desiredAmount;
        ret.toolTag = "Axe";
        
        string collectKey = "collected_" + farm.resourceType;
        agent.goals.Add(new SubGoal(collectKey, desiredAmount, true), 3);
        
        string returnKey = "returnedTool_" + takeTool.toolTag;
        agent.goals.Add(new SubGoal(returnKey, 1, true), 2);
    }
}
