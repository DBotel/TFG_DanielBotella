using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionTakeTool : GAction
{
    [Header("Herramienta a coger (Tag)")]
    public string toolTag;

    private Vector3 originalPosition;
    private Transform originalParent;

    private void Start()
    {
        string effectKey = "hasTool_" + toolTag;
        if (!effects.ContainsKey(effectKey))
            effects.Add(effectKey, 1);
    }

    public override bool PrePerform()
    {
        // No tomar herramienta si ya se tiene
        if (G_Agent.inventory.HasTool(toolTag))
        {
            Debug.Log("[TakeTool] Ya tengo la herramienta " + toolTag + ", ignorando acción.");
            return false;
        }

        // No tomar herramienta si ya se completó la meta de recolección
        string collectedKey = "collected_WOOD"; // Puedes parametrizar esto si es necesario
        int collectedSoFar = G_Agent.beliefs.GetState(collectedKey);
        if (collectedSoFar >= G_Agent.goals.Keys.FirstOrDefault()?.sGoals[collectedKey])
        {
            Debug.Log("[TakeTool] Ya se recogió suficiente recurso, no tomar herramienta.");
            return false;
        }

        var tools = GameObject.FindGameObjectsWithTag(toolTag);
        if (tools.Length == 0) return false;

        var closest = tools.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).First();
        target = closest;

        originalPosition = closest.transform.position;
        originalParent = closest.transform.parent;

        return true;
    }

    public override bool PostPerform()
    {
        if (target != null)
        {
            G_Agent.inventory.AddItem(target);
            G_Agent.beliefs.ModifyState("hasTool_" + toolTag, 1);

            var data = target.AddComponent<ToolData>();
            data.originalPosition = originalPosition;
            data.originalParent = originalParent;

            target.SetActive(false);
        }

        return true;
    }
}
