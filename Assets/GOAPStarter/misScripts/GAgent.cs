using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
/*------------------------------------------------- EXPLICACION ------------------------------------------------------------
 *1. Clase SubGoal:
Prop�sito: Representa un subobjetivo dentro de un sistema de metas. Los subobjetivos son metas m�s peque�as que contribuyen a alcanzar una meta principal o global.
Atributos:
sGoals: Un diccionario que guarda la clave (el nombre del subobjetivo) y el valor (la condici�n para cumplirlo, por ejemplo, un valor num�rico que representa el progreso
o la necesidad de ese subobjetivo).
remove: Una bandera booleana que indica si el subobjetivo debe ser removido una vez alcanzado.
M�todo:
SubGoal(string s, int i, bool b): El constructor inicializa un subobjetivo con un nombre (s), un valor asociado (i), y una bandera para indicar si debe eliminarse despu�s de
cumplirse (b).
2. Clase GAgent:
Prop�sito: Representa a un agente en el sistema, con una serie de acciones posibles que puede ejecutar para cumplir subobjetivos o metas. Esta clase maneja el proceso de toma
de decisiones del agente, que generalmente involucra planificar y ejecutar una serie de acciones para lograr metas.
Atributos:
actions: Una lista de todas las acciones disponibles para este agente, que son instancias de la clase GAction.
goals: Un diccionario que mapea los subobjetivos (SubGoal) con una prioridad o importancia asociada. Esto permite al agente gestionar y priorizar qu� subobjetivos debe perseguir.
planner: Una instancia de un planificador (GPlanner) que ayuda a determinar qu� acciones ejecutar en funci�n de los subobjetivos del agente. Este planificador genera planes de acci�n.
actionQueue: Una cola de acciones que el agente debe ejecutar, ordenada en la forma en que debe realizar las acciones para cumplir sus objetivos.
currentAction: La acci�n que el agente est� ejecutando actualmente.
currentGoal: El subobjetivo que el agente est� tratando de cumplir en ese momento.
M�todos:
Start(): Este es el m�todo que Unity llama cuando el agente se inicializa. Aqu�, el agente obtiene todas las acciones (GAction) que est�n asociadas a su GameObject y las agrega
a su lista de acciones. Este proceso permite al agente tener acceso a las acciones que puede ejecutar.
LateUpdate(): Este m�todo se llama despu�s del Update() de Unity, lo cual es ideal para agregar l�gica que debe ejecutarse al final de cada frame. Por ejemplo, aqu� podr�a ir la
l�gica para tomar decisiones sobre qu� acci�n ejecutar a continuaci�n, o c�mo gestionar el progreso de los subobjetivos y metas.
Funcionalidad del c�digo:
Subobjetivos y Metas: Los subobjetivos son metas intermedias que el agente debe cumplir en su camino hacia un objetivo m�s grande. El sistema permite agregar prioridades a esos 
subobjetivos y decidir si deben ser eliminados una vez alcanzados.
Planificaci�n: El agente tiene un planificador (GPlanner) que es responsable de generar un conjunto de acciones que se deben ejecutar para cumplir con los subobjetivos. El agente 
puede tener varios subobjetivos a la vez, y el planificador determinar� qu� acciones tomar para lograrlos.
Toma de decisiones: El agente tiene una lista de acciones disponibles y un conjunto de subobjetivos a cumplir. En cada frame, el agente puede revisar sus metas y tomar la siguiente
acci�n de su cola de acciones para acercarse m�s a cumplir sus subobjetivos.
 */
// Clase que representa un subobjetivo dentro de un sistema de metas.
public class SubGoal
{
    // Diccionario que almacena la clave  y su valor
    public Dictionary<string, int> sGoals;

    // Bool que indica si el subobjetivo debe eliminarse despu�s de cumplirse.
    public bool remove;

    // Constructor que inicializa el subobjetivo con una clave, un valor y una bool de eliminaci�n
    public SubGoal(string s, int i, bool r)
    {
        sGoals = new Dictionary<string, int>(); 
        sGoals.Add(s, i); 
        remove = r; 
    }
}

public class GAgent : MonoBehaviour
{
    // Lista de acciones disponibles para este agente
    public List<GAction> actions = new List<GAction>();
    // Diccionario que mapea subobjetivos a su prioridad o importancia
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    // Instancia del planificador (GPlanner) que se encarga de generar los planes para cumplir los subobjetivos
    GPlanner planner;
    // Cola de acciones que se ejecutar�n, en el orden determinado por el planificador
    Queue<GAction> actionQueue;
    public GAction currentAction;
    // Subobjetivo actual que el agente est� persiguiendo
    SubGoal currentGoal;

    public TownHall town;
    public GInventory inventory = new GInventory();
    public GBeliefs beliefs = new GBeliefs();

    protected virtual void Start()
    {
        
        inventory = GetComponent<GInventory>();
        beliefs = new GBeliefs(); // si uso esto en un futuro lo tendra que llevar de componente en el gameObject y hacer GetComponent

        
        // Obtiene todas las componentes de tipo GAction que est�n asociadas a este GameObject
        GAction[] acts = this.GetComponents<GAction>();
        // A�ade cada acci�n obtenida a la lista de acciones del agente
        foreach (GAction a in acts)
        {
            actions.Add(a); 
        }
        
    }

    bool invoked = false;
    
    void CompleteAction()
    {
        currentAction.runing = false;
        currentAction.PostPerform();
        invoked = false;
    }
    void LateUpdate()
    {
        // Aqu� se podr�a agregar la l�gica que se debe ejecutar despu�s de cada frame (como la toma de decisiones, 
        // ejecuci�n de la siguiente acci�n, o actualizaci�n del estado del agente)

        if (currentAction != null && currentAction.runing) 
        { 
            if(currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f)
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if(planner == null  || actionQueue == null)// No hay planes
        {
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach(KeyValuePair<SubGoal, int> subGoal in sortedGoals)
            {
                actionQueue = planner.plan(actions, subGoal.Key.sGoals, null);
                if (actionQueue != null)
                {
                    currentGoal = subGoal.Key;
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0) 
        {
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }

            planner = null;
        }

        if(actionQueue!=null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if(currentAction.target != null)
                {
                    currentAction.runing = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }
}
