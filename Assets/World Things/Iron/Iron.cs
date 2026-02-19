using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron : MonoBehaviour
{
    public void HitIron(SimpleOrc orc)
    {
        orc.carriedAmount += (int)(orc.character.stats.damage * 0.1f);
    }
}
