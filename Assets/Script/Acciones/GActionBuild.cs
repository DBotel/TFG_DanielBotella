using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GActionBuild : GAction
{
    [Tooltip("Tiempo que tarda en construir (s)")]
    public float buildDuration = 0.5f;

    private float startTime = 0f;
    private List<GameObject> buildings;
    
    private Building buildingSite;
    
    public bool nowBuilding = false;
    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();
        preconditions["hasTool_Cemento"] = 1;
        effects["build"] = 1;
        TownHall hall = FindObjectOfType<TownHall>();
        buildings = hall.buildings;
        duration = buildDuration;
    }

    public override bool PrePerform()
    {
        nowBuilding = true;
        TownHall hall = FindObjectOfType<TownHall>();
        if (hall == null || hall.buildings == null || hall.buildings.Count == 0)
        {
            Debug.Log("NO hay edificios disponibles para construir.");
            return false;
            
        }

        List<GameObject> availableBuildings = new List<GameObject>();
        foreach (var b in hall.buildings)
        {
            var building = b.GetComponent<Building>();
           if(!building.isBuilt && !building.isUnderConstruction)
            {
                availableBuildings.Add(b.gameObject);
            }
           
        }
        if(availableBuildings.Count == 0)
        {
            G_Agent.beliefs.ModifyState("build", 1);
            return false;
        }
        availableBuildings.Shuffle();
        /*
        var _buildings = hall.buildings
            .OfType<Building>()             
            .Where(s => !s.isBuilt)
            .ToArray();
        if (_buildings.Length == 0)  
        {
            Debug.Log("NO hay edificios disponibles para construir  2.");
            return false;
            
        }
*/
        int idx = Random.Range(0, availableBuildings.Count);
        buildingSite = availableBuildings[idx].GetComponent<Building>();
        buildingSite.isUnderConstruction = true;
        target     = buildingSite.gameObject;
        agent.SetDestination(target.transform.position);

        //StartCoroutine(PerformBuild());
        return true;
    }
    
    IEnumerator PerformBuild()
    {
        
        yield return new WaitUntil(() => Vector3.Distance(
            agent.transform.position,
            target.transform.position) < 1.5f);

        yield return new WaitForSeconds(buildDuration);

        PostPerform();
    }
    
    public override bool PostPerform()
    {
        G_Agent.beliefs.ModifyState("hasCement", -1);
        G_Agent.beliefs.ModifyState("build", 1);
        buildingSite.Build();
        startTime = 0f;
        
        NPCRoleAssigner npcRoleAssigner = GetComponent<NPCRoleAssigner>();
        npcRoleAssigner.ChangeRole();
        nowBuilding = false;
        return true;
    }
}













public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    /// <summary>
    /// Desordena la lista in-place usando Fisher–Yates.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            // swap list[k] y list[n]
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
}