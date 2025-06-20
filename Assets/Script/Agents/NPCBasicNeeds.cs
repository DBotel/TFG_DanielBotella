using UnityEngine;
using System.Linq;
public class NPCBasicNeeds : MonoBehaviour
{
    public float money = 50f;
    public float hunger = 100f;
    public float sleep = 100f;
    public GameObject house;
    
    public float happiness = 100f;

    GAgent agent;
    
    const float RATE   = 10f;
     float nextCheck   = 1f;
    void Awake()
    {
        agent = GetComponent<GAgent>();
    }

    void Update()
    {
        SubstractStats();
        if(Time.time> nextCheck)
        {
            EvaluateNeeds();
            nextCheck = Time.time + RATE;

        }
    }

    void SubstractStats()
    {
        hunger -= Time.deltaTime * 0.05f;
        sleep -= Time.deltaTime * 0.02f;
        happiness -= Time.deltaTime * 0.15f;

        hunger = Mathf.Clamp(hunger, 0f, 100f);
        sleep = Mathf.Clamp(sleep, 0f, 100f);
        happiness = Mathf.Clamp(happiness, 0f, 100f);
    }

   void EvaluateNeeds()
{
    agent.beliefs.SetState("hasMoney", money >= 5f ? 1 : 0);
    agent.beliefs.SetState("isHungry", hunger < 20f ? 1 : 0);
    agent.beliefs.SetState("isTired", sleep < 20f ? 1 : 0);
    
    agent.beliefs.SetState("lowHappiness", happiness < 30f ? 1 : 0);
    agent.beliefs.SetState("highHappiness", happiness > 70f ? 1 : 0);
    
    bool isStarving = hunger < 5f;
    bool isExhausted = sleep < 5f;
    bool isHungry = hunger < 20f;
    bool isTired = sleep < 20f;
    bool isUnHappy = happiness < 20f;

    var completed = agent.goals.Keys
        .Where(goal => goal.remove && goal.sGoals.All(s =>
            agent.beliefs.states.ContainsKey(s.Key) &&
            agent.beliefs.states[s.Key] >= s.Value))
        .ToList();

    foreach (var g in completed)
    {
        Debug.LogError("[GOAL COMPLETED MANUAL] " + string.Join(", ", g.sGoals.Select(kv => kv.Key + ">=" + kv.Value)));
        agent.goals.Remove(g);
    }

    if (isStarving || isExhausted)
    {
        agent.ResetPlan();
        agent.goals.Clear();

        if (isStarving && !agent.beliefs.states.ContainsKey("hungerRestored"))
            agent.goals.Add(new SubGoal("hungerRestored", 1, true), 20);

        if (isExhausted && !agent.beliefs.states.ContainsKey("rested"))
            agent.goals.Add(new SubGoal("rested", 1, true), isStarving ? 15 : 18);

        return;
    }

    if(happiness<30f && !agent.beliefs.states.ContainsKey("happinessRestored"))
    {
        agent.ResetPlan();
        agent.goals.Clear();
        agent.goals.Add(new SubGoal("happinessRestored", 1, true), 12);
    }
    if (agent.currentAction != null && agent.currentAction.runing)
        return;

    if (isHungry || isTired)
    {
        agent.goals.Clear();

        if (isHungry && !agent.beliefs.states.ContainsKey("hungerRestored"))
            agent.goals.Add(new SubGoal("hungerRestored", 1, true), 10);

        if (isTired && !agent.beliefs.states.ContainsKey("rested"))
            agent.goals.Add(new SubGoal("rested", 1, true), isHungry ? 5 : 9);
    }
    
    if(isUnHappy && !agent.beliefs.states.ContainsKey("happinessRestored"))
    {
        agent.goals.Add(new SubGoal("happinessRestored", 1, true), 8);
    }
    if (agent.goals.Count == 0 &&
        (agent.currentAction == null || !agent.currentAction.runing) &&
        !agent.goals.Any(g => g.Key.sGoals.ContainsKey("isWandering")))
    {
        agent.goals.Add(new SubGoal("isWandering", 1, true), 1);
    }
}

    public void GainMoney(float amount) => money += amount;
    public void SpendMoney(float amount) => money -= amount;
}
