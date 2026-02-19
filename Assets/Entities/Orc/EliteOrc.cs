using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteOrc : Orc
{
    private void Start()
    {
        GameManager.instance.orcs.Add(this);

        StartCoroutine(TryToLookForClosestEnemySometimes());

        //skills
        dodge = new Dodge(this);
        stunAttack = new StunAttack(this);
        shieldPiercingAttack = new ShieldPiercingAttack(this);
    }

    private void Update()
    {
        if (isFighting)
            Fight();
        else
            Follow();
    }



    const float attackRange = 2.3f;
    float attackTimer;
    float attackTimerMax;
    bool sidleRight;
    bool standing;
    bool rePositioning;
    bool goesBackwards;
    public enum attackStates
    {
        approaching,
        attacking,
        defending
    }
    public attackStates attackState = attackStates.approaching;
    //skills
    Dodge dodge;
    StunAttack stunAttack;
    ShieldPiercingAttack shieldPiercingAttack;
    bool isTargetedBySomeone()
    {
        return character.enemiesTargetingThis.Count > 0;
    }
    void Fight()
    {
        character.inputManager.shouldTravelToTravelPoint = false;

        if (targetedEnemy == null)
        {
            LookForEnemy();
            if (targetedEnemy == null)
            {
                isFighting = false;
                character.inputManager.shouldTravelToTravelPoint = true;
                character.inputManager.attackPressed = false;
                character.inputManager.blockPressed = false;

                character.inputManager.moveDirection = new Vector3(0, 0, 0);

                return;
            }
        }

        //if dodging then don't dodge
        if (dodge.abilityInputPressed)
            dodge.abilityInputPressed = false;
        if (stunAttack.abilityInputPressed)
            stunAttack.abilityInputPressed = false;
        if (shieldPiercingAttack.abilityInputPressed)
            shieldPiercingAttack.abilityInputPressed = false;

        switch (attackState)
        {
            #region approach
            case attackStates.approaching:
                MoveIntelligentlyToPoint(targetedEnemy.transform.position);
                character.inputManager.attackPressed = false;

                if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 3)
                {
                    if (!isTargetedBySomeone())
                        attackState = attackStates.attacking;
                    else
                    {
                        attackState = attackStates.defending;

                        MoveTowardsPoint(targetedEnemy.transform.position, 0.4f);
                    }
                }

                break;
            #endregion
            #region attack
            case attackStates.attacking:
                //timer
                attackTimer += Time.deltaTime;

                character.inputManager.blockPressed = false;


                if (!isTargetedBySomeone())
                {
                    character.inputManager.attackPressed = true;

                    if (attackTimer > attackTimerMax)
                    {
                        //reset timer
                        attackTimer = 0;
                        attackTimerMax = Random.Range(1f, 4f);

                        //if 50% and is close to the enemy then switch to standing
                        if (Random.Range(0, 100) < 50 && Vector3.Distance(targetedEnemy.transform.position, body.transform.position) > attackRange * 1.2f)
                            rePositioning = true;
                        else
                            rePositioning = false;

                        //if 50% change sidle direction
                        if (Random.Range(0, 100) < 50)
                        {
                            if (sidleRight)
                                sidleRight = false;
                            else
                                sidleRight = true;
                        }
                    }

                    //if can use stun attack and target not very far away then use stun attack
                    if (stunAttack.canUseSkill() && Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.2f)
                        stunAttack.abilityInputPressed = true;

                    //if can use pierce attack and target not very far away then use pierce attack
                    if (shieldPiercingAttack.canUseSkill() && Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.2f)
                        shieldPiercingAttack.abilityInputPressed = true;

                    //if far from enemy then stop standing
                    if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) > attackRange * 1.2f)
                    {
                        rePositioning = true;
                    }

                    //if very far away go after him
                    if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) > attackRange * 5f)
                    {
                        attackState = attackStates.approaching;
                        attackTimer = 0;
                        attackTimerMax = Random.Range(1f, 4f);
                    }

                    //if standing then stand, else sidle
                    if (rePositioning)
                    {
                        //character.inputManager.moveDirection = new Vector3(0, 0, 0);
                        MoveTowardsPoint(SearchCirclePos(), 0.6f);
                    }
                    else
                    {
                        if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.2f)
                        {
                            if (sidleRight)
                                character.inputManager.moveDirection = (character.transform.right).normalized;
                            else
                                character.inputManager.moveDirection = (-character.transform.right).normalized;
                            character.inputManager.moveDirection *= 0.3f;
                        }
                        //if getting away from the target with sidle, then move closer
                        else
                        {
                            MoveTowardsPoint(targetedEnemy.transform.position, 0.3f);
                        }
                    }

                }
                else
                {
                    attackState = attackStates.defending;
                    attackTimer = 0;
                    attackTimerMax = Random.Range(1f, 4f);
                }
                break;
            #endregion
            #region defend
            case attackStates.defending:
                attackTimer += Time.deltaTime;

                character.inputManager.blockPressed = true;

                if (attackTimer > attackTimerMax)
                {
                    attackTimer = 0;

                    //dodging
                    if (Random.Range(0, 100) < 70 && character.enemiesTargetingThis.Count > 1)
                    {
                        switch ((int)Random.Range(1, 4))
                        {
                            case 1:
                                dodge.dodgeDirection = Dodge.dodgeDirections.back;
                                break;
                            case 2:
                                dodge.dodgeDirection = Dodge.dodgeDirections.right;
                                break;
                            case 3:
                                dodge.dodgeDirection = Dodge.dodgeDirections.left;
                                break;
                        }
                        dodge.abilityInputPressed = true;
                    }

                    //if 70 % then sidle else change to standing
                    if (Random.Range(0, 100) < 70)
                    {
                        standing = false;

                        Quaternion rotation = Quaternion.Euler(0, Random.Range(-90, 90), 0);
                        Vector3 rotatedDirection = rotation * -character.transform.forward;

                        if (goesBackwards)
                            character.inputManager.moveDirection = rotatedDirection * 0.2f;
                        else
                            character.inputManager.moveDirection = rotatedDirection * -0.4f;
                    }
                    else
                    {
                        standing = true;

                        character.inputManager.moveDirection = new Vector3(0, 0, 0);
                    }

                    //if too close then stop or go back
                    if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange)
                    {
                        if (Random.Range(0, 100) < 50)
                            standing = true;
                        else
                            goesBackwards = true;
                    }

                    //if attack range *1.5 > distance then 50% else 30%
                    if (Random.Range(0, 100) < (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.5f ? 50 : 30))
                        character.inputManager.attackPressed = true;
                    else
                        character.inputManager.attackPressed = false;

                    //if distance is little, 80% to go backwards, else 50% to go backwards
                    if (Random.Range(0, 100) < (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.5f ? 80 : 50))
                    {
                        goesBackwards = true;
                    }
                    else
                    {
                        goesBackwards = false;
                    }

                    if (standing)
                        attackTimerMax = Random.Range(1f, 2f);
                    else
                        attackTimerMax = Random.Range(1f, 4f);

                }

                //MoveTowardsPoint(SearchCirclePos());

                //if far from hitting zone stop hitting
                if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) > attackRange * 1.5f)
                    character.inputManager.attackPressed = false;

                //if can use stun attack and target not very far away then use stun attack
                if (stunAttack.canUseSkill() && Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.2f)
                    stunAttack.abilityInputPressed = true;

                //if can use pierce attack and target not very far away then use pierce attack
                if (shieldPiercingAttack.canUseSkill() && Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 1.2f)
                    shieldPiercingAttack.abilityInputPressed = true;


                if (!isTargetedBySomeone())
                {
                    attackState = attackStates.attacking;
                    attackTimer = 0;
                    attackTimerMax = Random.Range(1f, 4f);
                }
                if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) > attackRange * 5f)
                {
                    attackState = attackStates.approaching;
                    attackTimer = 0;
                    attackTimerMax = Random.Range(1f, 4f);
                }

                break;
                #endregion
        }

        RotateTowardsPoint(targetedEnemy.transform.position);
    }
    Vector3 SearchCirclePos()
    {
        float angleStep = 360f / targetedEnemy.enemiesTargetingThis.Count;

        for (int i = 0; i < targetedEnemy.enemiesTargetingThis.Count; i++)
        {
            float angle = angleStep * i;
            if (targetedEnemy.enemiesTargetingThis[i] == character)
            {
                return targetedEnemy.transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle + targetedEnemy.transform.rotation.y) * (attackRange * Random.Range(2f, 3f)), 0f, Mathf.Cos(Mathf.Deg2Rad * angle + targetedEnemy.transform.rotation.y) * (attackRange * Random.Range(2f, 3f)));
            }
        }
        return new Vector3(0, 0, 0);
    }



    public Character followedTarget;
    void Follow()
    {
        if (Vector3.Distance(followedTarget.transform.position, body.position) > 10)
            MoveIntelligentlyToPoint(followedTarget.transform.position);
        else
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);

        RotateTowardsPoint(followedTarget.transform.position);
    }




    IEnumerator TryToLookForClosestEnemySometimes()
    {
        LookForEnemy();

        yield return new WaitForSeconds(Random.Range(2f, 3f));

        StartCoroutine(TryToLookForClosestEnemySometimes());
    }
    void LookForEnemy()
    {
        float min = 9999999999;
        int index = 0;
        int whichGroup = 0;     //0 - elves     1 - humans      2 - goblins
        if (GameManager.instance.elves.Count > 0)
        {
            for (int i = 1; i < GameManager.instance.elves.Count; i++)
            {
                if ((GameManager.instance.elves[i].body.position - body.position).magnitude < min)
                {
                    min = (GameManager.instance.elves[i].body.position - body.position).magnitude;
                    index = i;
                    whichGroup = 0;
                }
            }
        }
        if (GameManager.instance.humans.Count > 0)
        {
            for (int i = 1; i < GameManager.instance.humans.Count; i++)
            {
                if ((GameManager.instance.humans[i].body.position - body.position).magnitude < min)
                {
                    min = (GameManager.instance.humans[i].body.position - body.position).magnitude;
                    index = i;
                    whichGroup = 1;
                }
            }
        }
        if (GameManager.instance.goblins.Count > 0)
        {
            for (int i = 1; i < GameManager.instance.goblins.Count; i++)
            {
                if ((GameManager.instance.goblins[i].body.position - body.position).magnitude < min)
                {
                    min = (GameManager.instance.goblins[i].body.position - body.position).magnitude;
                    index = i;
                    whichGroup = 2;
                }
            }
        }

        if (min < 50)
        {
            if (targetedEnemy != null)
                targetedEnemy.enemiesTargetingThis.Remove(character);

            switch (whichGroup)
            {
                case 0:
                    targetedEnemy = GameManager.instance.elves[index].character;
                    break;
                case 1:
                    targetedEnemy = GameManager.instance.humans[index].character;
                    break;
                case 2:
                    targetedEnemy = GameManager.instance.goblins[index].character;
                    break;
            }
            targetedEnemy.enemiesTargetingThis.Add(character);
            isFighting = true;
        }
    }
}
