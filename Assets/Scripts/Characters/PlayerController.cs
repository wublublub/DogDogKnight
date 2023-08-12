using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;
    
    private CharacterStats characterStats;

    private GameObject attackTarget;//攻击对象

    private float lastAttackTime;//攻击的内置CD

    private bool isDead;

    public float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        //每次游戏开始时都将会把角色的当前血量调整为最大血量
        //characterStats.CurrentHealth = characterStats.MaxHealth;
        stopDistance = agent.stoppingDistance;

    }

    private void Start()
    {

        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClick += EventAttack;
        characterStats.MaxHealth = 100;

        GameManager.Instance.RegisterPlayer(characterStats);
    }



    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;//此处的运算顺序是先算右边的是否等于的bool运算，然后再将bool值赋值给isDead
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }
        
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    //
    public void SwitchAnimation()
    {
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);//此处的sqrMagnitude是将vector3的值转换为浮点数
        anim.SetBool("Death", isDead);
    }



    //任务启动时注册对应的mousemanager
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//打断所有协程，比如移动的攻击
        if(isDead)
        {
            return;
        }
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;//此处是为了避免在点击攻击之后人物就没法移动了
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead)
        {
            return;
        }
        if (target != null)//当攻击目标不为空时
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());

        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);//此处是对应敌人方向进行转向


        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        //Attack
        if(lastAttackTime < 0)
        {
            anim.SetBool("Critical",characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //动画事件
    private void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if(attackTarget.GetComponent<Rock>() != null && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;

                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;

                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            
            targetStats.TakeDamage(characterStats, targetStats);            
        }



    }

}
