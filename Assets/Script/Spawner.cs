using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public BoxCollider spawnArea;
    public int timeToSpawn = 5;
    public int maxToSpawn = 10;
    public int currentSpawned = 0;
    
    public int maxAttemptsPerSpawn = 10;
    public float minDistanceBetweenObjects = 1.5f;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, timeToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void TrySpawn()
    {
        if(currentSpawned>= maxToSpawn) return;
        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            Vector3 randomPos = GetRandomPointInBounds(spawnArea.bounds);

            if (IsPositionFree(randomPos))
            {
                currentSpawned++;
                GameObject obj = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
                spawnedObjects.Add(obj);
                obj.GetComponent<FarmResources>().spawner = this;
                break;
            }
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    bool IsPositionFree(Vector3 pos)
    {
        foreach (var obj in spawnedObjects)
        {
            if (Vector3.Distance(pos, obj.transform.position) < minDistanceBetweenObjects)
                return false;
        }
        return true;
    }
}
