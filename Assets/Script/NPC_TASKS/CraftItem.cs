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
        // 1) Inicializa diccionarios (Unity no lo hace al AddComponent)
        if (preconditions == null) preconditions = new Dictionary<string, int>();
        if (effects == null) effects = new Dictionary<string, int>();

        // 2) Busca la receta y la estación PARA PODER DEFINIR preconditions y effects
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
            Debug.LogError($"{name}: no se encontró receta '{recipeID}' ni estación.");
            return;
        }

        // 3) Define preconditions según los costes de la receta
        preconditions.Clear();
        foreach (var cost in recipe.costs)
        {
            // el planner ahora sabe que necesita estos recursos
            preconditions[cost.type.ToString()] = cost.amount;
        }

        // 4) Define efectos (clave-meta) para el planner
        effects.Clear();
        effects.Add(recipeID + "Crafted", 1);

        if (crafter == null) { Debug.LogError("No se encontró crafteadora"); }


        // 5) Ahora, antes de ejecutar, comprueba stock real
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

        // 6) Preparamos target y duración
        target = crafter.gameObject;
        duration = recipe.craftTime;

        // 7) Marcamos la acción como corriendo
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
        // 1) Descontar recursos
        foreach (var cost in recipe.costs)
            GetComponent<NPC>().town.town_resources.SubtractResourceAmount(cost.type, cost.amount);

        // 2) Instanciar el objeto fabricado
        Instantiate(recipe.prefab, crafter.outputPoint.position, Quaternion.identity);

        // 3) Notificar al NPC
        GetComponent<NPC>().OnItemCrafted(recipeID);

        return true;
    }
}
