using UnityEngine;

public class GoalDebugLog : MonoBehaviour
{
    private GAgent agent;

    void Start()
    {
        agent = GetComponent<GAgent>();
        PrintGoals();
    }

    void PrintGoals()
    {
        if (agent == null || agent.goals == null)
        {
            Debug.LogError("[GoalDebugLog] GAgent o goals no encontrados");
            return;
        }

        foreach (var goal in agent.goals)
        {
            foreach (var kv in goal.Key.sGoals)
            {
                Debug.Log("\tMeta: " + kv.Key + " = " + kv.Value + " | Eliminar: " + goal.Key.remove + " | Prioridad: " + goal.Value);
            }
        }
    }
}
