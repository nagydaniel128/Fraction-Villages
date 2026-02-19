using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterAnimationHandler animationHandler;
    public CharacterMovementHandler movementHandler;
    public InputManager inputManager;
    public Stats stats;
    public CharacterControllerScript controller;
    public BoxCollider bodyArea;

    public List<Character> enemiesTargetingThis = new List<Character>();

    bool HasShield = true;
    public bool hasShield
    {
        get { return HasShield; }
        set
        {
            HasShield = value;

            transform.GetChild(0).GetChild(1).gameObject.SetActive(HasShield);

            if (HasShield)
                durability = 50;
        }
    }
    float Durability = 10;
    public float durability
    {
        get { return Durability; }
        set
        {
            Durability = value;

            if (Durability <= 0)
                hasShield = false;
        }
    }



    public void DealDamageTo(Character otherCharacter, float damage, bool pierceAttack = false, bool stunAttack = false, float stunDuration = 0)
    {
        if (pierceAttack)
        {
            otherCharacter.TakeDamage(damage);
            return;
        }

        //if other character is not dodging
        if (otherCharacter.controller.currentAbiity == null)
            //if blocking
            if (otherCharacter.movementHandler.state == CharacterMovementHandler.movementState.blocking)
            {
                Vector3 direction = transform.position - otherCharacter.transform.position;
                float angle = Vector3.Angle(otherCharacter.transform.forward, direction);

                if (angle < 90)
                {
                    //blocked
                    if (otherCharacter.hasShield)
                        otherCharacter.durability -= damage;
                    else
                    {
                        if (Random.Range(0, 100) < otherCharacter.stats.blockSkill * 10 + 40)
                        {
                            //blocked
                            print("blocked");
                        }
                        else
                        {
                            otherCharacter.TakeDamage(damage);
                            if (stunAttack)
                                otherCharacter.movementHandler.StartStun(stunDuration);
                        }
                    }
                }
                else
                {
                    otherCharacter.TakeDamage(damage);
                    if (stunAttack)
                        otherCharacter.movementHandler.StartStun(stunDuration);
                }
            }
            else
            {
                otherCharacter.TakeDamage(damage);
                if (stunAttack)
                    otherCharacter.movementHandler.StartStun(stunDuration);
            }

        //target died
        if (otherCharacter.stats.hp <= 0)
        {
            stats.xp++;
        }
    }

    void TakeDamage(float damage)
    {
        stats.hp -= damage;

        if (stats.hp <= 0)
            controller.Die();
        else
            StartCoroutine(movementHandler.StartHit());
    }
}
