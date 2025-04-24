using UnityEngine;

public class FarmResources : MonoBehaviour
{
    public TownResourcesTypes type;
    public TownHall hall;
    public int value;

    public bool hasDestroyFX = false;
    public void FarmResource()
    {
        hall.town_resources.AddResourceAmount(type, value);
        if (!hasDestroyFX ) Destroy(gameObject);
    }
}