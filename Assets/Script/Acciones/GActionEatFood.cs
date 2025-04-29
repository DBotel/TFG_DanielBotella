using UnityEngine;

public class GActionEatFood : GAction
{
    public float foodCost = 5f;
    NPCBasicNeeds npcNeeds;

    public override void Awake()
    {
        npcNeeds = GetComponent<NPCBasicNeeds>();
        target = GameObject.FindGameObjectWithTag("FoodStore");
        base.Awake();
    }

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();

        preconditions["isHungry"] = 1;
        preconditions["hasMoney"] = 1;

        effects["hungerRestored"] = 1;
        effects["moneySpent"] = 1;


        if (npcNeeds.money >= foodCost)
        {
            target = GameObject.FindGameObjectWithTag("FoodStore");
        }
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        npcNeeds.money -= foodCost;
        npcNeeds.hunger += 50;
        npcNeeds.hunger = Mathf.Clamp(npcNeeds.hunger, 0f, 100f);
        return true;
    }
}
