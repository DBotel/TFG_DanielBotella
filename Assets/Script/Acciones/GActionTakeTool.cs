using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionTakeTool : GAction
{
    [Header("Herramienta a coger (Tag)")]
    public string toolTag;

    [Header("Clave de recurso a comprobar (ej. collected_WOOD, collected_STONE)")]
    public string collectStateKey; // Ahora parametrizado

    private Vector3 originalPosition;
    private Transform originalParent;

    private void Start()
    {
        // Efecto de tomar la herramienta
        string effectKey = "hasTool_" + toolTag;
        if (!effects.ContainsKey(effectKey))
            effects.Add(effectKey, 1);
    }

    public override bool PrePerform()
    {
        // No tomar herramienta si ya se tiene
        if (G_Agent.inventory.HasTool(toolTag))
        {
            Debug.Log($"[TakeTool] Ya tengo la herramienta {toolTag}, ignorando acción.");
            return false;
        }

        // No tomar herramienta si ya se completó la meta de recolección para este recurso
        if (!string.IsNullOrEmpty(collectStateKey))
        {
            int collectedSoFar = G_Agent.beliefs.GetState(collectStateKey);
            var goal = G_Agent.goals.Keys.FirstOrDefault(g => g.sGoals.ContainsKey(collectStateKey));
            if (goal != null && collectedSoFar >= goal.sGoals[collectStateKey])
            {
                Debug.Log($"[TakeTool] Ya recolectado suficiente ({collectStateKey}) = {collectedSoFar}, ignorando herramienta.");
                return false;
            }
        }

        // Buscar herramienta en la escena
        var tools = GameObject.FindGameObjectsWithTag(toolTag);
        if (tools.Length == 0)
            return false;

        // Seleccionar la más cercana
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
            // Añadir al inventario y actualizar creencias
            G_Agent.inventory.AddItem(target);
            G_Agent.beliefs.ModifyState("hasTool_" + toolTag, 1);

            // Guardar datos para devolver luego
            var data = target.AddComponent<ToolData>();
            data.originalPosition = originalPosition;
            data.originalParent = originalParent;

            target.SetActive(false);
        }
        return true;
    }
}
