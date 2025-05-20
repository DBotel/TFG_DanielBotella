 using UnityEngine;
using System.Collections.Generic;

public class CraftItem : GAction
{
    [Header("Configurar crafting")]
    public string recipeID;
    private CraftableItemSO recipe;
    private ToolCrafter crafter;
    bool canCraft = true;

    void Start()
    {
        if (preconditions == null) preconditions = new Dictionary<string, int>();
        if (effects == null) effects = new Dictionary<string, int>();

        ToolCrafter[] stations = FindObjectsOfType<ToolCrafter>();
        float minDist = Mathf.Infinity;
        foreach (var cs in stations)
        {
            var r = cs.GetRecipe(recipeID);
            if (r == null) continue;
            float d = Vector3.Distance(transform.position, cs.transform.position);
            if (d < minDist)
            {
                minDist = d;
                crafter = cs;
                recipe = r;
            }
        }

        if (recipe == null)
        {
            Debug.LogError($"{name}: no se encontr� receta '{recipeID}' ni estaci�n.");
            return;
        }

        preconditions.Clear();
        foreach (var cost in recipe.costs)
        {
            preconditions[cost.type.ToString()] = cost.amount;
        }

        effects.Clear();
        effects.Add(recipeID + "Crafted", 1);

        if (crafter == null) { Debug.LogError("No se encontr� crafteadora"); }


        var hall = GetComponent<NPC>().town;
        foreach (var cost in recipe.costs)
        {
            int avail = hall.town_resources.TownResources_All[cost.type];
            if (avail < cost.amount)
            {
                canCraft = false;
                Debug.Log($"{name}: faltan {cost.type} ({avail}/{cost.amount})");
            }
        }

        target = crafter.gameObject;
        duration = recipe.craftTime;

        runing = true;
        //effects[recipeID + "Crafted"] = 1;
    }


    public override bool PrePerform()
    {
       
        return true;
    }


    public override bool PostPerform()
    {
        if (!canCraft) return false;
        foreach (var cost in recipe.costs)
            GetComponent<NPC>().town.town_resources.SubtractResourceAmount(cost.type, cost.amount);

        Instantiate(recipe.prefab, crafter.outputPoint.position, Quaternion.identity);

        GetComponent<NPC>().OnItemCrafted(recipeID);

        return true;
    }
}
