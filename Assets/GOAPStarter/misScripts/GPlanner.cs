using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

/*------------------------------ EXPLICACION----------------------------------------------------
 Clase Node:
Propósito: Representa un nodo en el gráfico de búsqueda de un plan. Cada nodo tiene un estado, un costo asociado (que representa el costo de llegar hasta ese nodo) y una acción que se realiza para llegar a ese estado. Además, cada nodo tiene un padre (que es otro nodo del que proviene).
Método plan en GPlanner:
Propósito: Este es el corazón del planificador. Dado un conjunto de acciones posibles y un objetivo, intenta crear un plan (una secuencia de acciones) que conduzca al agente hacia su meta.
Primero, filtra las acciones alcanzables usando IsAchievable().
Luego, crea un nodo inicial con el estado actual del mundo y comienza a construir un gráfico de nodos (por ejemplo, mediante una búsqueda de grafos).
Una vez que se encuentran los nodos hoja (soluciones), selecciona la solución más barata (con menor costo).
Finalmente, reconstruye el plan desde la hoja más barata hasta el nodo raíz, generando una cola de acciones que el agente debe seguir.
 */
public class Node
{
    public Node parent; // Nodo padre del que proviene 
    public float cost; // Costo asociado a alcanzar este nodo desde el inicio
    public Dictionary<string, int> state; // Estado del mundo en este nodo 
    public GAction action; // Acción que se realiza para alcanzar este nodo

    // Constructor 
    public Node(Node parent, float cost, Dictionary<string, int> allstates, GAction action)
    {
        this.parent = parent; 
        this.cost = cost; 
        this.state = new Dictionary<string, int>(allstates); // Crea una copia del estado
        this.action = action; 
    }
}

// Clase que representa el planificador de acciones
public class GPlanner
{
    // Método que planifica un conjunto de acciones para alcanzar un objetivo
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldState states)
    {
        // Lista para almacenar las acciones que son alcanzables
        List<GAction> usableActions = new List<GAction>();

        // Filtra las acciones alcanzables
        foreach (GAction action in actions)
        {
            if (action.IsAchievable()) // Si la acción es alcanzable
                usableActions.Add(action); // Se añade a la lista de acciones utilizables
        }

        // Lista de nodos "hoja" (finales) que se generan durante la construcción del gráfico
        List<Node> leaves = new List<Node>();

        // Crea un nodo inicial con el estado actual del mundo y sin acción (nodo raíz)
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), null);

        // Intenta construir el gráfico de nodos a partir del nodo raíz (start)
        bool success = BuildGraph(start, leaves, usableActions, goal);

        // Si no se pudo construir un plan (gráfico), devuelve null
        if (!success)
        {
            Debug.Log("NO HAY PLAN");
            return null;
        }

        // Encuentra el nodo con el menor costo en las soluciones alcanzadas
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf; 
            else
                if (leaf.cost < cheapest.cost) // Si el nodo actual es más barato, lo selecciona
                cheapest = leaf;
        }

        // Lista que almacenará las acciones que forman el plan final
        List<GAction> result = new List<GAction>();

        // Recorre los nodos desde el más barato hasta la raíz para reconstruir el plan
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action); // Añade la acción en el principio de la lista para reconstruir el plan
            }
            n = n.parent; // Retrocede al nodo padre
        }

        // Convierte el resultado en una cola de acciones 
        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result)
        {
            queue.Enqueue(a); // Añade las acciones a la cola en el orden correcto
        }

        Debug.Log("The Plan is :");
        foreach (GAction a in queue)
        {
            Debug.Log("Q : " + a.actionName); 
        }

        // Devuelve la cola de acciones que debe ejecutar
        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        // Variable para indicar si se encontró al menos un camino hacia el objetivo.
        bool foundPath = false;

        // Itera sobre cada acción disponible y comprobamos si es ejecutable dadas las condiciones actuales.
        foreach (GAction action in usableActions)
        {
            // Verifica si la acción puede lograrse dados los estados actuales del nodo padre.
            if (action.IsAchievableGiven(parent.state))
            {
                // Crea una copia del estado actual para simular los efectos de la acción.
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                // Aplica los efectos de la acción al estado actual.
                foreach (KeyValuePair<string, int> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                        currentState.Add(eff.Key, eff.Value); // Agrega el efecto si no existe.
                }

                // Crea un nuevo nodo representando este paso en el gráfico.
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                // Verifica si con este nuevo estado se cumple el objetivo.
                if (GoalAchieved(goal, currentState))
                {
                    // Si el objetivo se alcanza, añade este nodo a la lista de "hojas".
                    leaves.Add(node);
                    foundPath = true; // Se encontró un camino hacia el objetivo.
                }
                else
                {
                    // Si el objetivo aún no se alcanza, reduce la lista de acciones disponibles.
                    List<GAction> subset = ActionSubset(usableActions, action);

                    // Llama recursivamente a BuildGraph para explorar este nuevo nodo.
                    bool found = BuildGraph(node, leaves, subset, goal);

                    // Si se encontró un camino en esta rama, actualiza el indicador.
                    if (found)
                        foundPath = true;
                }
            }
        }

        // Devuelve si se encontró un camino hacia el objetivo desde este punto.
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        // Recorre todos los estados necesarios para alcanzar el objetivo.
        foreach (KeyValuePair<string, int> g in goal)
        {
            // Si algún estado requerido no está presente en el estado actual, el objetivo no está logrado.
            if (!state.ContainsKey(g.Key))
                return false;
        }

        // Si todos los estados requeridos están presentes, el objetivo se considera alcanzado.
        return true;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)
    {
        // Crea un subconjunto de las acciones excluyendo la acción actual.
        List<GAction> subset = new List<GAction>();

        foreach (GAction a in actions)
        {
            // Solo añade las acciones que no sean la que se está excluyendo.
            if (!a.Equals(removeMe))
                subset.Add(a);
        }

        // Devuelve el subconjunto de acciones.
        return subset;
    }
}
