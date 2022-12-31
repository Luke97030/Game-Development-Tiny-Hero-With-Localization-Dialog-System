using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// we cannot attach the Scriptable object directly to a game object,
// hence, we need a class inheritance from MonoBehaviour to have an instance of CharacterDataSO. And it can be attached with game object 
public class CharacterData : MonoBehaviour
{
    public CharacterDataSO templateCharacterDataSO;
    public CharacterDataSO characterDataSO;
    public AttackDataSO attackDataSO;
    public event Action<int, int> updateHPBarOnAttack;
    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if (templateCharacterDataSO != null)
        {
            characterDataSO = Instantiate(templateCharacterDataSO);
        }
    }

    #region character INFO
    public int MaxHealth 
    {
        get
        {
            if (characterDataSO != null)
                return characterDataSO.maxHealth;
            else
                return 0;
        }
        set
        {
            characterDataSO.maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterDataSO != null)
                return characterDataSO.currentHealth;
            else
                return 0;
        }
        set
        {
            characterDataSO.currentHealth = value;
        }
    }

    public int InitialDefence
    {
        get
        {
            if (characterDataSO != null)
                return characterDataSO.initialDefence;
            else
                return 0;
        }
        set
        {
            characterDataSO.initialDefence = value;
        }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterDataSO != null)
                return characterDataSO.currentDefence;
            else
                return 0;
        }
        set
        {
            characterDataSO.currentDefence = value;
        }
    }
    #endregion

    #region attack INFO

    public float AttackRange
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.attackRange;
            else
                return 0;
        }
        set
        {
            attackDataSO.attackRange = value;
        }

    }
    public float SkillRange
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.skillRange;
            else
                return 0;
        }
        set
        {
            attackDataSO.skillRange = value;
        }

    }
    public float Cd
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.cd;
            else
                return 0;
        }
        set
        {
            attackDataSO.cd = value;
        }

    }
    public int MinimumDamage
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.minimumDamage;
            else
                return 0;
        }
        set
        {
            attackDataSO.minimumDamage = value;
        }

    }
    public int MaximumDamage
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.maximumDamage;
            else
                return 0;
        }
        set
        {
            attackDataSO.maximumDamage = value;
        }

    }
    public float CriticalMulti
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.criticalMulti;
            else
                return 0;
        }
        set
        {
            attackDataSO.criticalMulti = value;
        }

    }
    public float CriticalChance
    {
        get
        {
            if (attackDataSO != null)
                return attackDataSO.criticalChance;
            else
                return 0;
        }
        set
        {
            attackDataSO.criticalChance = value;
        }

    }
    #endregion

    #region damge INFO
    public void causeDamage(CharacterData attacker, CharacterData defender)
    {
        int finalDamage;
        int randomDamageValue = attacker.currentDamage();
        // only cause damage when the attack's attack damage greater than defender's defence value
        if (randomDamageValue - defender.CurrentDefence > 0)
        {
            
            finalDamage = randomDamageValue - defender.CurrentDefence;
        }
        else
        {
            // if attack <= defender.CurrentDefence, no damage
            finalDamage = 0;
        }

        // CurrentHealth calculation
        CurrentHealth = CurrentHealth - finalDamage;

        // when attacker made a critical attack, let defender play the hurt animation 
        if (attacker.isCritical)
            defender.GetComponent<Animator>().SetTrigger("hurt");

        if (CurrentHealth < 0)
        {
            // the Current Health cannot be negative
            CurrentHealth = 0;
        }

        // Update UI
        // execute action
        updateHPBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        // Exp update
        if (CurrentHealth == 0)
        {
            attacker.characterDataSO.UpdateExp(characterDataSO.expPointOnKill);
        }

    }

    // causeDamage for rock which is taking damage and defender as parameters 
    public void causeDamage(int damage, CharacterData defender)
    {
        int finalDamage;
        // only cause damage when the attack's attack damage greater than defender's defence value
        if (damage - defender.CurrentDefence > 0)
        {

            finalDamage = damage - defender.CurrentDefence;
        }
        else
        {
            // if attack <= defender.CurrentDefence, no damage
            finalDamage = 0;
        }
        // CurrentHealth calculation
        CurrentHealth = CurrentHealth - finalDamage;
        if (CurrentHealth < 0)
        {
            // the Current Health cannot be negative
            CurrentHealth = 0;
        }
        // Update UI
        // execute action
        updateHPBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        // if the enemy die by rock (player hit back rock) and kill him
        // we also need to update the exp 
        if (CurrentHealth <= 0)
            GameManager.singletonInstance.playerData.characterDataSO.UpdateExp(characterDataSO.expPointOnKill);
    }

    private int currentDamage()
    {
        float damageValue = UnityEngine.Random.Range(attackDataSO.minimumDamage, attackDataSO.maximumDamage);
        if (isCritical)
        {
            damageValue = damageValue * CriticalMulti;
        }
        return (int)damageValue;
    }


    #endregion
}
