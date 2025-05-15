using CodeMonkey.Utils;
using System;
using UnityEngine;

public class BuildBuilding : MonoBehaviour
{
    public PlacedObjectTypeSO building;
    public TownResourcesTypes first_resource;
    public int first_amount;
    public TownResourcesTypes second_resource;
    public int second_amount;

    public TownHall townHall;
    public GridBuildingSystem3D gridBuildingSystem;

    bool isSubscribed = false;
    public void SelectBuilding()
    {
        gridBuildingSystem.building = true;
        print("Selected");
        bool canAfford;
        if (second_amount == 0)
        {
            canAfford = townHall.CanAfford(
               first_resource,
               (int)first_amount,
               TownResourcesTypes.NULL,
               0,
               false);
        }
        else
        {
            canAfford = townHall.CanAfford(
               first_resource,
               (int)first_amount,
               second_resource,
               (int)second_amount,
               true
           );
        }

        if (canAfford)
        {
            ActivateAllConstructionObjects(false);
            gridBuildingSystem.placedObjectTypeSO = building;
            gridBuildingSystem.RefreshSelectedObjectType();
            if (!isSubscribed)
            {
                gridBuildingSystem.OnObjectPlaced += HandleOnObjectPlaced;
                isSubscribed = true;
            }
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Cannot Afford!", MouseUtils.GetMouseWorldPosition());
        }
    }
    private void HandleOnObjectPlaced(object sender, EventArgs e)
    {
        townHall.town_resources.SubtractResourceAmount(first_resource, first_amount);
        if (second_amount > 0)
            townHall.town_resources.SubtractResourceAmount(second_resource, second_amount);

        gridBuildingSystem.placedObjectTypeSO = null;

        gridBuildingSystem.OnObjectPlaced -= HandleOnObjectPlaced;
        isSubscribed = false;
        townHall.CallRefreshUI();
        ActivateAllConstructionObjects(true);
        Debug.Log("BuildBuilding: recursos descontados y modo construcci�n desactivado");
        gridBuildingSystem.building = false;

    }

    private void ActivateAllConstructionObjects(bool _active)
    {
        GameObject[] construcciones = GameObject.FindGameObjectsWithTag("ConstruccionEdificio");
        foreach (GameObject go in construcciones)
        {
            go.SetActive(_active);
        }
  
    }
}
