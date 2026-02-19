using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconPlace : MonoBehaviour
{
    public bool hasBuilding;

    public Beacon buildingOnIt;

    public IronGroup owner;

    public void Build<T>() where T : Beacon
    {
        switch (typeof(T).Name)
        {
            case "OrcBeacon":
                GameObject a = Instantiate(GameManager.instance.orcBeacon);
                a.transform.parent = transform;
                a.transform.localPosition = new Vector3(0, a.transform.localPosition.y, 0);

                buildingOnIt = a.GetComponent<Beacon>();
                (buildingOnIt as OrcBeacon).ironGroup = owner;
                hasBuilding = true;
                break;
        }
    }
}
