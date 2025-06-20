using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmResources : MonoBehaviour
{
    public TownResourcesTypes type;
    public TownHall hall;
    public int value;
    public Spawner spawner;
    public bool hasDestroyFX = false;

    public bool isTaken = false;
    public static event Action<FarmResources, bool, bool> OnTakenChanged;
    private void Start()
    {
        if (hall == null)
        {
            hall = FindObjectOfType<TownHall>();
            if (hall == null)
            {
                Debug.LogError("TownHall not found in the scene. Please assign it in the inspector or ensure it exists in the scene.");
            }
        }
        ResourceManager.Register(this);
        

    }
    
    void OnEnable()
    {
    }

    void OnDisable()
    {
        ResourceManager.Unregister(this);
    }

    // Llamar siempre que cambies isTaken
    public void SetTaken(bool taken)
    {
        if (isTaken == taken) return;
        bool old = isTaken;
        isTaken = taken;
        OnTakenChanged?.Invoke(this, old, isTaken);
    }

    public void FarmResource()
    {
        hall.town_resources.AddResourceAmount(type, value);
        spawner.currentSpawned--;
        spawner.spawnedObjects.Remove(gameObject);
        if (!hasDestroyFX ) Destroy(gameObject);
    }
}

public static class ResourceManager
{
    static List<FarmResources> freeNodes = new List<FarmResources>();

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        foreach (var fr in UnityEngine.Object.FindObjectsOfType<FarmResources>())
            Register(fr);
    }

    public static void Register(FarmResources fr)
    {
        if (!fr.isTaken && !freeNodes.Contains(fr))
            freeNodes.Add(fr);
        FarmResources.OnTakenChanged += OnTakenChanged;
    }

    public static void Unregister(FarmResources fr)
    {
        freeNodes.Remove(fr);
        FarmResources.OnTakenChanged -= OnTakenChanged;
    }

    static void OnTakenChanged(FarmResources fr, bool oldTaken, bool newTaken)
    {
        if (newTaken)      freeNodes.Remove(fr);
        else if (!freeNodes.Contains(fr))
            freeNodes.Add(fr);
    }

    public static FarmResources GetNearest(Vector3 from, string resourceTag)
    {
        FarmResources best     = null;
        float         bestDist = float.MaxValue;

        for (int i = 0; i < freeNodes.Count; i++)
        {
            var node = freeNodes[i];

            if (!string.IsNullOrEmpty(resourceTag)
                && !node.gameObject.CompareTag(resourceTag))
                continue;

            float d = (node.transform.position - from).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best     = node;
            }
        }

        return best;
    }
}