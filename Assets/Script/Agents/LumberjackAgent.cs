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

        // Ajuste de acciones
        takeTool.toolTag = "Axe";
        farm.resourceTag = "Tree";
        farm.resourceType = TownResourcesTypes.WOOD;
        farm.amountNeeded = desiredAmount;
        ret.toolTag = "Axe";

        // Meta: recolectar madera
        string collectKey = "collected_" + farm.resourceType;
        agent.goals.Add(new SubGoal(collectKey, desiredAmount, true), 3);

        // Meta: devolver herramienta (ahora basada en returnedTool)
        string returnKey = "returnedTool_" + takeTool.toolTag;
        agent.goals.Add(new SubGoal(returnKey, 1, true), 2);
    }
}
