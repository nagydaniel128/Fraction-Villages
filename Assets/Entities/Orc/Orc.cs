using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : CharacterControllerScript
{
    public Transform body;
    public Character targetedEnemy;


    public bool isFighting;

    protected void MoveTowardsPoint(Vector3 point, float speed = 1f)
    {
        Vector3 direction = point - body.position;
        character.inputManager.moveDirection = direction.normalized * speed;
    }
    protected void MoveIntelligentlyToPoint(Vector3 point, float speed = 1f)
    {
        character.inputManager.desiredTravelPoint = point * speed;
        character.inputManager.shouldTravelToTravelPoint = true;
    }
    protected void RotateTowardsPoint(Vector3 point)
    {
        Vector3 direction = (point - body.position).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        character.inputManager.lookAngle = targetAngle;
    }




    public override void SkillsCd()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
