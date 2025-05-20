using UnityEngine;

public class FarmingResources : GAction
{
    [Header("Recurso a farmear")]
    public string resourceTag;              
    public TownResourcesTypes resourceType; 
    bool stopFarm = false;
    void Start()
    {
        effects.Add(resourceType.ToString(), 1);
    }

    public override bool PrePerform()
    {
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
        
        if(target!=null)
    {
        var farmComp = target.GetComponent<FarmResources>();
        if (farmComp != null)
        {
            farmComp.FarmResource();

            int farmAmount = farmComp.value;
            GetComponent<NPC>().OnResourceGathered(resourceType, farmAmount);
        }
    }
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


