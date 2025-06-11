using System;
using UnityEngine;

public class FarmResources : MonoBehaviour
{
    public TownResourcesTypes type;
    public TownHall hall;
    public int value;
    public Spawner spawner;
    public bool hasDestroyFX = false;

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
    }

    public void FarmResource()
    {
        hall.town_resources.AddResourceAmount(type, value);
        spawner.currentSpawned--;
        spawner.spawnedObjects.Remove(gameObject);
        if (!hasDestroyFX ) Destroy(gameObject);
    }
}