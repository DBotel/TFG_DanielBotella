using UnityEngine;

public class TownHall : MonoBehaviour
{
    public int town_id;
    public string town_name;
    public Town_Resources town_resources;
    public GameUI gameUI;

    private void Awake()    
    {
        town_resources = new Town_Resources();
        town_resources.TownHall = this;

        //setear variables de prueba
        town_resources.SetResourceAmount(TownResourcesTypes.WOOD, 100);
        town_resources.SetResourceAmount(TownResourcesTypes.STONE, 50);
        town_resources.SetResourceAmount(TownResourcesTypes.MONEY, 200);

        Debug.Log("TownHall: recursos inicializados");

    }
    public bool CanAfford(TownResourcesTypes firstType, int firstAmount, TownResourcesTypes secondType, int secondAmount, bool two)
    {
        int availableFirst = town_resources.TownResources_All[firstType];
        bool canAfford;

        if (two)
        {
            int availableSecond = town_resources.TownResources_All[secondType];
            canAfford = (availableFirst >= firstAmount) && (availableSecond >= secondAmount);
        }
        else
        {
            canAfford = (availableFirst >= firstAmount);
        }
        return canAfford;
    }

    public void CallRefreshUI()
    {
        gameUI.RefreshUI();
    }
}