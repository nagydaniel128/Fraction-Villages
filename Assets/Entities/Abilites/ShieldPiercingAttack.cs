using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPiercingAttack : Ability
{
    public ShieldPiercingAttack(CharacterControllerScript CharacterController)
    {
        characterController = CharacterController;
    }
    public float cdTimer = 0;
    public float cdMax = 6f;
    public override bool canUseSkill()
    {
        return cdTimer > cdMax;
    }

    public override void Cd()
    {
        if (cdTimer < cdMax && !abilityStarted)
            cdTimer += Time.deltaTime;
    }

    public override void UseAbility()
    {
        if (!abilityStarted)
        {
            characterController.character.animationHandler.SetAttackAnimation();
            //characterController.character.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            characterController.character.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
            abilityStarted = true;
        }
        else
        {
            characterController.character.movementHandler.agent.Move(characterController.character.movementHandler.agent.transform.forward * 0.5f * Time.deltaTime);

            abilityTimer += Time.deltaTime;
            //wait for the end of the attack and then apply stun to the enemy
            if (abilityTimer > 1)
            {
                LeavingCharacterCurrentAbility();
                cdTimer = 0;

                //characterController.character.transform.localScale = new Vector3(1, 1, 1);
                characterController.character.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

                ApplyPiercingAttackToEnemies();
            }
        }
    }
    void ApplyPiercingAttackToEnemies()
    {
        switch (characterController.character.tag)
        {
            case "Orc":
                for (int i = 0; i < GameManager.instance.elves.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.elves[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.elves[i].character, characterController.character.stats.damage, true);
                }
                for (int i = 0; i < GameManager.instance.humans.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.humans[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.humans[i].character, characterController.character.stats.damage, true);
                }
                for (int i = 0; i < GameManager.instance.goblins.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.goblins[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.goblins[i].character, characterController.character.stats.damage, true);
                }
                break;
            case "Elf":
                if (characterController.character.movementHandler.damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    characterController.character.DealDamageTo(Player.instance.character, characterController.character.stats.damage, true);
                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.orcs[i].character, characterController.character.stats.damage, true);
                }
                break;
            case "Human":
                if (characterController.character.movementHandler.damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    characterController.character.DealDamageTo(Player.instance.character, characterController.character.stats.damage, true);
                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.orcs[i].character, characterController.character.stats.damage, true);
                }
                break;
            case "Goblin":
                if (characterController.character.movementHandler.damageArea.bounds.Intersects(Player.instance.character.bodyArea.bounds))
                    characterController.character.DealDamageTo(Player.instance.character, characterController.character.stats.damage, true);
                for (int i = 0; i < GameManager.instance.orcs.Count; i++)
                {
                    if (characterController.character.movementHandler.damageArea.bounds.Intersects(GameManager.instance.orcs[i].character.bodyArea.bounds))
                        characterController.character.DealDamageTo(GameManager.instance.orcs[i].character, characterController.character.stats.damage, true);
                }
                break;
        }
    }
}
