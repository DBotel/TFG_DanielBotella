using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(GBeliefs), typeof(GInventory))]
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    private GPlanner planner;
    private Queue<GAction> actionQueue;
    public GAction currentAction;
    private bool planningCombined = false;
    private bool invoked = false;

    [HideInInspector]
    public GInventory inventory;
    [HideInInspector]
    public GBeliefs beliefs;

    public TownHall town;
    public bool selected = false;

    protected virtual void Awake()
    {
        // Asegurar componentes
        inventory = GetComponent<GInventory>();
        beliefs = GetComponent<GBeliefs>();
    }

    protected virtual void Start()
    {
        // Carga todas las acciones disponibles
        GAction[] acts = this.GetComponents<GAction>();
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

    protected virtual void LateUpdate()
    {
        // 1) Si estás seleccionando al NPC por UI, ignora GOAP
        if (selected)
        {
            if (actions.Count > 0 && actions[0].GetComponent<Wander>())
                actions.Clear();
            goals.Clear();
            return;
        }

        // 2) Si una acción está corriendo, espera a que termine
        if (currentAction != null && currentAction.runing)
        {
            if (currentAction.agent.hasPath
                && currentAction.agent.remainingDistance < 1f
                && !invoked)
            {
                Invoke(nameof(CompleteAction), currentAction.duration);
                invoked = true;
            }
            return;
        }

        // 3) Si no hay plan o ya se acabó, genera uno nuevo
        if (planner == null || actionQueue == null)
        {
            // ←—— **Recargamos** TODAS las acciones antes de planificar
            actions = this.GetComponents<GAction>().ToList();

            // ←—— **Inject** dinámicamente sus preconditions/effects
            foreach (var a in actions)
                a.SetupAction();

            // Combinamos metas
            var combinedGoals = new Dictionary<string, int>();
            foreach (var g in goals)
                foreach (var req in g.Key.sGoals)
                    combinedGoals[req.Key] = req.Value;

            LogAgentState();
            Debug.Log("[Planner] Planificando para metas: " +
          string.Join(", ", combinedGoals.Select(kv => kv.Key + ">=" + kv.Value)));

            planner = new GPlanner();
            actionQueue = planner.plan(this, actions, combinedGoals);
        }

        // 4) Si tenemos acciones planificadas, ejecútalas
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
                    Debug.LogWarning($"[GAgent] Acción {currentAction.actionName} sin target.");
                    actionQueue = null;
                }
            }
            else
            {
                actionQueue = null;
            }
            return;
        }

        // 5) Si ya no quedan acciones, limpia metas y resetea planner
        if (actionQueue != null && actionQueue.Count == 0)
        {
            var toRemove = goals.Where(kv => kv.Key.remove).Select(kv => kv.Key).ToList();
            foreach (var g in toRemove)
                goals.Remove(g);
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
