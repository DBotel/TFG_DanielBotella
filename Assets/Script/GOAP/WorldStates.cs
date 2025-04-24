using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

/* -------------------------------------------------- EXPLICACION -------------------------------------------------------
 Clase WorldState:

Propósito: Representa un estado único en el mundo. La clase tiene una clave (key) que describe el tipo de estado (por ejemplo, "energía", "salud", "ubicación") y un valor (value)
que representa el estado cuantificable (por ejemplo, "100 de energía", "está en la posición X del mapa").
Uso: Este objeto se utiliza en sistemas donde es necesario almacenar y manipular el estado del mundo del agente, como en el caso de un sistema GOAP para verificar precondiciones 
y efectos.
Clase WorldStates:

Propósito: Esta clase contiene un diccionario de múltiples WorldStates y proporciona métodos para añadir, modificar, eliminar y consultar los estados del mundo. La clase permite
manejar y actualizar los estados del mundo del agente, que son cruciales para la planificación de acciones (por ejemplo, en un sistema GOAP).
Métodos:
HasState: Verifica si un estado específico existe en el diccionario de estados.
AddState: Añade un nuevo estado al diccionario.
ModifyState: Modifica el valor de un estado existente, o lo añade si no existe. Si el valor del estado llega a ser cero o negativo, elimina el estado del diccionario.
RemoveState: Elimina un estado del diccionario.
SetState: Establece o actualiza el valor de un estado existente, o lo añade si no existe.
GetStates: Devuelve el diccionario completo de estados, útil para obtener todos los estados actuales del mundo.
Funcionalidad:
Manejo de estados del mundo: Este sistema permite que el agente o el mundo puedan tener un conjunto de estados que se pueden modificar y consultar en cualquier momento.
Esto es fundamental para un sistema GOAP, ya que las acciones dependen del estado del mundo, y cada acción tiene efectos sobre el estado del mundo.
Modificación dinámica del estado: Los métodos como ModifyState permiten que el estado de un mundo cambie durante la ejecución del juego o el proceso de planificación. 
Esto puede ser útil en situaciones donde, por ejemplo, un agente gane o pierda energía, o cambie de ubicación.
  */
// Clase que representa un estado en el mundo 
public class WorldState
{
    public string key; 
    public int value; 
}

// Clase que representa los estados del mundo de un agente 
public class WorldStates
{
    public Dictionary<string, int> states;

    // Constructor de la clase
    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    // Método que verifica si un estado específico está presente en el mundo (diccionario)
    public bool HasState(string key)
    {
        return states.ContainsKey(key); // Devuelve true si la clave existe en el diccionario
    }

    // Método que añade un nuevo estado al diccionario
    public void AddState(string key, int value)
    {
        states.Add(key, value); 
    }

    // Método que modifica el valor de un estado existente. Si el estado no existe, lo añade.
    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key)) 
        {
            states[key] += value; // Modifica el valor sumando el valor proporcionado
            if (states[key] <= 0) RemoveState(key); // Si el valor llega a cero o es negativo, elimina el estado
        }
        else
            states.Add(key, value); // Si el estado no existe, lo añade con el valor proporcionado
    }

    // Método que elimina un estado del diccionario
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key)) 
        {
            states.Remove(key); 
        }
    }

    // Método que establece el valor de un estado, o lo añade si no existe
    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
            states[key] = value; // Si el estado existe, actualiza su valor
        else
            states.Add(key, value); // Si el estado no existe, lo añade 
    }

    // Método que devuelve el diccionario de estados
    public Dictionary<string, int> GetStates()
    {
        return states; 
    }
}