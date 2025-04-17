using System.Collections.Generic;
using UnityEngine;

public class GInventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject item)
    {
        items.Add(item);
    }

    public bool HasItemTag(string tag)
    {
        foreach (GameObject item in items)
        {
            if (item.CompareTag(tag))
                return true;
        }
        return false;
    }
}