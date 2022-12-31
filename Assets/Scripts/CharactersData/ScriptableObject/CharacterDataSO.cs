using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Character Info")]
    public int maxHealth;
    public int currentHealth;
    public int initialDefence;
    public int currentDefence;

    [Header("Kill")]
    public int expPointOnKill;

    [Header("Level Info")]
    public int level;
    public int maxLevel;
    public int currentExp;
    public int levelUpNeededExp;
    public float levelExponent;
    // when level up, we need more exp
    public float levelMultipler { get { return 1 + (level - 1) * levelExponent; } }

    public void UpdateExp(int expPoint)
    {
        currentExp += expPoint;
        // Level Up
        if (currentExp >= levelUpNeededExp)
        {
            levelUp();
        }
    }

    private void levelUp()
    {
        // make sure the currentLevel + 1 wont over maxLevel
        if (level + 1 > maxLevel)
        {
            level = maxLevel;
        }
        else
        {
            // we can update max health and defence of player by level up
            level = level + 1;
            levelUpNeededExp += (int)(levelUpNeededExp * levelMultipler);
            maxHealth = (int)(maxHealth * levelMultipler);
            currentHealth = maxHealth;
            currentDefence = currentDefence + 1;
            Debug.Log("Level Up!" + level + "Max Health: " + maxHealth + "Current Defence: " + currentDefence);

        }

    }
}
