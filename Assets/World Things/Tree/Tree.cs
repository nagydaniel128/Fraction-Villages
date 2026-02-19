using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public Animator animator;

    [SerializeField]
    int Hp = 100;
    int hp
    {
        get { return Hp; }
        set
        {
            Hp = value;
            if (Hp <= 0)
            {
                choppedDown = true;

                animator.SetBool("down", true);
            }
        }
    }

    [SerializeField]
    bool choppedDown = false;
    [SerializeField]
    int resourcesLeft = 100;


    public void HitTree(SimpleOrc orc)
    {
        if(!choppedDown)
            hp -= (int)orc.character.stats.damage;
        else
        {
            resourcesLeft -= (int)orc.character.stats.damage;

            if (resourcesLeft <= 0)
            {
                //if resourcesLeft is -5 then orc gets the damage (e.g. 20) but add -5 to it (15)
                orc.carriedAmount += (int)orc.character.stats.damage + resourcesLeft;

                //delete
                GameManager.instance.trees.Remove(this);
                Destroy(gameObject);
            }
            else
                orc.carriedAmount += 5;
        }

    }


    private void Start()
    {
        GameManager.instance.trees.Add(this);
    }
}
