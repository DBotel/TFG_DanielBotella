using System;
using System.Collections.Generic;
using System.IO;
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
    public List<GAgent> agents;
    public List<Transform> patrolPoints;

    public List<GameObject> buildings;
    public void SetStates()
    {
        world.SetState("town_resources_Wood", town_resources.TownResources_All[TownResourcesTypes.WOOD]);
        world.SetState("town_resources_Stone", town_resources.TownResources_All[TownResourcesTypes.STONE]);
        world.SetState("food", town_resources.TownResources_All[TownResourcesTypes.FOOD]);
        world.SetState("money", town_resources.TownResources_All[TownResourcesTypes.MONEY]);
        world.SetState("unemployedRate", unemployedRate);
    }
    public void UpdateStates()
    {
        world.ModifyState("town_resources_Stone", town_resources.TownResources_All[TownResourcesTypes.STONE]);
        world.ModifyState("food", town_resources.TownResources_All[TownResourcesTypes.FOOD]);
        world.ModifyState("money", town_resources.TownResources_All[TownResourcesTypes.MONEY]);
        world.ModifyState("unemployedRate", unemployedRate);
        world.ModifyState("town_resources_Wood", town_resources.TownResources_All[TownResourcesTypes.WOOD]);
    }


    private void Start()
    {
        InvokeRepeating(nameof(UpdateStates), 0f, 10f);
    }

    public void SetNPCS()
    {
        agents = new List<GAgent>();
        foreach (var agent in FindObjectsOfType<GAgent>())
        {
            if (agent.town == this)
            {
                agents.Add(agent);
            }
            else // esto es provisinal porque solo van a tener una ciudad
            {
                agents.Add(agent);
                agent.town = this;
            }
        }
    }

    public void AddNPC(GAgent npc)
    {
        
        agents.Add(npc);
        npc.town = this;
    }

    public List<GAgent> GetNPC()
    {
        return agents;
    }
    private void Awake()    
    {
        town_resources = new Town_Resources();
        town_resources.TownHall = this;

        town_resources.SetResourceAmount(TownResourcesTypes.WOOD, 100);
        town_resources.SetResourceAmount(TownResourcesTypes.STONE, 80);
        town_resources.SetResourceAmount(TownResourcesTypes.MONEY, 200);

        Debug.Log("TownHall: recursos inicializados");
        
        gameUI= GameObject.Find("UI").GetComponent<GameUI>();
        CallRefreshUI();
        
        world = GWorld.Instance.GetWorld();
        SetNPCS();
        
        SetStates();
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