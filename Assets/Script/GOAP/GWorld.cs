using UnityEngine;
/*---------------------------------------- EXPLICACION ------------------------------------------------
 Patr�n Singleton:

Prop�sito: La clase GWorld utiliza el patr�n Singleton para garantizar que solo haya una instancia de la clase en toda la aplicaci�n. Esto es �til cuando se necesita un acceso
centralizado y �nico a la informaci�n global, como el estado del mundo en este caso.
C�mo funciona:
La instancia est�tica instance es la �nica instancia de la clase GWorld.
El constructor est�tico static GWorld() se encarga de inicializar la instancia de world (el estado global del mundo) la primera vez que se accede a la clase.
El constructor privado private GWorld() evita que otras clases creen instancias adicionales de GWorld. La �nica forma de acceder a la instancia es a trav�s de la propiedad est�tica
Instance.

Manejo del mundo:

Prop�sito: La clase GWorld contiene el estado global del mundo a trav�s del objeto WorldStates. WorldStates es un diccionario que contiene los estados clave-valor del mundo 
(como la energ�a del agente, su ubicaci�n, etc.).
M�todos:
Instance: Permite acceder a la instancia �nica de GWorld. Gracias a este patr�n, no importa en qu� parte del c�digo se encuentre, siempre acceder�s al mismo objeto de GWorld.
GetWorld: Devuelve el estado actual del mundo (un objeto WorldStates), que es donde se almacenan y gestionan todos los estados del mundo (como la energ�a del agente, su posici�n, etc.
Funcionalidad:
Acceso centralizado al estado del mundo: El objeto GWorld se utiliza para mantener un �nico punto de acceso global al estado del mundo. Esto es especialmente �til cuando se tiene
un sistema en el que m�ltiples agentes o entidades necesitan acceder y modificar el mismo conjunto de estados globales, sin necesidad de tener m�ltiples instancias del mundo.

Consistencia del estado global: Al ser un Singleton, GWorld garantiza que todo el sistema de juego o simulaci�n est� trabajando con la misma instancia del mundo.
Esto es importante cuando el mundo es compartido por varios componentes (por ejemplo, agentes, NPCs, el entorno), para evitar inconsistencias.
 
 */
// Clase GWorld implementando el patr�n Singleton para garantizar que haya solo una instancia global del mundo.
public sealed class GWorld
{
    // Instancia est�tica y �nica de la clase (Singleton)
    private static readonly GWorld instance = new GWorld();

    // Instancia de WorldStates que representa el estado global del mundo
    private static WorldStates world;

    // Constructor est�tico que inicializa el mundo cuando la clase se carga
    static GWorld()
    {
        world = new WorldStates(); 
    }

    // Constructor privado que impide que se creen instancias adicionales de la clase GWorld fuera de esta clase
    private GWorld()
    {
        // Este constructor se mantiene vac�o, ya que no se necesita realizar ninguna inicializaci�n adicional aqu�.
    }

    // Propiedad est�tica que devuelve la instancia �nica de GWorld
    public static GWorld Instance
    {
        get { return instance; } // Devuelve la instancia �nica de GWorld
    }

    // M�todo p�blico para obtener el estado actual del mundo (WorldStates)
    public WorldStates GetWorld()
    {
        return world; // Devuelve el objeto 'world', que contiene los estados actuales del mundo
    }
}
