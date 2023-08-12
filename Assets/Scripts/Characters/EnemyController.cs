using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;




//第一种关于切换人物状态的方法：使用Switch和枚举enum来进行切换
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD}


//第二种切换人物状态的方法：使用有限状态机FSM来进行切换




//此处是为了确保人物的身上具有NavMeshAgent组件
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;//枚举变量

    private NavMeshAgent agent;
    
    private Animator anim;

    protected CharacterStats characterStats;

    private Collider coll;


    [Header("Basic Settings")]
    public float sightRadius;//怪物可视范围

    public bool isGuard;//当前敌人是否是站桩的敌人

    private float speed;//记录当前速度，主要用于区分敌人警戒和追击时的速度差异

    protected GameObject attackTarget;

    public float lookAtTime;

    private float remainLookAtTime;

    private float lastAttackTime;

    private Quaternion guardRotation;//记录起始时旋转的角度


    [Header("Patrol State")]
    public float patrolRange;//巡逻范围

    private Vector3 wayPoint;

    private Vector3 guardPos;//巡逻的基准点


    //bool判断来配合动画
    bool isWalk;

    bool isChase;
    
    bool isFollow;
    
    bool isDead;

    bool playerDead;

    bool isSkill;

    bool ifContinue;//是否继续执行动画事件

    private void Awake()
    {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        agent.stoppingDistance = 0.5f;
        


    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates=EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }
    //切换场景时启用 
    /*void OnEnable()
    {
        GameManager.Instance.AddObserver(this);
    }*/

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
        {
            return;
        }
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        if(characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }



    public void SwitchStates()
    {
        if(isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //如果发现了玩家，则应该切换到追击的状态
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }


        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isFollow = false;
                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if(Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk=false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //判断怪物是否已经走到了随机巡逻点
                if (Vector3.Distance(wayPoint,transform.position)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime>0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    { 
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk=true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if(!FoundPlayer())
                {
                    isFollow = false;
                    isChase = false; 
                    //怪物拉脱返回上一个状态
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = agent.transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }

                }
                else//此部分为怪物的追击
                {
                    isFollow = true;
                    agent.isStopped = false;//确保敌人在攻击之后可以继续追着玩家
                    agent.destination = attackTarget.transform.position;
                }
                //接下来将会判断玩家是否在攻击范围之内
                if (TargetInAttackRange()||TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0 && !characterStats.isBeenHit)//此处为当攻击的CD冷却完成并且角色并未处于受击僵直，眩晕状态
                    {
                        lastAttackTime = characterStats.attackData.coolDown;//此时将下一次的攻击事件重置为攻击的CD时间

                        //此时进行暴击的判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;//随机取值判断是否小于暴击率，如果小于则判断发生了暴击
                        //执行攻击
                        Attack();
                    }
                }



                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;//缩小agent的范围
                Destroy(gameObject, 2f);//死亡两秒后消除该游戏对象

                break;
        }
    }

    private void Attack()
    {
        //第一步，敌人需要先看向你的攻击目标
        transform.LookAt(attackTarget.transform);//此处应该可以同时使用transform类型的变量和transform.position类型的变量
        //第二步，判断是近身攻击还是技能攻击
        bool inAttackRange = TargetInAttackRange();
        bool inSkillRange = TargetInSkillRange();

        if (inAttackRange && inSkillRange) // 当目标同时在技能范围内和攻击范围内
        {
            // 随机选择技能或攻击
            if (Random.value < 0.5f) // 50% 的机会进行攻击，50% 的机会进行技能
            {
                anim.SetTrigger("Attack");
                Debug.Log("普通攻击！");
            }
            else
            {
                anim.SetTrigger("Skill");
                Debug.Log("技能攻击！");
            }
        }
        else if (inSkillRange && !inAttackRange) // 只在技能范围内
        {
            anim.SetTrigger("Skill");
            Debug.Log("技能攻击！");
        }
        else if (inAttackRange && !inSkillRange) // 只在攻击范围内
        {
            anim.SetTrigger("Attack");
            Debug.Log("普通攻击！");
        }


    }
    public bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//使用unity内置的方法在一定范围恶的球体内查找碰撞体
        
        foreach (var target in colliders) 
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }

        }
        attackTarget = null;//脱离范围
        return false;
    }

    private bool TargetInAttackRange()
    {
        if(attackTarget != null)
        {
            //判断是否在攻击范围以内
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else
        {
            return false; 
        }
    }

    private bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            //判断是否在攻击范围以内
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else
        {
            return false;
        }
    }

    private void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, guardPos.y, guardPos.z + randomZ);

        NavMeshHit hit;
        //如果随机到的点是可以移动的则移动过去，如果不是则移动到当前点（也就是不动，然后开始下一次随机取点）
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position : transform.position;


    }

    private void OnDrawGizmosSelected()//此处是将巡逻范围，检测范围绘制出来方便后旗调整
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//视野范围


    }

    //动画事件
    private void Hit()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))//判断攻击目标不为空且处在攻击角度内,同时还需要满足此时动画并未被打断
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);

        }
    }

    //受击打断动画
    public void ResetAttack()
    {
        anim.ResetTrigger("Attack");//重置攻击的触发器
    }

    public void ResetHit()
    {
        anim.ResetTrigger("Hit");//重置僵直
    }

    public void ResetDizzy()
    {
        anim.ResetTrigger("Dizzy");//重置眩晕
    }


    public void EndNotify()
    {
        //怪物获胜动画
        anim.SetBool("Win", true);
        playerDead = true;
        //停止移动
        isChase = false;
        isWalk = false;
        attackTarget = null;

        //停止agent

    }
}
