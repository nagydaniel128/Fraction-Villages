using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementHandler : MonoBehaviour
{
    public float speed = 0;
    public Character character;
    public NavMeshAgent agent;

    [SerializeField]
    public movementState state = movementState.nothing;
    public enum movementState
    {
        nothing,
        attacking,
        blocking,
        usingAbility,
        hit,
        stunned
    }

    void Update()
    {
        ChangeState();

        switch (state)
        {
            case movementState.nothing:
                MoveToDirection();
                RotateToAngle();
                break;
            case movementState.attacking:
                MoveToDirection();
                RotateToAngle();
                Attacking();
                break;
            case movementState.blocking:
                MoveToDirection();
                RotateToAngle();
                Block();
                break;
            case movementState.hit:
                //nothing to do
            case movementState.stunned:
                //nothing to do
                break;
            case movementState.usingAbility:
                character.controller.currentAbiity.UseAbility();
                RotateToAngle();
                break;
        }
    }


    void ChangeState()
    {
        if (state == movementState.hit || state == movementState.stunned)
            return;

        if (character.inputManager.attackPressed && state != movementState.usingAbility)
        {
            state = movementState.attacking;
            return;
        }

        if (character.controller.currentAbiity != null)
        {
            attackStarted = false;
            state = movementState.usingAbility;
            return;
        }

        if (character.inputManager.blockPressed && state != movementState.attacking && state != movementState.usingAbility)
        {
            state = movementState.blocking;
            return;
        }

        if (state != movementState.attacking && state != movementState.usingAbility)
            state = movementState.nothing;
    }

    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public void RotateToAngle()
    {
        if (!character.inputManager.shouldTravelToTravelPoint)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, character.inputManager.lookAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }
    public void MoveToDirection()
    {
        if(character.inputManager.shouldTravelToTravelPoint)
        {
            //if should move
            if (character.inputManager.desiredTravelPoint != new Vector3(0,0,0))
            {
                if (speed < character.stats.speed / 2)
                    speed = character.stats.speed / 2;

                if (speed < character.stats.speed)
                    speed += (character.stats.speed / 5) * Time.deltaTime;

                character.animationHandler.SetRunAnimation(state == movementState.nothing ? false : true);

                agent.isStopped = false;
            }
            else
            {
                if (speed > 0)
                    speed -= (character.stats.speed / 5) * 10 * Time.deltaTime;

                character.animationHandler.SetIdleAnimation(state == movementState.nothing ? false : true);

                agent.isStopped = true;
            }

            if (speed < 0)
                speed = 0;

            agent.speed = speed;
            agent.SetDestination(character.inputManager.desiredTravelPoint);
        }
        else
        {
            agent.isStopped = true;

            //if should move
            if (character.inputManager.moveDirection != new Vector3(0, 0, 0))
            {
                if (speed < character.stats.speed / 2)
                    speed = character.stats.speed / 2;

                if (speed < character.stats.speed)
                    speed += (character.stats.speed / 5) * Time.deltaTime;

                character.animationHandler.SetRunAnimation(state == movementState.nothing ? false : true);
            }
            else
            {
                if (speed > 0)
                    speed -= (character.stats.speed / 5) * 10 * Time.deltaTime;

                character.animationHandler.SetIdleAnimation(state == movementState.nothing ? false : true);
            }

            if (speed < 0)
                speed = 0;

            agent.Move(character.inputManager.moveDirection * speed * Time.deltaTime);
        }
    }

    bool attackStarted;
    void Attacking()
    {
        if (!attackStarted)
            StartCoroutine(StartSwinging());
    }
    IEnumerator StartSwinging()
    {
        character.animationHandler.SetAttackAnimation();
        attackStarted = true;

        yield return new WaitForSeconds(1f / character.stats.attackSpeed);

        if (state != movementState.hit && state != movementState.stunned)
        {
            state = movementState.nothing;
            DealDamage();
            character.animationHandler.SetIdleAnimation(false);
        }
        attackStarted = false;
    }
    public BoxCollider damageArea;
    void DealDamage()
    {
        switch(tag)
        {
            case "Orc":
                for (int i = 0; i < GameManager.instance.elves.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.elves[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.elves[i].character, character.stats.damage);
                }
                for (int i = 0; i < GameManager.instance.humans.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.humans[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.humans[i].character, character.stats.damage);
                }
                for (int i = 0; i < GameManager.instance.goblins.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.goblins[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.goblins[i].character, character.stats.damage);
                }
                break;
            case "Elf":
                if (damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    character.DealDamageTo(Player.instance.character, character.stats.damage);

                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.orcs[i].character, character.stats.damage);
                }
                break;
            case "Goblin":
                if (damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    character.DealDamageTo(Player.instance.character, character.stats.damage);

                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.orcs[i].character, character.stats.damage);
                }
                break;
            case "Human":
                if (damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    character.DealDamageTo(Player.instance.character, character.stats.damage);

                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        character.DealDamageTo(GameManager.instance.orcs[i].character, character.stats.damage);
                }
                break;
        }
    }


    void Block()
    {
        character.animationHandler.SetBlockAnimation();
    }

    public IEnumerator StartHit()
    {
        state = movementState.hit;
        character.animationHandler.SetHitAnimation();
        agent.isStopped = true;

        yield return new WaitForSeconds(1.5f);

        if (state != movementState.stunned)
        {
            state = movementState.nothing;
            agent.isStopped = false;
        }
    }
    public IEnumerator StartStunEnumerator(float duration)
    {
        state = movementState.stunned;
        character.animationHandler.SetStunnedAnimation();
        agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        agent.isStopped = false;
        state = movementState.nothing;
    }
    public void StartStun(float duration)
    {
        StartCoroutine(StartStunEnumerator(duration));
    }

}
