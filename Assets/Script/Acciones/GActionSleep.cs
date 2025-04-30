using UnityEngine;

public class GActionSleep : GAction
{
    NPCBasicNeeds needs;

    public float sleepAmount = 60f;
    public float sleepDuration = 5f;

    public override void Awake()
    {
        base.Awake();
        needs = GetComponent<NPCBasicNeeds>();
    }

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        effects["rested"] = 1;
       // targetTag = "House";
        duration = sleepDuration;
    }

    public override bool PrePerform()
    {
        target = needs.house;
        Debug.Log("[Sleep] NPC va a dormir a: " + (target != null ? target.name : "null"));
        return target != null;
    }

    public override bool PostPerform()
    {
        Debug.Log("[Sleep] NPC ha dormido. +" + sleepAmount + " sueño");
        needs.sleep = 100;
        needs.sleep = Mathf.Clamp(needs.sleep, 0f, 100f);
        
        return true;
    }
}