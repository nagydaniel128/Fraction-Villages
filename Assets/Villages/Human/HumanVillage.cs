using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanVillage : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < Random.Range(5, 10); i++)
        {
            GameObject a = Instantiate(GameManager.instance.human);

            a.transform.position = transform.position + new Vector3(Random.Range(-40, 40), 0, Random.Range(-40, 40));
            a.transform.parent = transform;
        }
    }
}
