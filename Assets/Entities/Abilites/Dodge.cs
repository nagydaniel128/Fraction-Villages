using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : Ability
{
    public Dodge(CharacterControllerScript CharacterController)
    {
        characterController = CharacterController;
    }


    public float cdTimer = 0;
    public float cdMax = 2f;
    public enum dodgeDirections
    {
        nothing,
        left,
        right,
        back
    }
    public dodgeDirections dodgeDirection = dodgeDirections.nothing;

    public override bool canUseSkill()
    {
        return cdTimer > cdMax && dodgeDirection != dodgeDirections.nothing;
    }

    float dodgeSpeed = 30f;

    public override void UseAbility()
    {
        if (!abilityStarted)
        {
            characterController.character.animationHandler.SetDodgeAnimation(dodgeDirection);
            abilityStarted = true;
            dodgeSpeed = 30f;
        }
        else
        {
            abilityTimer += Time.deltaTime;
            if (abilityTimer > 0.5f)
            {
                LeavingCharacterCurrentAbility();
                cdTimer = 0;
            }

            characterController.character.movementHandler.agent.isStopped = true;
            switch (dodgeDirection)
            {
                case dodgeDirections.left:
                    characterController.character.movementHandler.agent.Move(-characterController.character.movementHandler.agent.transform.right * dodgeSpeed * Time.deltaTime);
                    break;
                case dodgeDirections.right:
                    characterController.character.movementHandler.agent.Move(characterController.character.movementHandler.agent.transform.right * dodgeSpeed * Time.deltaTime);
                    break;
                case dodgeDirections.back:
                    characterController.character.movementHandler.agent.Move(-characterController.character.movementHandler.agent.transform.forward * dodgeSpeed * Time.deltaTime);
                    break;
            }
            dodgeSpeed -= Time.deltaTime * 80f;
        }
    }

    public override void Cd()
    {
        if (cdTimer < cdMax && !abilityStarted)
            cdTimer += Time.deltaTime;
    }
}
