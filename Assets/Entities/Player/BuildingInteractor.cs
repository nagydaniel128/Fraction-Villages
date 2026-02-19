using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInteractor : MonoBehaviour
{
    public Building nearestBuilding;
    public BuildablePlace targetedBuildingPlace;
    public BeaconPlace targetedBeaconPlace;

    private void Update()
    {
        FindNearestBuildingPlace();
        FindNearestBeaconPlace();

        Build();
    }

    void FindNearestBuildingPlace()
    {
        bool yes = false;
        for (int i = 0; i < OrcVillage.instance.buildablePlaces.Length; i++)
        {
            if (Vector3.Distance(transform.position, OrcVillage.instance.buildablePlaces[i].transform.position) < 10)
            {
                targetedBuildingPlace = OrcVillage.instance.buildablePlaces[i];
                yes = true;
            }
        }
        if (!yes)
            targetedBuildingPlace = null;
    }
    void FindNearestBeaconPlace()
    {
        bool yes = false;
        for (int i = 0; i < GameManager.instance.ironGroups.Count; i++)
        {
            if (Vector3.Distance(transform.position, GameManager.instance.ironGroups[i].beaconPlace.transform.position) < 10)
            {
                targetedBeaconPlace = GameManager.instance.ironGroups[i].beaconPlace;
                yes = true;
            }
        }
        if (!yes)
            targetedBeaconPlace = null;
    }
    void Build()
    {
        //village building
        if(targetedBuildingPlace != null)
        {
            if(!targetedBuildingPlace.hasBuilding)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    targetedBuildingPlace.Build<Temple>();
                }
            }
        }

        //beacon building
        if (targetedBeaconPlace != null)
        {
            if (!targetedBeaconPlace.hasBuilding)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    targetedBeaconPlace.Build<OrcBeacon>();
                }
            }
        }
    }
}
