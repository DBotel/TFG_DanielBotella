using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allstates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates);
        this.action = action;
    }
}

public class GPlanner
{
    public Queue<GAction> plan(GAgent agent, List<GAction> actions, Dictionary<string, int> goal)
    {
        // Filtrar acciones alcanzables seg�n estado inicial del agente
        List<GAction> usableActions = actions.Where(a => a.IsAchievable()).ToList();

        // Estado inicial a partir de creencias
        Node start = new Node(null, 0, new Dictionary<string, int>(agent.beliefs.states), null);
        List<Node> leaves = new List<Node>();

        bool success = BuildGraph(start, leaves, usableActions, goal);
        if (!success)
        {
            return null;
        }

        // Seleccionar el plan de menor coste
        Node cheapest = leaves.OrderBy(n => n.cost).First();

        // Reconstruir secuencia de acciones
        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
                result.Insert(0, n.action);
            n = n.parent;
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result)
        {
            queue.Enqueue(a);
        }
        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;

        // Por cada acci�n alcanzable desde el estado del padre
        foreach (GAction action in usableActions)
        {
            if (!action.IsAchievableGiven(parent.state))
                continue;

            // Aplicar efectos para generar nuevo estado
            Dictionary<string, int> newState = new Dictionary<string, int>(parent.state);
            foreach (var eff in action.effects)
            {
                if (newState.ContainsKey(eff.Key))
                    newState[eff.Key] += eff.Value;
                else
                    newState[eff.Key] = eff.Value;
            }

            Node node = new Node(parent, parent.cost + action.cost, newState, action);

            // Si el nuevo estado cumple la meta, agregar a leaves
            if (GoalAchieved(goal, newState))
            {
                leaves.Add(node);
                foundPath = true;
            }
            else
            {
                // Crear subconjunto sin reutilizar la misma acci�n para evitar loops
                List<GAction> subset = usableActions.Where(a => !a.Equals(action)).ToList();
                bool found = BuildGraph(node, leaves, subset, goal);
                if (found)
                    foundPath = true;
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (var g in goal)
        {
            if (!state.ContainsKey(g.Key) || state[g.Key] < g.Value)
            {
                return false;
            }
                
        }
        return true;
    }
}
