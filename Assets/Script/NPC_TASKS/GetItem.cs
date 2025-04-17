using UnityEngine;

public class GetItem : GAction
{
    public string itemTag = "Tool";
    private GameObject targetItem;
    private GAgent agent;

    public override bool PrePerform()
    {
        agent = GetComponent<GAgent>();

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
        agent.inventory.AddItem(targetItem);
        agent.beliefs.ModifyState("Has" + itemTag, 1);
        targetItem.SetActive(false);

        Debug.Log($"{gameObject.name} ha recogido un {itemTag}");
        return true;
    }

    void Start()
    {
        effects.Add("Has" + itemTag, 1);
    }
}