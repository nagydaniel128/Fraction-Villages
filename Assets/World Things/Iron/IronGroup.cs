using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronGroup : MonoBehaviour
{
    public Iron[] irons;

    public BeaconPlace beaconPlace;

    private void Start()
    {
        irons = new Iron[Random.Range(4, 6)];

        for (int i = 0; i < irons.Length; i++)
        {
            GameObject o = Instantiate(GameManager.instance.iron);
            o.transform.parent = transform;
            o.transform.localPosition = new Vector3(Random.Range(-20, 20), o.transform.localPosition.y, Random.Range(-20, 20));

            irons[i] = o.GetComponent<Iron>();
        }

        GameManager.instance.ironGroups.Add(this);
    }
}
