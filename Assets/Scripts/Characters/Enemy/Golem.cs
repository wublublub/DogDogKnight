using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 30;

    public GameObject rockPerfab;//对应的投掷的石头

    public Transform handPos;//投掷时手的位置

    //动画事件
    public void KickOff()
    {
        if (attackTarget != null)
        {
            attackTarget.GetComponent<Animator>().ResetTrigger("Attack");
            transform.LookAt(attackTarget.transform.position);//不管怎样敌人首先得面向玩家

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//此处是将方向规范化（也就是三个方向的平方和等于1）

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//先打断玩家的移动
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//速度等于方向乘以力（此处是简化的模拟表示，而并不是真实的物理
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//当玩家被打飞时会进行眩晕
        }
    }

    public void ThrowRock()
    {
        

        if(attackTarget != null)
        {
            var rock = Instantiate(rockPerfab,handPos.position,Quaternion.identity);//此处是石头预制体，投掷的位置，初始的角度

            rock.GetComponent<Rock>().target = attackTarget;


        }
    }

}
