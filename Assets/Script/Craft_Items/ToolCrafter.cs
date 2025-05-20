using UnityEngine;
using System.Collections.Generic;

public class ToolCrafter : MonoBehaviour
{
    public CraftableItemSO[] recipes;    
    public Transform outputPoint;        

    public CraftableItemSO GetRecipe(string id)
    {
        foreach (var r in recipes)
            if (r.itemID == id) return r;
        return null;
    }
}