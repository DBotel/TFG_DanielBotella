using UnityEngine;
using UnityEngine.Serialization;

public class ToolShop : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject pickaxePrefab;
    public GameObject weaponPrefab;
    public GameObject shieldPrefab;
    public GameObject cementoPrefab;
    private WorldStates world = GWorld.Instance.GetWorld();

    public Transform axeStand;
    public Transform pickaxeStand;
    public Transform weaponStand;
    public Transform shieldStand;
    public Transform cementoStand;
    
    public void BuyItem(string itemName, int amount)
    {
        int moneyAvailable = world.GetState("money");
        if(moneyAvailable>=amount)
        {
            world.ModifyState("money", -amount);
            Debug.Log($"Bought {itemName} for {amount}. Remaining money: {world.GetState("money")}");
            AddItemToInventory(itemName);
        }
        else
        {
            Debug.LogWarning($"Not enough money to buy {itemName}. Available: {moneyAvailable}, Required: {amount}");
        }
    }
    
    private void AddItemToInventory(string itemName)
    {
        Debug.Log($"Adding {itemName} to inventory.");
        world.ModifyState("avariableTool" + itemName, 1);
        switch (itemName)
        {
            case "Axe":
                Instantiate(axePrefab, axeStand);
                Debug.Log("Axe added to inventory.");
                break;
            case "Pickaxe":
                Instantiate(pickaxePrefab, pickaxeStand);
                break;
            case "Weapon":
                Instantiate(weaponPrefab, weaponStand);
                return;
            case "Shield":
                Instantiate(shieldPrefab, shieldStand);
                return;
            case "Cemento":
                Instantiate(cementoPrefab, cementoStand);
                return;
        }
    }
    
    public void BuyAxe(int amount)
    {
        BuyItem("Axe", amount);
    }
    public void BuyPickAxe(int amount)
    {
        BuyItem("Pickaxe", amount);
    }
    public void BuyWeapon(int amount)
    {
        BuyItem("Weapon", amount);
    }
    public void BuyShield(int amount)
    {
        BuyItem("Shield", amount);
    }
    public void BuyCemento(int amount)
    {
        BuyItem("Cemento", amount);
    }

}
