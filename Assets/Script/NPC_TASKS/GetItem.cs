using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetItem : GAction
{
    public string itemTag = "Tool";
    private GameObject targetItem;
    public Transform pruebaDestination;

    [ContextMenu("SetDestination")]
    public void SetDestination()
    {
        agent.ResetPath();
        if (agent.path != null) Debug.LogError("Hay un Path" + agent.path);
        print("SetDestination");

        agent.SetDestination(pruebaDestination.transform.position);
    }
    public override bool PrePerform()
    {

        // Buscar el objeto más cercano con el tag deseado
        GameObject[] items = GameObject.FindGameObjectsWithTag(itemTag);
        float closestDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject item in items)
        {
            if (!item.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = item;
            }
        }

        if (closest == null)
        {
            Debug.Log($"{gameObject.name} no encontró ningún {itemTag}");
            return false;
        }

        targetItem = closest;
        target = targetItem;
        return true;
    }
   
    public override bool PostPerform()
    {
        G_Agent.inventory.AddItem(targetItem);
        G_Agent.GetComponent<NPC>().hasTool = true;
        G_Agent.beliefs.ModifyState("Has" + itemTag, 1);
        targetItem.SetActive(false);

        Debug.Log($"{gameObject.name} ha recogido un {itemTag}");

       
        G_Agent.actions.Remove(this);
        Destroy(this);

        return true;
    }

    void Start()
    {
        effects.Add("Has" + itemTag, 1);
    }
}