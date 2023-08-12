using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates {HitPlayer, HitEnemy, HitNothing }//石头的三种状态，分别对应撞击玩家，撞击怪物，撞击地面

    private Rigidbody rb;

    public RockStates rockStates;//记录当前的石头的状态

    [Header("Basic Settings")]
    public float force;//投掷的石头的力度

    public int damage;//投掷的石头所能造成的伤害

    public GameObject target;//投掷的目标

    public Vector3 direction;//飞行的方向

    public GameObject breakEffect;//主要用于处理粒子特效

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;//初始生成时是为了打击玩家的
        FlyToTarget();

    }



    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if(target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;//注意，此处仅适用于场景中只有一个主角的情况
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch(rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player") && rb.velocity.sqrMagnitude > 1f)//如果碰撞的是玩家
                {
                    //将玩家弹开并造成一次伤害
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent <NavMeshAgent>().velocity = direction * force;


                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//造成眩晕的效果
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockStates.HitNothing;
                    
                }

                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);


                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }

                break;
        }
    }

}
