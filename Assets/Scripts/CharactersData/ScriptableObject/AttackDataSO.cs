using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Attack Data")]
public class AttackDataSO : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float skillRange;
    public float cd;
    public int minimumDamage;
    public int maximumDamage;
    public float criticalMulti;            // critical damage multiplier random.range(minimumDamage, maximumDamage)X2 
    public float criticalChance;                // critical chance

}

