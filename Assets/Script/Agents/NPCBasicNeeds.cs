using UnityEngine;

public class NPCBasicNeeds : MonoBehaviour 
{
    public float money = 50f;
    public float hunger = 100f;
    public float sleep = 100f;

    public GameObject house;

    GAgent agent;
    void Awake()
    {
        agent = GetComponent<GAgent>();
    }
    void Update()
    {
        SubstractStats();
        EvaluateNeeds();
    }

    void SubstractStats()
    {
        hunger -= Time.deltaTime * 0.10f;
        sleep -= Time.deltaTime * 0.05f;

        hunger = Mathf.Clamp(hunger, 0f, 100f);
        sleep = Mathf.Clamp(sleep, 0f, 100f);
    }
    void HungryOrSleepy()
    {
        if (agent.currentAction != null && agent.currentAction.runing)
            return;

        bool needFood = hunger < 20f;
        bool needSleep = hunger < 20f;

        if (needFood)
        {
            agent.goals.Clear();
            SubGoal eat = new SubGoal("hungerRestored", 1, false);
            agent.goals.Add(eat, 10);
        }

        if (needSleep)
        {
            agent.goals.Clear();
            SubGoal sleep = new SubGoal("rested", 1, false);
            agent.goals.Add(sleep, needFood ? 5 : 9);
        }

    }
    void EvaluateNeeds()
    {
       

        bool isStarving = hunger < 5f;
        bool isExhausted = sleep < 5f;
        bool isHungry = hunger < 20f;
        bool isTired = sleep < 20f;

        if (isStarving || isExhausted)
        {
            agent.ResetPlan();
            agent.goals.Clear();

            if (isStarving)
                agent.goals.Add(new SubGoal("hungerRestored", 1, false), 20);

            if (isExhausted)
                agent.goals.Add(new SubGoal("rested", 1, false), isStarving ? 15 : 18);
        }
        if (agent.currentAction != null )
            return;
         if (isHungry || isTired)
        {
            agent.goals.Clear();

            if (isHungry)
                agent.goals.Add(new SubGoal("hungerRestored", 1, false), 10);

            if (isTired)
                agent.goals.Add(new SubGoal("rested", 1, false), isHungry ? 5 : 9);
        }
    }
    void GainMoney(float amount)
    {
        money += amount;
    }

    void SpendMoney(float amount)
    {
        money -= amount;
    }
}
