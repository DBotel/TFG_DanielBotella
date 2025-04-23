using UnityEngine;

public class FarmingResources : GAction
{
    [Header("Recurso a farmear")]
    public string resourceTag;                  // p.ej. "Tree", "Ore", etc.
    public TownResourcesTypes resourceType;     // enum WOOD, STONE, ...
    bool stopFarm = false;
    void Start()
    {
        // Efecto: cada vez que ejecutes esta acción, produces 1 unidad  
        effects.Add(resourceType.ToString(), 1);
    }

    public override bool PrePerform()
    {
        // Busca el recurso más cercano
        var resources = GameObject.FindGameObjectsWithTag(resourceTag);
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (var res in resources)
        {
            if (!res.activeInHierarchy) continue;
            float d = Vector3.Distance(transform.position, res.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = res;
            }
        }

        if (closest == null) { Debug.LogError("no hay mas recurosos"); stopFarm = true; PostPerform(); return false; }
        target = closest;
        return true;
    }

    public override bool PostPerform()
    {
        NPC npcScript = GetComponent<NPC>();
        
        // Llama a FarmResources para añadir al TownHall
        if(target!=null)
    {
        var farmComp = target.GetComponent<FarmResources>();
        if (farmComp != null)
        {
            farmComp.FarmResource();

            // Notifica al NPC para que él lleve la cuenta
            int farmAmount = farmComp.value;
            GetComponent<NPC>().OnResourceGathered(resourceType, farmAmount);
        }
    }
        // Comprobar si aún no se ha cumplido y reactivar el subgoal
        if (!npcScript.HasCompletedMission() && !stopFarm)
        {
            npcScript.ReassignCurrentMission();
        }
        else if(npcScript.hasTool)
        {
            npcScript.DropItem(npcScript.toolTag);

        }
    
       G_Agent.actions.Remove(this);
        Destroy(this);
        return true;
    }
}


