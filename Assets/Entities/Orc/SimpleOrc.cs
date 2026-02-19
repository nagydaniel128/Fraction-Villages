using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleOrc : Orc
{
    public states state = states.wanderInVillage;
    public enum states
    {
        wanderInVillage,
        following,
        guarding,
        patrol,
        gatheringWood,
        gatheringIron,
        emergency,
        restoreHp
    }




    void Update()
    {
        SkillsCd();

        if (!character.hasShield && !isFighting)
        {
            RestoreShield();
            return;
        }

        if (character.stats.hp < character.stats.maxHp * 0.8 && !isFighting)
        {
            ChangeState(states.restoreHp);
        }

        if (isFighting)
            Fight();
        else
            switch (state)
            {
                case states.following:
                    Follow();
                    break;
                case states.wanderInVillage:
                    WanderInVillage();
                    break;
                case states.guarding:
                    Guard();
                    break;
                case states.patrol:
                    Patrol();
                    break;
                case states.gatheringWood:
                    GatherWood();
                    break;
                case states.gatheringIron:
                    GatherIron();
                    break;
                case states.emergency:
                    Emergency();
                    break;
                case states.restoreHp:
                    RestoreHp();
                    break;
            }
    }
    private void Start()
    {
        GameManager.instance.orcs.Add(this);
        OrcVillage.instance.simpleOrcs.Add(this);

        targetedPatrolPoint = OrcVillage.instance.patrolPoints[Random.Range(0, OrcVillage.instance.patrolPoints.Length)];

        StartCoroutine(TryToLookForClosestEnemySometimes());

        //skills
        dodge = new Dodge(this);
        stunAttack = new StunAttack(this);
        shieldPiercingAttack = new ShieldPiercingAttack(this);
    }

    


    public Character followedTarget;
    void Follow()
    {
        if (Vector3.Distance(followedTarget.transform.position, body.position) > 5)
            MoveIntelligentlyToPoint(followedTarget.transform.position);
        else
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);

        RotateTowardsPoint(followedTarget.transform.position);
    }




    IEnumerator TryToLookForClosestEnemySometimes()
    {
        LookForEnemy();

        yield return new WaitForSeconds(Random.Range(2f,3f));

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

            switch(whichGroup)
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
                reachedWanderDestination = true;    //idk
                madeEmergency = false;
                character.inputManager.shouldTravelToTravelPoint = true;
                character.inputManager.attackPressed = false;
                character.inputManager.blockPressed = false;

                character.inputManager.moveDirection = new Vector3(0, 0, 0);

                return;
            }
        }

        if (character.enemiesTargetingThis.Count > 3)
            MakeEmergency();

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
                            if (Random.Range(0,100) < 50)
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
                        switch((int)Random.Range(1,4))
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
                    if(Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange)
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

                    if(standing)
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



    bool madeEmergency;
    void MakeEmergency()
    {
        if (!madeEmergency)
        {
            for (int i = 0; i < GameManager.instance.emergencies.Count; i++)
            {
                if (Vector3.Distance(GameManager.instance.emergencies[i].transform.position, body.position) < 60)
                    return;
            }
            GameObject a = Instantiate(GameManager.instance.emergency);
            a.transform.position = body.position + new Vector3(0, 5, 0);

            madeEmergency = true;
        }
    }





    Vector3 wanderPoint = new Vector3(0, 0, 0);
    bool reachedWanderDestination = true;
    float waitTimer;
    float timeToWait = 2;
    bool isWaiting;
    void WanderInVillage()
    {
        if (!isWaiting)
        {
            if (reachedWanderDestination)
            {
                wanderPoint = OrcVillage.instance.legitPointToWanderInsideVillage();
                reachedWanderDestination = false;
                MoveIntelligentlyToPoint(wanderPoint);
                RotateTowardsPoint(wanderPoint);
            }
            else
            {
                if (Vector3.Distance(body.position, wanderPoint) < 3)
                {
                    reachedWanderDestination = true;
                    isWaiting = true;

                    character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            waitTimer += Time.deltaTime;

            if(waitTimer > timeToWait)
            {
                waitTimer = 0;

                timeToWait = Random.Range(2, 4);

                isWaiting = false;
            }
        }
    }





    public OrcGuardPoint guardPointToGuard;
    void Guard()
    {
        if (Vector3.Distance(guardPointToGuard.transform.position, body.position) > 2)
        {
            MoveIntelligentlyToPoint(guardPointToGuard.transform.position);
            RotateTowardsPoint(guardPointToGuard.transform.position);
        }
        else
        {
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
        }
    }



    Transform targetedPatrolPoint;
    void Patrol()
    {
        if(isWaiting)
        {
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
            waitTimer += Time.deltaTime;

            if(waitTimer > timeToWait)
            {
                waitTimer = 0;
                isWaiting = false;
            }
        }
        else
        {
            if (Vector3.Distance(targetedPatrolPoint.position, body.position) > 5)
            {
                MoveIntelligentlyToPoint(targetedPatrolPoint.transform.position);
                RotateTowardsPoint(targetedPatrolPoint.transform.position);
            }
            else
            {
                isWaiting = true;
                targetedPatrolPoint = OrcVillage.instance.patrolPoints[Random.Range(0, OrcVillage.instance.patrolPoints.Length)];
            }
        }
    }



    const int CARRYINGCAPACITY = 20;
    public int carriedAmount = 0;
    Tree targetedTree;
    float chopDigTimer;
    void GatherWood()
    {
        character.inputManager.attackPressed = false;

        if (carriedAmount == CARRYINGCAPACITY)
        {
            if (Vector3.Distance(body.position, OrcVillage.instance.tent.transform.position) > attackRange * 5)
                MoveIntelligentlyToPoint(OrcVillage.instance.tent.transform.position);
            else
            {
                OrcVillage.instance.wood += carriedAmount;
                carriedAmount = 0;
            }
        }
        else 
        {
            if (targetedTree == null)
            {
                //look for nearest tree
                if (GameManager.instance.trees.Count > 0)
                {
                    float min = (GameManager.instance.trees[0].transform.position - body.position).magnitude;
                    int index = 0;
                    for (int i = 1; i < GameManager.instance.trees.Count; i++)
                    {
                        if ((GameManager.instance.trees[i].transform.position - body.position).magnitude < min)
                        {
                            min = (GameManager.instance.trees[i].transform.position - body.position).magnitude;
                            index = i;
                        }
                    }

                    targetedTree = GameManager.instance.trees[index];
                }
                else
                    ChangeState(states.wanderInVillage);
            }
            else
            {
                if (Vector3.Distance(body.position, targetedTree.transform.position) > attackRange * 2)
                    MoveIntelligentlyToPoint(targetedTree.transform.position);
                else
                {
                    character.inputManager.attackPressed = true;
                    character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
                    RotateTowardsPoint(targetedTree.transform.position);

                    chopDigTimer += Time.deltaTime;

                    if (chopDigTimer > 1)
                    {
                        chopDigTimer = 0;
                        targetedTree.HitTree(this);
                    }
                }
            }
        }
    }



    public OrcBeacon beacon;
    Iron targetedIron;
    void GatherIron()
    {
        if (targetedIron == null)
        {
            targetedIron = beacon.ironGroup.irons[Random.Range(0, beacon.ironGroup.irons.Length)];
        }
        else
        {
            if (carriedAmount == CARRYINGCAPACITY)
            {
                if (Vector3.Distance(body.position, OrcVillage.instance.tent.transform.position) > attackRange * 5)
                {
                    MoveIntelligentlyToPoint(OrcVillage.instance.tent.transform.position);
                    character.inputManager.attackPressed = false;
                }
                else
                {
                    OrcVillage.instance.wood += carriedAmount;
                    carriedAmount = 0;
                }
            }
            else
            {
                if (Vector3.Distance(targetedIron.transform.position, body.position) > attackRange * 2)
                {
                    MoveIntelligentlyToPoint(targetedIron.transform.position);
                    character.inputManager.attackPressed = false;
                }
                else
                {
                    character.inputManager.attackPressed = true;
                    character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
                    RotateTowardsPoint(targetedIron.transform.position);

                    chopDigTimer += Time.deltaTime;
                    if (chopDigTimer > 5)
                    {
                        chopDigTimer = 0;
                        carriedAmount += 1;
                    }

                    waitTimer += Time.deltaTime;
                    if (waitTimer > timeToWait)
                    {
                        waitTimer = 0;
                        timeToWait = Random.Range(10, 20);

                        targetedIron = null;
                    }
                }

            }
        }
    }



    public Emergency emergency;
    void Emergency()
    {
        if (Vector3.Distance(emergency.transform.position, body.position) > 10)
        {
            MoveIntelligentlyToPoint(emergency.transform.position);
            RotateTowardsPoint(emergency.transform.position);
        }
        else
        {
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
        }
    }



    void RestoreHp()
    {
        if (Vector3.Distance(OrcVillage.instance.campfire.transform.position, body.position) > 5)
        {
            MoveIntelligentlyToPoint(OrcVillage.instance.campfire.transform.position);
            RotateTowardsPoint(OrcVillage.instance.campfire.transform.position);
        }
        else
        {
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);

            if (character.stats.hp == character.stats.maxHp)
                ChangeState(states.wanderInVillage);
        }
    }



    void RestoreShield()
    {
        if (Vector3.Distance(OrcVillage.instance.tent.transform.position, body.position) > attackRange * 5)
            MoveIntelligentlyToPoint(OrcVillage.instance.tent.transform.position);
        else
            character.hasShield = true;
    }



    public override void Die()
    {
        GameManager.instance.orcs.Remove(this);

        if (targetedEnemy != null)
            targetedEnemy.enemiesTargetingThis.Remove(character);

        if (state == states.guarding)
            guardPointToGuard.guardingOrc = null;

        if (state == states.gatheringWood)
            OrcVillage.instance.woodGathererCounter--;

        if (state == states.gatheringIron)
            beacon.orcsAssignedHere--;

        if (state == states.patrol)
            OrcVillage.instance.patrollingOrcsCounter--;

        Destroy(gameObject);
    }



    public void ChangeState(states newState)
    {
        carriedAmount = 0;

        character.inputManager.attackPressed = false;
        character.inputManager.blockPressed = false;

        if (state == states.guarding && newState != states.guarding)
            guardPointToGuard.guardingOrc = null;

        if (state == states.gatheringWood && newState != states.gatheringWood)
            OrcVillage.instance.woodGathererCounter--;

        if (state == states.gatheringIron && newState != states.gatheringIron)
            beacon.orcsAssignedHere--;

        if (state == states.patrol && newState != states.patrol)
            OrcVillage.instance.patrollingOrcsCounter--;

        switch (newState)
        {
            case states.following:
                state = states.following;
                break;
            case states.guarding:
                state = states.guarding;
                break;
            case states.wanderInVillage:
                waitTimer = 0;
                reachedWanderDestination = true;
                isWaiting = false;
                timeToWait = 2;
                state = states.wanderInVillage;
                break;
            case states.patrol:
                OrcVillage.instance.patrollingOrcsCounter++;
                waitTimer = 0;
                isWaiting = false;
                timeToWait = 2;
                targetedPatrolPoint = OrcVillage.instance.patrolPoints[Random.Range(0, OrcVillage.instance.patrolPoints.Length)];
                state = states.patrol;
                break;
            case states.gatheringWood:
                OrcVillage.instance.woodGathererCounter++;
                state = states.gatheringWood;
                break;
            case states.gatheringIron:
                waitTimer = 0;
                timeToWait = 15;
                state = states.gatheringIron;
                break;
            case states.emergency:
                state = states.emergency;
                break;
            case states.restoreHp:
                state = states.restoreHp;
                break;
        }
    }

    public override void SkillsCd()
    {
        dodge.Cd();
        stunAttack.Cd();
        shieldPiercingAttack.Cd();
    }
}
