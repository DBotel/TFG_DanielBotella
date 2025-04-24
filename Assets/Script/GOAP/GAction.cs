using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*------------------------------- EXPLICACION ---------------------------------------------
 Clase GAction:

Prop�sito: Esta clase define una acci�n que un agente puede realizar en el sistema GOAP. Las acciones tienen precondiciones, efectos, y un costo, 
lo que las hace utilizables en el proceso de planificaci�n. Las subclases de GAction deber�an definir acciones espec�ficas que un agente puede llevar a cabo,
como "Recoger objeto", "Atacar", etc.
Propiedades:

actionName: Nombre de la acci�n (utilizado principalmente para depuraci�n o identificaci�n).
cost: Costo asociado a realizar la acci�n (por ejemplo, el tiempo o los recursos que consume).
target y targetTag: Objetivos de la acci�n. target es un GameObject al que la acci�n se dirige, y targetTag podr�a ser usado para etiquetar tipos de objetivos.
duration: Duraci�n de la acci�n, si aplica (puede ser �til si la acci�n toma tiempo, como caminar hacia un objetivo).
preConditions y afterEffects: Son las condiciones que deben cumplirse para que la acci�n se realice y los efectos que ocurren despu�s de realizarla (usadas en la planificaci�n GOAP).
agent: Componente NavMeshAgent utilizado para mover al agente en el mundo (en caso de que la acci�n requiera movimiento, como moverse hacia un objetivo).
preconditions y effects: Diccionarios que almacenan las precondiciones y efectos de la acci�n. Estos son usados para verificar si una acci�n se puede realizar y c�mo cambiar� el estado
del mundo despu�s de su ejecuci�n.
M�todos:

Awake: Este m�todo se llama cuando el objeto de la acci�n se inicializa. Aqu� se inicializan las precondiciones y efectos a partir de las matrices preConditions y afterEffects 
definidas en el inspector de Unity.
IsAchievable: M�todo que verifica si la acci�n es alcanzable, por defecto siempre devuelve true. Las subclases pueden sobrescribir este m�todo para realizar comprobaciones 
adicionales (por ejemplo, si el agente tiene suficientes recursos para realizar la acci�n).
IsAchievableGiven: M�todo que verifica si la acci�n es alcanzable dado un conjunto de condiciones espec�ficas. Recorre las precondiciones de la acci�n y las compara con las 
condiciones del mundo para asegurarse de que la acci�n se pueda realizar.
PrePerform y PostPerform: M�todos abstractos que deben ser implementados por las subclases. PrePerform se ejecuta antes de realizar la acci�n (por ejemplo, para verificar recursos),
y PostPerform se ejecuta despu�s de realizar la acci�n (por ejemplo, para actualizar el estado del mundo o la creencia del agente).
 */

// Clase  que representa una acci�n en el sistema GOAP
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
                preconditions.Add(w.key, w.value); // Agrega cada precondici�n al diccionario
            }

        if (afterEffects != null)
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value); // Agrega cada efecto al diccionario
            }

        if(G_Agent==null)G_Agent=GetComponent<GAgent>();
    }

    // M�todo  para indicar si la acci�n es alcanzable
    public bool IsAchievable()
    {
        return true; 
    }

    // M�todo que verifica si la acci�n es alcanzable dado un conjunto de condiciones del mundo
    public virtual bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            // Si alguna precondici�n no est� en las condiciones del mundo, la acci�n no es alcanzable
            if (!conditions.ContainsKey(p.Key)) return false;
        }
        // Si todas las precondiciones est�n cubiertas, la acci�n es alcanzable
        return true;
    }

    // M�todos abstractos que deben ser implementados por las subclases de GAction
    // PrePerform se ejecuta antes de realizar la acci�n (p. ej., comprobaci�n de recursos o validaci�n previa)
    public abstract bool PrePerform();

    // PostPerform se ejecuta despu�s de realizar la acci�n (p. ej., actualizaci�n de estado del mundo)
    public abstract bool PostPerform();

    /// <summary>
    /// Se llama justo antes de planificar/executar la acci�n, para refrescar
    /// preconditions y effects seg�n los campos p�blicos actuales.
    /// </summary>
    public virtual void SetupAction()
    {
    }
}