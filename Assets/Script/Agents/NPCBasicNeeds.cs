using UnityEngine;
using System.Linq;
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
        hunger -= Time.deltaTime * 0.05f;
        sleep -= Time.deltaTime * 0.02f;

        hunger = Mathf.Clamp(hunger, 0f, 100f);
        sleep = Mathf.Clamp(sleep, 0f, 100f);
    }

   void EvaluateNeeds()
{
    // Actualizamos la creencia sobre el dinero de forma directa
    agent.beliefs.SetState("hasMoney", money >= 5f ? 1 : 0);

    bool isStarving = hunger < 5f;
    bool isExhausted = sleep < 5f;
    bool isHungry = hunger < 20f;
    bool isTired = sleep < 20f;

    // Limpia metas ya satisfechas (remove = true y condición ya cumplida)
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

    // Emergencia: prioridad máxima
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

    // Si está haciendo algo, no interrumpir
    if (agent.currentAction != null && agent.currentAction.runing)
        return;

    // Necesidades moderadas
    if (isHungry || isTired)
    {
        agent.goals.Clear();

        if (isHungry && !agent.beliefs.states.ContainsKey("hungerRestored"))
            agent.goals.Add(new SubGoal("hungerRestored", 1, true), 10);

        if (isTired && !agent.beliefs.states.ContainsKey("rested"))
            agent.goals.Add(new SubGoal("rested", 1, true), isHungry ? 5 : 9);
    }

    // Meta por defecto: deambular si está libre
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
