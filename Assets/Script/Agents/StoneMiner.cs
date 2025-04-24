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

    /// <summary>
    /// Configura este agente para minar piedra.
    /// </summary>
    public void ConfigureStoneMiner(int desiredAmount)
    {
        // Limpiar plan y metas anteriores
        agent.ResetPlan();
        agent.goals.Clear();

        // Ajuste de acciones
        takeTool.toolTag = "Pickaxe";
        takeTool.collectStateKey = "collected_STONE";
        farm.resourceTag = "Stone";          // Asegúrate de que tus objetos de piedra tengan este tag en Unity
        farm.resourceType = TownResourcesTypes.STONE;
        farm.amountNeeded = desiredAmount;

        // Al devolver, usar el mismo tag de herramienta
        ret.toolTag = takeTool.toolTag;

        // Meta: recolectar la piedra
        string collectKey = "collected_" + farm.resourceType;
        agent.goals.Add(new SubGoal(collectKey, desiredAmount, true), 3);

        // Meta: devolver la herramienta
        string returnKey = "returnedTool_" + takeTool.toolTag;
        agent.goals.Add(new SubGoal(returnKey, 1, true), 2);
    }
}
