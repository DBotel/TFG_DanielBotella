using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*------------------------------- EXPLICACION ---------------------------------------------
 Clase GAction:

Propósito: Esta clase define una acción que un agente puede realizar en el sistema GOAP. Las acciones tienen precondiciones, efectos, y un costo, 
lo que las hace utilizables en el proceso de planificación. Las subclases de GAction deberían definir acciones específicas que un agente puede llevar a cabo,
como "Recoger objeto", "Atacar", etc.
Propiedades:

actionName: Nombre de la acción (utilizado principalmente para depuración o identificación).
cost: Costo asociado a realizar la acción (por ejemplo, el tiempo o los recursos que consume).
target y targetTag: Objetivos de la acción. target es un GameObject al que la acción se dirige, y targetTag podría ser usado para etiquetar tipos de objetivos.
duration: Duración de la acción, si aplica (puede ser útil si la acción toma tiempo, como caminar hacia un objetivo).
preConditions y afterEffects: Son las condiciones que deben cumplirse para que la acción se realice y los efectos que ocurren después de realizarla (usadas en la planificación GOAP).
agent: Componente NavMeshAgent utilizado para mover al agente en el mundo (en caso de que la acción requiera movimiento, como moverse hacia un objetivo).
preconditions y effects: Diccionarios que almacenan las precondiciones y efectos de la acción. Estos son usados para verificar si una acción se puede realizar y cómo cambiará el estado
del mundo después de su ejecución.
Métodos:

Awake: Este método se llama cuando el objeto de la acción se inicializa. Aquí se inicializan las precondiciones y efectos a partir de las matrices preConditions y afterEffects 
definidas en el inspector de Unity.
IsAchievable: Método que verifica si la acción es alcanzable, por defecto siempre devuelve true. Las subclases pueden sobrescribir este método para realizar comprobaciones 
adicionales (por ejemplo, si el agente tiene suficientes recursos para realizar la acción).
IsAchievableGiven: Método que verifica si la acción es alcanzable dado un conjunto de condiciones específicas. Recorre las precondiciones de la acción y las compara con las 
condiciones del mundo para asegurarse de que la acción se pueda realizar.
PrePerform y PostPerform: Métodos abstractos que deben ser implementados por las subclases. PrePerform se ejecuta antes de realizar la acción (por ejemplo, para verificar recursos),
y PostPerform se ejecuta después de realizar la acción (por ejemplo, para actualizar el estado del mundo o la creencia del agente).
 */

// Clase  que representa una acción en el sistema GOAP
public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";
    public float cost = 1f;
    public GameObject target;
    public string targetTag;
    public float duration = 0f;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;
    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;

    public WorldStates agentsBeliefs;
    public GAgent G_Agent;
    public bool runing = false;

    // Constructor de la clase GAction
    // Inicializa los diccionarios de precondiciones y efectos
    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        if (preConditions != null)
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value); // Agrega cada precondición al diccionario
            }

        if (afterEffects != null)
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value); // Agrega cada efecto al diccionario
            }

        if(G_Agent==null)G_Agent=GetComponent<GAgent>();
    }

    // Método  para indicar si la acción es alcanzable
    public bool IsAchievable()
    {
        return true; 
    }

    // Método que verifica si la acción es alcanzable dado un conjunto de condiciones del mundo
    public virtual bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            // Si alguna precondición no está en las condiciones del mundo, la acción no es alcanzable
            if (!conditions.ContainsKey(p.Key)) return false;
        }
        // Si todas las precondiciones están cubiertas, la acción es alcanzable
        return true;
    }

    // Métodos abstractos que deben ser implementados por las subclases de GAction
    // PrePerform se ejecuta antes de realizar la acción (p. ej., comprobación de recursos o validación previa)
    public abstract bool PrePerform();

    // PostPerform se ejecuta después de realizar la acción (p. ej., actualización de estado del mundo)
    public abstract bool PostPerform();

    /// <summary>
    /// Se llama justo antes de planificar/executar la acción, para refrescar
    /// preconditions y effects según los campos públicos actuales.
    /// </summary>
    public virtual void SetupAction()
    {
    }
}