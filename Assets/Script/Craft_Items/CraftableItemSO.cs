using UnityEngine;

[CreateAssetMenu( menuName = "Craftable/Item")]
public class CraftableItemSO : ScriptableObject
{
    public string itemID;      // ej. "Arrow", "Sword"
    public GameObject prefab;    // Prefab que se coloca al terminar
    public float craftTime = 5f; // Segundos

    [System.Serializable]
    public struct Cost
    {
        public TownResourcesTypes type;
        public int amount;
    }
    public Cost[] costs;         // Lista de recursos necesarios
}
