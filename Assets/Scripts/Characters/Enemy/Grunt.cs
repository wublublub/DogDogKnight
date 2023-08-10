using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]//此处对应特殊敌人的特殊攻击

    public float kickForce = 10;//该设置对应第一段推开的力

    public void KickOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform.position);//不管怎样敌人首先得面向玩家

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//此处是将方向规范化（也就是三个方向的平方和等于1）

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//先打断玩家的移动
            attackTarget.GetComponent <NavMeshAgent>().velocity = direction * kickForce;//速度等于方向乘以力（此处是简化的模拟表示，而并不是真实的物理
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//当玩家被打飞时会进行眩晕
        }
    }
    
}
