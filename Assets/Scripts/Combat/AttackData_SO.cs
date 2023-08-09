using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//������Χ
    public float skillRange;
    public float coolDown;//CD��ȴʱ��
    public float minDamage;//��С������ֵ
    public float maxDamage;//��󹥻���ֵ
    public float criticalMultiplier;//����֮��ļӳɰٷֱ�
    public float criticalChance;//��������

}
