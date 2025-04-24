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
        List<GAction> usableActions = new List<GAction>();

        foreach (GAction action in actions)
        {
            if (action.IsAchievable())
                usableActions.Add(action);
        }

        List<Node> leaves = new List<Node>();

        // Usar las creencias del agente como estado inicial
        Node start = new Node(null, 0, new Dictionary<string, int>(agent.beliefs.states), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            Debug.Log("NO HAY PLAN");
            return null;
        }

        Node cheapest = leaves.OrderBy(n => n.cost).FirstOrDefault();

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
            Debug.Log("Q : " + a.actionName);
            queue.Enqueue(a);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;

        foreach (GAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                foreach (var eff in action.effects)
                {
                    if (currentState.ContainsKey(eff.Key))
                        currentState[eff.Key] += eff.Value;
                    else
                        currentState[eff.Key] = eff.Value;
                }

                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    if (node.parent != null && node.state.SequenceEqual(node.parent.state))
                        continue; // previene bucles infinitos

                    List<GAction> subset = usableActions.Where(a => !a.Equals(action) || a is GActionFarmResource).ToList();
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (var g in goal)
        {
            if (!state.ContainsKey(g.Key) || state[g.Key] < g.Value)
                return false;
        }
        return true;
    }
}
