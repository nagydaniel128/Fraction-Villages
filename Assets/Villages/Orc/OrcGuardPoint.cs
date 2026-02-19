using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcGuardPoint : MonoBehaviour
{
    public SimpleOrc guardingOrc;

    public void SetOrcToGuard(SimpleOrc orc)
    {
        guardingOrc = orc;

        guardingOrc.guardPointToGuard = this;
        guardingOrc.ChangeState(SimpleOrc.states.guarding);
    }

    public void ReleaseOrc()
    {
        if (guardingOrc != null)
        {
            guardingOrc.ChangeState(SimpleOrc.states.wanderInVillage);
            guardingOrc.guardPointToGuard = null;
            guardingOrc = null;
        }
    }
}
