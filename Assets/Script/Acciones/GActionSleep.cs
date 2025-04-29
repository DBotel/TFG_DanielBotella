using UnityEngine;

public class GActionSleep : GAction
{
    NPCBasicNeeds npcNeeds;

    public override void Awake()
    {
        npcNeeds = GetComponent<NPCBasicNeeds>();
    }

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        preconditions["isTired"] = 1;

        effects["rested"] = 1;
    }

    public override bool PrePerform()
    {
        target = npcNeeds.house;
        return target != null;
    }

    public override bool PostPerform()
    {
        npcNeeds.sleep += 80;
        npcNeeds.sleep = Mathf.Clamp(npcNeeds.sleep, 0f, 100f);
        return true;
    }
}
