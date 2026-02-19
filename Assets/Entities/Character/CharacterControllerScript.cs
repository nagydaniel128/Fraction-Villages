using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControllerScript : MonoBehaviour
{
    public abstract void Die();
    public abstract void SkillsCd();

    public Character character;

    public Ability currentAbiity;
}
