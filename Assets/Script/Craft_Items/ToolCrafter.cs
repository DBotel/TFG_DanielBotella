using UnityEngine;
using System.Collections.Generic;

public class ToolCrafter : MonoBehaviour
{
    public CraftableItemSO[] recipes;    // Qu� recetas acepta esta estaci�n
    public Transform outputPoint;        // D�nde instanciar el resultado

    // Buscar receta por ID
    public CraftableItemSO GetRecipe(string id)
    {
        foreach (var r in recipes)
            if (r.itemID == id) return r;
        return null;
    }
}