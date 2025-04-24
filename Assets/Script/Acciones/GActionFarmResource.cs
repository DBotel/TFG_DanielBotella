using UnityEngine.AI;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionFarmResource : GAction
{
    public string resourceTag;
    public TownResourcesTypes resourceType;
    public int amountNeeded = 5;

    public override void SetupAction()
    {
        Debug.Log($"SetupAction FarmResource: resourceTag={resourceTag}, preconditions={preconditions.Count}, effects={effects.Count}");
        preconditions.Clear();
        effects.Clear();

        // 1) Necesito la herramienta correcta
        string toolKey = "hasTool_" + (resourceType == TownResourcesTypes.WOOD ? "Axe" : "Pickaxe");
        preconditions[toolKey] = 1;

        // 2) Efecto en el plan: voy a recolectar X unidades
        string collectKey = "collected_" + resourceType;
        int val = 1;
        var nodes = GameObject.FindGameObjectsWithTag(resourceTag);
        if (nodes.Length > 0)
        {
            var ex = nodes[0].GetComponent<FarmResources>();
            if (ex != null) val = ex.value;
        }
        effects[collectKey] = val;
    }

    public override bool PrePerform()
    {
        Debug.Log("PrePerform FarmResource");
        var nodes = GameObject.FindGameObjectsWithTag(resourceTag);
        if (nodes.Length == 0) return false;
        target = nodes.OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).First();
        return true;
    }

    public override bool PostPerform()
    {
        var farm = target.GetComponent<FarmResources>();
        if (farm != null)
        {
            farm.FarmResource();
            G_Agent.beliefs.ModifyState("collected_" + resourceType, farm.value);
            GWorld.Instance.GetWorld().ModifyState(resourceType.ToString(), farm.value);
        }
        return true;
    }
}