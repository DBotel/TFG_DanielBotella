using UnityEngine;
using UnityEngine.AI;

public class GActionSocialice : GAction
{
    private NPCBasicNeeds needs;
    public override void SetupAction()
    {
        preconditions["lowHappiness"] = 1;
        effects["happinessRestored"] = 1;
    }

    public override bool PrePerform()
    {
        needs = agent.GetComponent<NPCBasicNeeds>();
        if (!needs) return false;
        else
        {
            Tabern[] taberns = FindObjectsOfType<Tabern>();
            if(taberns== null || taberns.Length == 0) return false;
            float minDistance = Mathf.Infinity;
            foreach (Tabern tabern in taberns)
            {
                float distance = Vector3.Distance(transform.position, tabern.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = tabern.gameObject;
                    return true;
                }
            }
            Debug.LogError("Socialice Target: " + target.name);
            return true;
        }
    }
    
    public override bool PostPerform()
    {
        Debug.LogError("PostPerform Socialice");
        if (needs != null)
        {
            needs.happiness += 35f;

        }
        return true;
    }
}
