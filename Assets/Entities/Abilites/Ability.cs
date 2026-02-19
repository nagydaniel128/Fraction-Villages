using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public CharacterControllerScript characterController;

    public bool abilityStarted;
    public float abilityTimer;

    bool AbilityInputPressed;
    public bool abilityInputPressed
    {
        get { return AbilityInputPressed; }
        set
        {
            AbilityInputPressed = value;
            if (AbilityInputPressed)
                StartAbility();
        }
    }
    public abstract bool canUseSkill();
    public abstract void UseAbility();
    public abstract void Cd();
    public void StartAbility()
    {
        if (canUseSkill())
            FillingCharacterCurrentAbility();
    }

    public void FillingCharacterCurrentAbility()
    {
        characterController.currentAbiity = this;
    }
    public void LeavingCharacterCurrentAbility()
    {
        abilityTimer = 0;
        abilityStarted = false;
        characterController.character.movementHandler.state = CharacterMovementHandler.movementState.nothing;
        characterController.currentAbiity = null;
    }
}
