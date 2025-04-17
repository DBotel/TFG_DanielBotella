using UnityEngine;

public class ChopWood : GAction
{
    void Start()
    {
        preconditions.Add("HasAxe", 1);
        effects.Add("ChoppedTree", 1);
    }

    public override bool PrePerform()
    {
        // Verificamos si el agente tiene lo necesario para cortar madera
        return true;
    }

    public override bool PostPerform()
    {
        // Después de realizar la tarea (cortar un árbol), actualizamos el estado
        Debug.Log("Árbol talado");
        return true;
    }
}
