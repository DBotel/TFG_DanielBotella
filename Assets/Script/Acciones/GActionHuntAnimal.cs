using UnityEngine;
using UnityEngine.AI;

public class GActionHuntAnimal : GAction
{
    public string animalTag = "Animal";
    public int amountNeeded = 1;
    public string collectStateKey = "collected_MEAT";
    
    private GameObject targetAnimal;

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        preconditions["hasTool_Weapon"] = 1;

        effects[collectStateKey] = amountNeeded;
    }

    public override bool PrePerform()
    {
        var animals = GameObject.FindGameObjectsWithTag(animalTag);
        if (animals.Length == 0)   return false;
        float minDistance = Mathf.Infinity;
        foreach (var a in animals)
        {
            float distance = Vector3.Distance(transform.position, a.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetAnimal = a;
            }
        }
        if (targetAnimal == null) return false;
        agent.SetDestination(targetAnimal.transform.position);
        target= targetAnimal;
        return true;
    }
    
   
    public override bool PostPerform()
    {
        Destroy((targetAnimal != null) ? targetAnimal : target);
        var farm = target.GetComponent<FarmResources>();
        farm.FarmResource();
        G_Agent.beliefs.ModifyState(collectStateKey, farm.value);
        GWorld.Instance.GetWorld().ModifyState("MEAT", farm.value);
        return true;
    }
}

