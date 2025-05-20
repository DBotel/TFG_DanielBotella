using UnityEngine;

public class StoneMiner : MonoBehaviour
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

    public void ConfigureStoneMiner(int desiredAmount)
    {
        agent.ResetPlan();
        agent.goals.Clear();

        takeTool.toolTag = "Pickaxe";
        takeTool.collectStateKey = "collected_STONE";
        farm.resourceTag = "Stone";          
        farm.resourceType = TownResourcesTypes.STONE;
        farm.amountNeeded = desiredAmount;

        ret.toolTag = takeTool.toolTag;

        string collectKey = "collected_" + farm.resourceType;
        agent.goals.Add(new SubGoal(collectKey, desiredAmount, true), 3);

        string returnKey = "returnedTool_" + takeTool.toolTag;
        agent.goals.Add(new SubGoal(returnKey, 1, true), 2);
    }
}
