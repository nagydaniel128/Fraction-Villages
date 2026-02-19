using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAttack : Event
{
    List<HumanGroup> humangroups = new List<HumanGroup>();
    public override void CheckIfEnded()
    {
        bool allNull = true;
        for (int i = 0; i < humangroups.Count; i++)
        {
            if (humangroups[i] != null)
            {
                allNull = false;
                break;
            }
        }
        if (allNull)
            EndEvent();
    }

    public override void StartEvent()
    {
        Vector3 startPos = new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500));

        while (startPos.x > -240 && startPos.x < 240 && startPos.z > -240 && startPos.z < 240)
            startPos = new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500));

        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            GameObject a = GameObject.Instantiate(GameManager.instance.humanGroup);
            a.transform.position = startPos;
            a.transform.parent = EventHandler.instance.transform;

            humangroups.Add(a.GetComponent<HumanGroup>());

            a.GetComponent<HumanGroup>().wanderPoint = new Vector3(0, 0, 0);
            a.GetComponent<HumanGroup>().ChangeState(HumanGroup.states.wandering, false);
        }

        TakePlace();
    }
}
