using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GInventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject item)
    {
        items.Add(item);
        Debug.LogWarning("added item to inventory : " + item);
    }

    public void RemoveItem(GameObject item)
    {
        items.Remove(item);
        Debug.LogWarning("removed item of inventory : " + item);
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

    public bool HasTool(string toolTag)
    {
        foreach (GameObject tool in items)
        {
            if (tool.CompareTag(toolTag))
                return true;
        }
        return false;
    }
}