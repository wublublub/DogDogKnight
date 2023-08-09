using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//攻击范围
    public float skillRange;
    public float coolDown;//CD冷却时间
    public float minDamage;//最小攻击数值
    public float maxDamage;//最大攻击数值
    public float criticalMultiplier;//暴击之后的加成百分比
    public float criticalChance;//暴击几率

}
