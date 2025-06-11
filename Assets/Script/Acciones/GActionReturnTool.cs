using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionReturnTool : GAction
{
    [Header("Herramienta a devolver (Tag)")]
    public string toolTag;


    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();
        string haveKey = "hasTool_" + toolTag;
        preconditions[haveKey] = 1;
        effects[haveKey] = -1;
        effects["returnedTool_" + toolTag] = 1;
    }

    public override bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        return conditions.ContainsKey("hasTool_" + toolTag) &&
               conditions["hasTool_" + toolTag] > 0;
    }

    public override bool PrePerform()
    {
        Debug.Log("PrePerform ReturnTool");
        var tool = G_Agent.inventory.items.Find(i => i.CompareTag(toolTag));
        if (tool == null) return false;
        var data = tool.GetComponent<ToolData>();
        if (data == null) return false;
        var dummy = new GameObject("ReturnPoint");
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
            G_Agent.beliefs.ModifyState("hasTool_" + toolTag, -1);
            G_Agent.beliefs.ModifyState("returnedTool_" + toolTag, 1);
            G_Agent.beliefs.SetState("returnedTool_" + toolTag, 1);
            Debug.Log($"[ReturnTool] Devolved {toolTag}");
            Debug.LogError("[ReturnTool] Devuelta " + toolTag + ": hasTool_" + toolTag +
                      " = " + G_Agent.beliefs.GetState("hasTool_" + toolTag) +
                      ", returnedTool_" + toolTag + " = " + G_Agent.beliefs.GetState("returnedTool_" + toolTag));
            //GetComponent<NPCRoleAssigner>().ChangeRole();
            G_Agent.ResetPlan();

        }
        if (target != null) Destroy(target);
        target = null;
        return true;
    }
}