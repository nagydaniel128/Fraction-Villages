using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : Building
{
    public float healingPower = 5f;
    float timer;
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 3)
        {
            timer = 0;

            for (int i = 0; i < GameManager.instance.orcs.Count; i++)
            {
                if (Vector3.Distance(transform.position, GameManager.instance.orcs[i].character.transform.position) < 10)
                {
                    GameManager.instance.orcs[i].character.stats.hp += healingPower;
                }
            }
        }
    }
}
