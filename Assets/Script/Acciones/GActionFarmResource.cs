using UnityEngine.AI;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionFarmResource : GAction
{
    public string resourceTag;
    public TownResourcesTypes resourceType;
    public int amountNeeded = 5;
    private string tool;
    public override void SetupAction()
    {
        if(resourceTag=="")return;
        preconditions.Clear();
        effects.Clear();

        switch (resourceType)
        {
            case TownResourcesTypes.WOOD:
                tool = "Axe";
                break;
            case TownResourcesTypes.STONE:
                tool = "Pickaxe";
                break;
            case TownResourcesTypes.FOOD:
                tool = "Weapon";
                break;
            default:
                Debug.LogError("Unknown resource type: " + resourceType);
                return;
        }
  
        string toolKey = "hasTool_" + tool;
        preconditions[toolKey] = 1;

        string collectKey = "collected_" + resourceType;
        int val = 1;
        var nodes = GameObject.FindGameObjectsWithTag(resourceTag);
        if (nodes.Length > 0)
        {
            var ex = nodes[0].GetComponent<FarmResources>();
            if (ex != null) val = ex.value;
        }
        NPCRoleAssigner npcRoleAssigner = GetComponent<NPCRoleAssigner>();
        
        //effects[collectKey] = val;
        effects[collectKey] = npcRoleAssigner.desiredAmount;
    }

    public override bool PrePerform()
    {
        if (resourceTag == "") return false;
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
            NPCBasicNeeds npcNeeds = GetComponent<NPCBasicNeeds>();
            npcNeeds.money += 20f;
            farm.FarmResource();
            G_Agent.beliefs.ModifyState("collected_" + resourceType, farm.value);
            GWorld.Instance.GetWorld().ModifyState(resourceType.ToString(), farm.value);
        }
        return true;
    }
}