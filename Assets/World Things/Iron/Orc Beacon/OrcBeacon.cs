using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcBeacon : Beacon
{
    public int maxOrcsToAssign = 2;
    public int orcsAssignedHere;

    public IronGroup ironGroup;

    void AssignOrcsHere()
    {
        if (maxOrcsToAssign > orcsAssignedHere)
        {
            for (int i = 0; i < OrcVillage.instance.simpleOrcs.Count; i++)
            {
                if (OrcVillage.instance.simpleOrcs[i].state == SimpleOrc.states.wanderInVillage)
                {
                    OrcVillage.instance.simpleOrcs[i].ChangeState(SimpleOrc.states.gatheringIron);
                    OrcVillage.instance.simpleOrcs[i].beacon = this;
                    orcsAssignedHere++;
                }

                if (orcsAssignedHere >= maxOrcsToAssign)
                    break;
            }
        }
    }

    private void Update()
    {
        AssignOrcsHere();
    }

    private void Start()
    {
        OrcVillage.instance.ownedIronCamps.Add(this);
    }
}
