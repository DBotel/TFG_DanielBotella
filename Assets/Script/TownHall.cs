using UnityEngine;

public class TownHall : MonoBehaviour
{
    public int town_id;
    public string town_name;
    public Town_Resources town_resources;
    public GameUI gameUI;

    private int food;
    private int townMoney;
    private int unemployedRate;

    private WorldStates world;
    public void SetStates()
    {
        world.SetState("town_resources_Wood", town_resources.TownResources_All[TownResourcesTypes.WOOD]);
        world.SetState("town_resources_Stone", town_resources.TownResources_All[TownResourcesTypes.STONE]);
        world.SetState("food", food);
        world.SetState("money", townMoney);
        world.SetState("unemployedRate", unemployedRate);
    }
    public void UpdateStates()
    {
        world.ModifyState("town_resources_Stone", town_resources.TownResources_All[TownResourcesTypes.STONE]);
        world.ModifyState("food", food);
        world.ModifyState("money", food);
        world.ModifyState("unemployedRate", unemployedRate);
        world.ModifyState("town_resources_Wood", town_resources.TownResources_All[TownResourcesTypes.WOOD]);
    }
    private void Awake()    
    {
        town_resources = new Town_Resources();
        town_resources.TownHall = this;

        town_resources.SetResourceAmount(TownResourcesTypes.WOOD, 100);
        town_resources.SetResourceAmount(TownResourcesTypes.STONE, 50);
        town_resources.SetResourceAmount(TownResourcesTypes.MONEY, 200);

        Debug.Log("TownHall: recursos inicializados");
        
        gameUI= GameObject.Find("UI").GetComponent<GameUI>();
        CallRefreshUI();
        
        world = GWorld.Instance.GetWorld();

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
       if(gameUI)gameUI.RefreshUI();
    }
}