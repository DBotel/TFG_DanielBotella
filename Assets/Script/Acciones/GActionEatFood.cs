using UnityEngine;

public class GActionEatFood : GAction
{
    NPCBasicNeeds needs;
    public float eatDuration = 2f;
    public float foodCost = 5f;

    public override void Awake()
    {
        needs = GetComponent<NPCBasicNeeds>();
        base.Awake();
    }

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        preconditions["hasMoney"] = 1;
        effects["hungerRestored"] = 1;

        targetTag = "FoodStore";

        duration = eatDuration;
    }

    public override bool IsAchievableGiven(System.Collections.Generic.Dictionary<string, int> conditions)
    {
        return needs.money >= foodCost;
    }

    public override bool PrePerform()
    {
        target = GameObject.FindGameObjectWithTag(targetTag);
        return target != null;
    }

    public override bool PostPerform()
    {
        Debug.Log("[EatFood] PostPerform ejecutado.");
        needs.money -= foodCost;
        needs.hunger = 100f;
        needs.hunger = Mathf.Clamp(needs.hunger, 0f, 100f);
        return true;
    }
}
