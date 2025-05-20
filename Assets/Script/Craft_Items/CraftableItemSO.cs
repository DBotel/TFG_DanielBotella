using UnityEngine;

[CreateAssetMenu( menuName = "Craftable/Item")]
public class CraftableItemSO : ScriptableObject
{
    public string itemID;      
    public GameObject prefab;    
    public float craftTime = 5f; 

    [System.Serializable]
    public struct Cost
    {
        public TownResourcesTypes type;
        public int amount;
    }
    public Cost[] costs;         
}
