using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : Event
{
    List<GoblinGroup> goblingroups = new List<GoblinGroup>();
    public override void CheckIfEnded()
    {
        bool allNull = true;
        for (int i = 0; i < goblingroups.Count; i++)
        {
            if (goblingroups[i] != null)
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
            GameObject a = GameObject.Instantiate(GameManager.instance.goblinGroup);
            a.transform.position = startPos;
            a.transform.parent = EventHandler.instance.transform;

            goblingroups.Add(a.GetComponent<GoblinGroup>());

            a.GetComponent<GoblinGroup>().wanderPoint = new Vector3(0, 0, 0);
            a.GetComponent<GoblinGroup>().ChangeState(GoblinGroup.states.wandering, false);
        }

        TakePlace();
    }
}
