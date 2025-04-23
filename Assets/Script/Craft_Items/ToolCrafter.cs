using UnityEngine;
using System.Collections.Generic;

public class ToolCrafter : MonoBehaviour
{
    public CraftableItemSO[] recipes;    // Qué recetas acepta esta estación
    public Transform outputPoint;        // Dónde instanciar el resultado

    // Buscar receta por ID
    public CraftableItemSO GetRecipe(string id)
    {
        foreach (var r in recipes)
            if (r.itemID == id) return r;
        return null;
    }
}