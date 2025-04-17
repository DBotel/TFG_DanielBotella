using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TownResourcesTypes
{
    NULL,
    WOOD,
    STONE,
    MONEY
}
public class Town_Resources 
{
    public Dictionary<TownResourcesTypes, int> TownResources_All;
    public int Wood { get; private set; }
    public int Stone { get; private set; }
    public int Money { get; private set; }

    private int nextId;

    public void SetAllTownResources(Dictionary<TownResourcesTypes, int> savedResources)
    {

    }
    public void SetTownResources()
    {
        TownResources_All[TownResourcesTypes.WOOD] = 0;
        TownResources_All[TownResourcesTypes.STONE] = 0;
        TownResources_All[TownResourcesTypes.MONEY] = 0;
    }
    public Town_Resources()
    {
        TownResources_All = new Dictionary<TownResourcesTypes, int>();
        SetTownResources();


        nextId = 0;

    }

    private int GetNextId()
    {
        return nextId++;
    }
    public void SetResourceAmount(TownResourcesTypes _type, int _amount)
    {
        TownResources_All[_type] = _amount;
        SyncProperty(_type, _amount);
    }

    public void SubtractResourceAmount(TownResourcesTypes _type, int _amount)
    {
        int current = TownResources_All[_type];
        int updated = Mathf.Max(0, current - _amount);
        TownResources_All[_type] = updated;
        SyncProperty(_type, updated);
    }
    private void SyncProperty(TownResourcesTypes type, int amount)
    {
        switch (type)
        {
            case TownResourcesTypes.WOOD: Wood = amount; break;
            case TownResourcesTypes.STONE: Stone = amount; break;
            case TownResourcesTypes.MONEY: Money = amount; break;
        }
    }

    public TownResourcesTypes GetEnumFromID(int _id)
    {
        return (TownResourcesTypes)_id;
    }
}


public class TownsManager
{
    public TownHall[] towns;
    public int numberOfTowns = 100;

    public TownsManager()
    {
        towns = new TownHall[numberOfTowns];
    }

    public void AddTownHall(int _id, TownHall town)
    {
        towns[_id] = town;
    }
}


