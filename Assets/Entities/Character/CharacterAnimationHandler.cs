using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    public Animator bodyAnimator;
    public Animator legAnimator;

    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    RotateTowardsCam rotater;
    void EverythingToFalseBody()
    {
        bodyAnimator.SetBool("dodgeLeft", false);
        bodyAnimator.SetBool("dodgeRight", false);
        bodyAnimator.SetBool("dodgeBack", false);
        bodyAnimator.SetBool("attack", false);
        bodyAnimator.SetBool("block", false);
        bodyAnimator.SetBool("hit", false);
        bodyAnimator.SetBool("idle", false);
        bodyAnimator.SetBool("running", false);
        bodyAnimator.SetBool("stunned", false);
    }
    void EverythingToFalseLegs()
    {
        legAnimator.SetBool("idle", false);
        legAnimator.SetBool("running", false);
        legAnimator.SetBool("dodgeLeft", false);
        legAnimator.SetBool("dodgeRight", false);
        legAnimator.SetBool("dodgeBack", false);
        legAnimator.SetBool("stunned", false);
    }


    public void SetRunAnimation(bool legsOnly)
    {
        if (!legsOnly)
        {
            EverythingToFalseBody();
            bodyAnimator.SetBool("running", true);
        }
        EverythingToFalseLegs();
        legAnimator.SetBool("running", true);
    }
    public void SetIdleAnimation(bool legsOnly)
    {
        if (!legsOnly)
        {
            EverythingToFalseBody();
            bodyAnimator.SetBool("idle", true);
        }
        EverythingToFalseLegs();
        legAnimator.SetBool("idle", true);
    }
    public void SetAttackAnimation()
    {
        EverythingToFalseBody();
        bodyAnimator.SetBool("attack", true);
    }

    public void SetBlockAnimation()
    {
        EverythingToFalseBody();
        bodyAnimator.SetBool("block", true);
    }


    public void SetHitAnimation()
    {
        EverythingToFalseBody();
        EverythingToFalseLegs();
        bodyAnimator.SetBool("hit", true);
        legAnimator.SetBool("idle", true);
        bodyAnimator.Play("hit", -1, 0f);
    }

    public void SetStunnedAnimation()
    {
        EverythingToFalseBody();
        EverythingToFalseLegs();
        bodyAnimator.SetBool("stunned", true);
        legAnimator.SetBool("stunned", true);
        bodyAnimator.Play("stunned", -1, 0f);
    }

    public void SetDodgeAnimation(Dodge.dodgeDirections dodgeDirections)
    {
        switch (dodgeDirections)
        {
            case Dodge.dodgeDirections.left:
                legAnimator.SetBool("dodgeLeft", true);
                bodyAnimator.SetBool("dodgeLeft", true);
                break;
            case Dodge.dodgeDirections.right:
                legAnimator.SetBool("dodgeRight", true);
                bodyAnimator.SetBool("dodgeRight", true);
                break;
            case Dodge.dodgeDirections.back:
                legAnimator.SetBool("dodgeBack", true);
                bodyAnimator.SetBool("dodgeBack", true);
                break;
        }
    }
}
