using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int level = 1;
    public float maxHp;
    [SerializeReference]
    float Hp = 100;
    public float hp
    {
        get { return Hp; }
        set
        {
            Hp = value;

            if (Hp > maxHp)
                Hp = maxHp;
        }
    }

    int Xp = 0;
    public int xp
    {
        get { return Xp; }
        set
        {
            Xp = value;

            if(Xp == 2)
            {
                level++;
                print(level);
                damage += 10;
                maxHp += 20;
                blockSkill++;
                attackSpeed += 0.2f;

                transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

                Xp = 0;
            }
        }
    }

    public float damage;
    float AttackSpeed = 1f;
    public float attackSpeed
    {
        get { return AttackSpeed; }
        set
        {
            AttackSpeed = value;

            GetComponent<CharacterAnimationHandler>().bodyAnimator.SetFloat("attackMultiplier", AttackSpeed);
        }
    }

    public float blockSkill;
    public float speed;
}
