using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    private GPlanner planner;
    private Queue<GAction> actionQueue;
    public GAction currentAction;
    private bool planningCombined = false;

    public TownHall town;
    public GInventory inventory;
    public GBeliefs beliefs;

    public bool selected = false;

    protected virtual void Start()
    {
        inventory = GetComponent<GInventory>();
        beliefs = new GBeliefs();

        // Carga todas las acciones disponibles
        GAction[] acts = GetComponents<GAction>();
        actions = acts.ToList();
    }

    public void ResetPlan()
    {
        goals.Clear();
        if (actionQueue != null)
            actionQueue.Clear();
        planningCombined = false;
        planner = null;
        actionQueue = null;
        currentAction = null;
    }

    void CompleteAction()
    {
        if (currentAction != null)
        {
            currentAction.runing = false;
            currentAction.PostPerform();
        }
        planner = null;
        invoked = false;
    }

    private bool invoked = false;

    protected virtual void LateUpdate()
    {
        if (selected)
        {
            // UI selection override: ignora GOAP
            if (actions.Count > 0 && actions[0].GetComponent<Wander>())
                actions.Clear();
            goals.Clear();
            return;
        }

        // Si hay una acción corriendo, espera a completarla
        if (currentAction != null && currentAction.runing)
        {
            if (currentAction.agent.hasPath &&
                currentAction.agent.remainingDistance < 1f &&
                !invoked)
            {
                Invoke(nameof(CompleteAction), currentAction.duration);
                invoked = true;
            }
            return;
        }

        // Si no hay cola de acciones, genera un nuevo plan para TODO goals
        if (actionQueue == null)
        {
            // Combinar todas las metas en un solo diccionario
            Dictionary<string, int> combinedGoals = new Dictionary<string, int>();
            foreach (var goal in goals)
                foreach (var req in goal.Key.sGoals)
                    combinedGoals[req.Key] = req.Value;

            // Registrar estado y metas
            LogAgentState();
            Debug.Log("[Planner] Planificando para metas combinadas: " +
                      string.Join(", ", combinedGoals.Select(kv => kv.Key + ">=" + kv.Value)));

            planner = new GPlanner();
            actionQueue = planner.plan(this, actions, combinedGoals);
            planningCombined = (actionQueue != null);
        }

        // Ejecutar siguiente acción de la cola
        if (actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && !string.IsNullOrEmpty(currentAction.targetTag))
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if (currentAction.target != null)
                {
                    currentAction.runing = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
                else
                {
                    Debug.LogWarning($"[GAgent] Acción {currentAction.actionName} no tiene target válido.");
                    actionQueue = null;
                }
            }
            else
            {
                // Acción no pudo comenzar, descartar plan
                actionQueue = null;
            }

            return;
        }

        // Si la cola está vacía, hemos completado el plan
        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (planningCombined)
            {
                // Eliminar todas las metas marcadas para limpiar
                var toRemove = goals.Where(kv => kv.Key.remove).Select(kv => kv.Key).ToList();
                foreach (var g in toRemove)
                    goals.Remove(g);
                planningCombined = false;
            }

            planner = null;
        }
    }

    void LogAgentState()
    {
        Debug.Log("[AgentState] Creencias:");
        foreach (var kv in beliefs.states)
            Debug.Log($"    {kv.Key} = {kv.Value}");

        Debug.Log("[AgentState] Metas:");
        foreach (var g in goals)
            foreach (var req in g.Key.sGoals)
                Debug.Log($"    {req.Key} >= {req.Value} (prio {g.Value}, remove {g.Key.remove})");
    }
}