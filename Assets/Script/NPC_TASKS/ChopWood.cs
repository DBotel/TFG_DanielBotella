using Mono.Cecil;
using UnityEngine;

public class ChopWood : GAction
{
    void Start()
    {
        
        preconditions.Add("HasAxe", 1);
        effects.Add("choppedTree", 1);
    }

    public override bool PrePerform()
    {
        // Verificamos si el agente tiene lo necesario para cortar madera
        return true;
    }

    public override bool PostPerform()
    {
        // Despu�s de realizar la tarea (cortar un �rbol), actualizamos el estado
        Debug.Log("�rbol talado");

        target.GetComponent<FarmResources>().FarmResource();
        return true;
    }
}
