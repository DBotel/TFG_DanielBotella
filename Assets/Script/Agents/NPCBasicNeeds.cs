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

        // Emergencia: reiniciamos plan y damos máxima prioridad
        if (isStarving || isExhausted)
        {
            agent.ResetPlan();
            agent.goals.Clear();

            if (isStarving)
                agent.goals.Add(new SubGoal("hungerRestored", 1, true), 20);

            if (isExhausted)
                agent.goals.Add(new SubGoal("rested", 1, true), isStarving ? 15 : 18);

            return;
        }

        // Si está ejecutando una acción, no interrumpimos
        if (agent.currentAction != null && agent.currentAction.runing)
            return;

        // Necesidades moderadas
        if (isHungry || isTired)
        {
            agent.goals.Clear();

            if (isHungry)
                agent.goals.Add(new SubGoal("hungerRestored", 1, true), 10);

            if (isTired)
                agent.goals.Add(new SubGoal("rested", 1, true), isHungry ? 5 : 9);
        }
    }

    public void GainMoney(float amount) => money += amount;
    public void SpendMoney(float amount) => money -= amount;
}
