using System.Linq;
using UnityEngine.AI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionTakeTool : GAction
{
    [Header("Herramienta a coger (Tag)")]
    public string toolTag;

    [Header("Clave de recurso a comprobar (ej. collected_WOOD, collected_STONE)")]
    public string collectStateKey;

    private Vector3 originalPosition;
    private Transform originalParent;

    
    public GameObject noToolMessage;
    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        string haveKey = "hasTool_" + toolTag;
        preconditions[haveKey] = 0;

        effects[haveKey] = 1;

        targetTag = toolTag;
    }

    public override bool PrePerform()
    {

        if (G_Agent.inventory.HasTool(toolTag))
            return false;

        if (!string.IsNullOrEmpty(collectStateKey))
        {
            int have = G_Agent.beliefs.GetState(collectStateKey);
            var goal = G_Agent.goals.Keys.FirstOrDefault(g => g.sGoals.ContainsKey(collectStateKey));
            if (goal != null && have >= goal.sGoals[collectStateKey])
                return false;
        }

        var tools = GameObject.FindGameObjectsWithTag(toolTag);
        if (tools.Length == 0)             return false; 
/*
        {
            
            var go = Instantiate(noToolMessage, G_Agent.transform.position + Vector3.up * 2, Quaternion.identity);
            go.GetComponent<Canvas>().worldCamera = Camera.main;
            go.GetComponentInChildren<TextMeshPro>().text = "NO HAY HERRAMIENTAS DISPONIBLES";
            Destroy(go, 2f); // Destruye el texto tras 2 segundos
            
        }*/ 
        var closest = tools.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).First();

        target = closest;
        originalPosition = closest.transform.position;
        originalParent = closest.transform.parent;
        return true;
    }

    public override bool PostPerform()
    {
        if (target == null) return false;
        G_Agent.inventory.AddItem(target);
        G_Agent.beliefs.ModifyState("hasTool_" + toolTag, 1);

        var data = target.AddComponent<ToolData>();
        data.originalPosition = originalPosition;
        data.originalParent = originalParent;

        target.SetActive(false);
        return true;
    }
}