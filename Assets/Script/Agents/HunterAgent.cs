using UnityEngine;

public class HunterAgent : MonoBehaviour
{
    private GAgent agent;
    private GActionTakeTool takeTool;
    private GActionHuntAnimal huntAnimal;
    private GActionFarmResource farm;
    private GActionReturnTool ret;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        takeTool = GetComponent<GActionTakeTool>();
       // huntAnimal = GetComponent<GActionHuntAnimal>();
        farm = GetComponent<GActionFarmResource>();
        ret = GetComponent<GActionReturnTool>();
    }

    public void ConfigureHunter(int desiredAmount)
    {
        agent.ResetPlan();
        agent.goals.Clear();

        takeTool.toolTag = "Weapon";
        takeTool.collectStateKey = "collected_MEAT";
        farm.resourceTag = "Animal";
        farm.resourceType = TownResourcesTypes.FOOD;
        farm.amountNeeded = desiredAmount;
        ret.toolTag = "Weapon";
        

        string collectKey = "collected_" + farm.resourceType;
        agent.goals.Add(new SubGoal(collectKey, desiredAmount, true), 3);

        string returnKey = "returnedTool_" + takeTool.toolTag;
        agent.goals.Add(new SubGoal(returnKey, 1, true), 2);
    }
}
