using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GActionFarmResource : GAction
{
    [Header("Recurso a farmear (Tag)")]
    public string resourceTag;
    [Header("Tipo de recurso en enum")]
    public TownResourcesTypes resourceType;
    [Header("Cantidad total necesaria de recurso (unidades)")]
    public int amountNeeded = 5;

    private bool harvested = false;

    private void Start()
    {
        // Precondición: necesito herramienta
        string toolKey = "hasTool_" + GetRequiredToolTag();
        if (!preconditions.ContainsKey(toolKey))
            preconditions.Add(toolKey, 1);

        // Efecto: aumentar recurso recolectado en la planificación
        string collectKey = "collected_" + resourceType;
        if (!effects.ContainsKey(collectKey))
        {
            int effectValue = 1;
            var nodes = GameObject.FindGameObjectsWithTag(resourceTag);
            if (nodes.Length > 0)
            {
                var exampleNode = nodes[0].GetComponent<FarmResources>();
                if (exampleNode != null)
                    effectValue = exampleNode.value;
            }
            effects.Add(collectKey, effectValue);
            Debug.Log($"[FarmResource] Efecto añadido: {collectKey} += {effectValue}");
        }
    }

    // Limitar recursión: solo farmear si no se ha alcanzado la cantidad deseada
    public override bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        if (!base.IsAchievableGiven(conditions))
            return false;

        string collectKey = "collected_" + resourceType;
        conditions.TryGetValue(collectKey, out int have);
        return have < amountNeeded;
    }

    public override bool PrePerform()
    {
        harvested = false;

        var nodes = GameObject.FindGameObjectsWithTag(resourceTag);
        if (nodes.Length == 0)
            return false;

        // Seleccionamos el recurso más cercano
        target = nodes.OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).First();
        return true;
    }

    public override bool PostPerform()
    {
        if (target != null && !harvested)
        {
            var farmScript = target.GetComponent<FarmResources>();
            if (farmScript != null)
            {
                int val = farmScript.value;
                farmScript.FarmResource();

                // Actualizamos creencias del agente
                string collectKey = "collected_" + resourceType;
                if (!G_Agent.beliefs.states.ContainsKey(collectKey))
                    G_Agent.beliefs.states.Add(collectKey, 0);
                G_Agent.beliefs.ModifyState(collectKey, val);

                // Actualizamos el estado global
                GWorld.Instance.GetWorld().ModifyState(resourceType.ToString(), val);

                Debug.Log($"[FarmResource] Recolectado {val} unidades de {resourceType}");
            }
            harvested = true;
        }
        return true;
    }

    private string GetRequiredToolTag()
    {
        return resourceType switch
        {
            TownResourcesTypes.WOOD => "Axe",
            TownResourcesTypes.STONE => "Pickaxe",
            _ => string.Empty,
        };
    }
}