using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

/* -------------------------------------------------- EXPLICACION -------------------------------------------------------
 Clase WorldState:

Proposito: Representa un estado unico en el mundo. La clase tiene una clave (key) que describe el tipo de estado (por ejemplo, "energ�a", "salud", "ubicaci�n") y un valor (value)
que representa el estado cuantificable (por ejemplo, "100 de energ�a", "est� en la posici�n X del mapa").
Uso: Este objeto se utiliza en sistemas donde es necesario almacenar y manipular el estado del mundo del agente, como en el caso de un sistema GOAP para verificar precondiciones 
y efectos.
Clase WorldStates:

Prop�sito: Esta clase contiene un diccionario de m�ltiples WorldStates y proporciona m�todos para a�adir, modificar, eliminar y consultar los estados del mundo. La clase permite
manejar y actualizar los estados del mundo del agente, que son cruciales para la planificaci�n de acciones (por ejemplo, en un sistema GOAP).
M�todos:
HasState: Verifica si un estado espec�fico existe en el diccionario de estados.
AddState: Añade un nuevo estado al diccionario.
ModifyState: Modifica el valor de un estado existente, o lo a�ade si no existe. Si el valor del estado llega a ser cero o negativo, elimina el estado del diccionario.
RemoveState: Elimina un estado del diccionario.
SetState: Establece o actualiza el valor de un estado existente, o lo a�ade si no existe.
GetStates: Devuelve el diccionario completo de estados, �til para obtener todos los estados actuales del mundo.
Funcionalidad:
Manejo de estados del mundo: Este sistema permite que el agente o el mundo puedan tener un conjunto de estados que se pueden modificar y consultar en cualquier momento.
Esto es fundamental para un sistema GOAP, ya que las acciones dependen del estado del mundo, y cada acci�n tiene efectos sobre el estado del mundo.
Modificaci�n din�mica del estado: Los m�todos como ModifyState permiten que el estado de un mundo cambie durante la ejecuci�n del juego o el proceso de planificaci�n. 
Esto puede ser �til en situaciones donde, por ejemplo, un agente gane o pierda energ�a, o cambie de ubicaci�n.
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

    // Metodo que verifica si un estado espec�fico est� presente en el mundo (diccionario)
    public bool HasState(string key)
    {
        return states.ContainsKey(key); // Devuelve true si la clave existe en el diccionario
    }

    // Metodo que a�ade un nuevo estado al diccionario
    public void AddState(string key, int value)
    {
        states.Add(key, value); 
    }

    // Metodo que modifica el valor de un estado existente. Si el estado no existe, lo a�ade.
    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key)) 
        {
            states[key] += value; // Modifica el valor sumando el valor proporcionado
            if (states[key] <= 0) RemoveState(key); // Si el valor llega a cero o es negativo, elimina el estado
        }
        else
            states.Add(key, value); // Si el estado no existe, lo a�ade con el valor proporcionado
    }

    // Metodo que elimina un estado del diccionario
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key)) 
        {
            states.Remove(key); 
        }
    }
    
    public int GetState(string key)
    {
        if (states.ContainsKey(key))
            return states[key]; 
        return 0; 
    }

    // Metodo que establece el valor de un estado, o lo a�ade si no existe
    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
            states[key] = value; // Si el estado existe, actualiza su valor
        else
            states.Add(key, value); // Si el estado no existe, lo a�ade 
    }

    // Metodo que devuelve el diccionario de estados
    public Dictionary<string, int> GetStates()
    {
        return states; 
    }
}