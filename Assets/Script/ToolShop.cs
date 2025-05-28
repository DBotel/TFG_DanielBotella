using UnityEngine;

public class ToolShop : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject pickaxePrefab;
    public GameObject spearPrefab;
    private WorldStates world = GWorld.Instance.GetWorld();
    public void BuyItem(string itemName, int amount)
    {
        int moneyAvailable = world.GetState("money");
        if(moneyAvailable>=amount)
        {
            world.ModifyState("money", -amount);
            AddItemToInventory(itemName);
        }
        else
        {
            Debug.LogWarning($"Not enough money to buy {amount} of {itemName}. Available: {moneyAvailable}, Required: {amount}");
        }
    }
    
    private void AddItemToInventory(string itemName)
    {
        var item = new GameObject("Empty: "+itemName);
        world.ModifyState("avariableTool" + itemName, 1);
        switch (itemName)
        {
            case "Axe":
                Instantiate(axePrefab, item.transform);
                break;
            case "Pickaxe":
                Instantiate(pickaxePrefab, item.transform);
                break;
            case "Spear":
                Instantiate(spearPrefab, item.transform);
                return;
        }
    }
}
