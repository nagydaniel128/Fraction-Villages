using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Elf : CharacterControllerScript
{
    public Transform target;
    public Transform body;

    public ElfGroup group;

    public states state = states.idle;
    public enum states
    {
        idle,
        wandering
    }

    private void Update()
    {
        SkillsCd();

        if (isFighting)
            Fight();
        else
            switch (state)
            {
                case states.idle:
                    character.inputManager.moveDirection = new Vector3(0, 0, 0);
                    break;
                case states.wandering:
                    Wander();
                    break;
            }
    }
    private void Start()
    {
        GameManager.instance.elves.Add(this);

        StartCoroutine(TryToLookForClosestEnemySometimes());

        body.GetComponent<NavMeshAgent>().enabled = true;

        //skills
        dodge = new Dodge(this);
        stunAttack = new StunAttack(this);
        shieldPiercingAttack = new ShieldPiercingAttack(this);
        shieldPiercingAttack.cdMax = 10;
    }

    void MoveTowardsPoint(Vector3 point, float speed = 1f)
    {
        Vector3 direction = point - body.position;
        character.inputManager.moveDirection = direction.normalized * speed;
    }
    void MoveIntelligentlyToPoint(Vector3 point, float speed = 1f)
    {
        character.inputManager.desiredTravelPoint = point * speed;
        character.inputManager.shouldTravelToTravelPoint = true;
    }
    void RotateTowardsPoint(Vector3 point)
    {
        Vector3 direction = (point - body.position).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        character.inputManager.lookAngle = targetAngle;
    }






    public bool isFighting;
    Character targetedEnemy;
    const float attackRange = 2.3f;
    float attackTimer;
    float attackTimerMax;
    bool sidleRight;
    bool standing;
    bool rePositioning;
    bool goesBackwards;
    enum attackStates
    {
        approaching,
        attacking,
        defending
    }
    attackStates attackState = attackStates.approaching;
    //skills
    Dodge dodge;
    StunAttack stunAttack;
    ShieldPiercingAttack shieldPiercingAttack;
    void Fight()
    {
        character.inputManager.shouldTravelToTravelPoint = false;

        if (targetedEnemy == null)
        {
            LookForEnemy();
            if (targetedEnemy == null)
            {
                isFighting = true;

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

                if (Vector3.Distance(targetedEnemy.transform.position, body.transform.position) < attackRange * 3f)
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

                //if target too far away from hitting zone hten stop hitting
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

    bool isTargetedBySomeone()
    {
        return character.enemiesTargetingThis.Count > 0;
    }


    IEnumerator TryToLookForClosestEnemySometimes()
    {
        LookForEnemy();

        yield return new WaitForSeconds(Random.Range(2f, 3f));

        StartCoroutine(TryToLookForClosestEnemySometimes());
    }
    void LookForEnemy()
    {
        float min = (Player.instance.character.transform.position - body.position).magnitude;
        int index = -1;
        if (GameManager.instance.orcs.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.orcs.Count; i++)
            {
                if ((GameManager.instance.orcs[i].body.position - body.position).magnitude < min)
                {
                    min = (GameManager.instance.orcs[i].body.position - body.position).magnitude;
                    index = i;
                }
            }
        }
        if (min < 50)
        {
            if (targetedEnemy != null)
                targetedEnemy.enemiesTargetingThis.Remove(character);

            if(index == -1)
                targetedEnemy = Player.instance.character;
            else
                targetedEnemy = GameManager.instance.orcs[index].character;

            targetedEnemy.enemiesTargetingThis.Add(character);
            isFighting = true;
        }
    }



    public bool arrivedToWanderPoint;
    public Vector3 wanderPoint;
    void Wander()
    {
        if (Vector3.Distance(wanderPoint, body.position) > 4)
        {
            MoveIntelligentlyToPoint(wanderPoint);
            RotateTowardsPoint(wanderPoint);
            arrivedToWanderPoint = false;
        }
        else
        {
            character.inputManager.desiredTravelPoint = new Vector3(0, 0, 0);
            arrivedToWanderPoint = true;
        }
    }



    public override void Die()
    {
        GameManager.instance.elves.Remove(this);

        if (targetedEnemy != null)
            targetedEnemy.enemiesTargetingThis.Remove(character);

        if (group != null)
            group.RemoveElf(this);

        Destroy(gameObject);
    }

    public override void SkillsCd()
    {
        dodge.Cd();
        stunAttack.Cd();
        shieldPiercingAttack.Cd();
    }
}
