using System.Collections.Generic;
using UnityEngine;

public class GBeliefs : MonoBehaviour
{
    public Dictionary<string, int> states = new Dictionary<string, int>();

    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key))
            states[key] += value;
        else
            states.Add(key, value);
    }

    public int GetState(string key)
    {
        if (states.ContainsKey(key))
            return states[key];
        return 0;
    }

    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
            states.Remove(key);
    }
}