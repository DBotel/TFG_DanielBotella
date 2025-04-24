using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionReturnTool : GAction
{
    [Header("Herramienta a devolver (Tag)")]
    public string toolTag;

    private void Start()
    {
        // Precondición: debo tener la herramienta para devolverla
        string toolKey = "hasTool_" + toolTag;
        if (!preconditions.ContainsKey(toolKey))
            preconditions.Add(toolKey, 1);

        // Efecto: se quita la herramienta
        if (!effects.ContainsKey(toolKey))
            effects.Add(toolKey, -1);

        // Efecto adicional: marca que la herramienta fue devuelta
        string returnedKey = "returnedTool_" + toolTag;
        if (!effects.ContainsKey(returnedKey))
            effects.Add(returnedKey, 1);

        Debug.Log($"[ReturnTool] Precondicion: {toolKey} >= 1, Efectos: {toolKey} -=1, {returnedKey} +=1");
    }

    public override bool PrePerform()
    {
        var tool = G_Agent.inventory.items.Find(i => i.CompareTag(toolTag));
        if (tool == null) return false;
        var data = tool.GetComponent<ToolData>();
        if (data == null) return false;

        // Creamos un dummy para moverse al punto original
        GameObject dummy = new GameObject("ReturnPoint");
        dummy.transform.position = data.originalPosition;
        target = dummy;
        return true;
    }

    public override bool PostPerform()
    {
        var tool = G_Agent.inventory.items.Find(i => i.CompareTag(toolTag));
        if (tool != null)
        {
            G_Agent.inventory.RemoveItem(tool);
            tool.SetActive(true);
            var data = tool.GetComponent<ToolData>();
            tool.transform.position = data.originalPosition;
            tool.transform.parent = data.originalParent;

            // Actualizamos creencias
            G_Agent.beliefs.ModifyState("hasTool_" + toolTag, -1);
            G_Agent.beliefs.ModifyState("returnedTool_" + toolTag, 1);

            Debug.Log($"[ReturnTool] Herramienta devuelta. hasTool_{toolTag} = " +
                      G_Agent.beliefs.GetState("hasTool_" + toolTag) +
                      ", returnedTool_{toolTag} = " +
                      G_Agent.beliefs.GetState("returnedTool_" + toolTag));
        }

        if (target != null)
            Destroy(target);
        return true;
    }
}

