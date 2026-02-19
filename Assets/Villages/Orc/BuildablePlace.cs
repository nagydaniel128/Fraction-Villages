using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildablePlace : MonoBehaviour
{
    public bool hasBuilding;

    public Building buildingOnIt;

    public void Build<T>() where T : Building
    {
        switch(typeof(T).Name)
        {
            case "Temple":
                GameObject a = Instantiate(GameManager.instance.temple);
                a.transform.parent = transform;
                a.transform.localPosition = new Vector3(0, a.transform.localPosition.y, 0);

                buildingOnIt = a.GetComponent<Building>();
                hasBuilding = true;
                break;
        }
    }
}
