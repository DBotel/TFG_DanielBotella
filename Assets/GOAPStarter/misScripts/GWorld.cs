using UnityEngine;
/*---------------------------------------- EXPLICACION ------------------------------------------------
 Patrón Singleton:

Propósito: La clase GWorld utiliza el patrón Singleton para garantizar que solo haya una instancia de la clase en toda la aplicación. Esto es útil cuando se necesita un acceso
centralizado y único a la información global, como el estado del mundo en este caso.
Cómo funciona:
La instancia estática instance es la única instancia de la clase GWorld.
El constructor estático static GWorld() se encarga de inicializar la instancia de world (el estado global del mundo) la primera vez que se accede a la clase.
El constructor privado private GWorld() evita que otras clases creen instancias adicionales de GWorld. La única forma de acceder a la instancia es a través de la propiedad estática
Instance.

Manejo del mundo:

Propósito: La clase GWorld contiene el estado global del mundo a través del objeto WorldStates. WorldStates es un diccionario que contiene los estados clave-valor del mundo 
(como la energía del agente, su ubicación, etc.).
Métodos:
Instance: Permite acceder a la instancia única de GWorld. Gracias a este patrón, no importa en qué parte del código se encuentre, siempre accederás al mismo objeto de GWorld.
GetWorld: Devuelve el estado actual del mundo (un objeto WorldStates), que es donde se almacenan y gestionan todos los estados del mundo (como la energía del agente, su posición, etc.
Funcionalidad:
Acceso centralizado al estado del mundo: El objeto GWorld se utiliza para mantener un único punto de acceso global al estado del mundo. Esto es especialmente útil cuando se tiene
un sistema en el que múltiples agentes o entidades necesitan acceder y modificar el mismo conjunto de estados globales, sin necesidad de tener múltiples instancias del mundo.

Consistencia del estado global: Al ser un Singleton, GWorld garantiza que todo el sistema de juego o simulación esté trabajando con la misma instancia del mundo.
Esto es importante cuando el mundo es compartido por varios componentes (por ejemplo, agentes, NPCs, el entorno), para evitar inconsistencias.
 
 */
// Clase GWorld implementando el patrón Singleton para garantizar que haya solo una instancia global del mundo.
public sealed class GWorld
{
    // Instancia estática y única de la clase (Singleton)
    private static readonly GWorld instance = new GWorld();

    // Instancia de WorldStates que representa el estado global del mundo
    private static WorldStates world;

    // Constructor estático que inicializa el mundo cuando la clase se carga
    static GWorld()
    {
        world = new WorldStates(); 
    }

    // Constructor privado que impide que se creen instancias adicionales de la clase GWorld fuera de esta clase
    private GWorld()
    {
        // Este constructor se mantiene vacío, ya que no se necesita realizar ninguna inicialización adicional aquí.
    }

    // Propiedad estática que devuelve la instancia única de GWorld
    public static GWorld Instance
    {
        get { return instance; } // Devuelve la instancia única de GWorld
    }

    // Método público para obtener el estado actual del mundo (WorldStates)
    public WorldStates GetWorld()
    {
        return world; // Devuelve el objeto 'world', que contiene los estados actuales del mundo
    }
}
