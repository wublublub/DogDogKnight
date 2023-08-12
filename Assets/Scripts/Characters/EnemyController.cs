using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;




//��һ�ֹ����л�����״̬�ķ�����ʹ��Switch��ö��enum�������л�
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD}


//�ڶ����л�����״̬�ķ�����ʹ������״̬��FSM�������л�




//�˴���Ϊ��ȷ����������Ͼ���NavMeshAgent���
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;//ö�ٱ���

    private NavMeshAgent agent;
    
    private Animator anim;

    protected CharacterStats characterStats;

    private Collider coll;


    [Header("Basic Settings")]
    public float sightRadius;//������ӷ�Χ

    public bool isGuard;//��ǰ�����Ƿ���վ׮�ĵ���

    private float speed;//��¼��ǰ�ٶȣ���Ҫ�������ֵ��˾����׷��ʱ���ٶȲ���

    protected GameObject attackTarget;

    public float lookAtTime;

    private float remainLookAtTime;

    private float lastAttackTime;

    private Quaternion guardRotation;//��¼��ʼʱ��ת�ĽǶ�


    [Header("Patrol State")]
    public float patrolRange;//Ѳ�߷�Χ

    private Vector3 wayPoint;

    private Vector3 guardPos;//Ѳ�ߵĻ�׼��


    //bool�ж�����϶���
    bool isWalk;

    bool isChase;
    
    bool isFollow;
    
    bool isDead;

    bool playerDead;

    bool isSkill;

    bool ifContinue;//�Ƿ����ִ�ж����¼�

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
        //FIXME:�����л����޸ĵ�
        GameManager.Instance.AddObserver(this);
    }
    //�л�����ʱ���� 
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
        //�����������ң���Ӧ���л���׷����״̬
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
                //�жϹ����Ƿ��Ѿ��ߵ������Ѳ�ߵ�
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
                    //�������ѷ�����һ��״̬
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
                else//�˲���Ϊ�����׷��
                {
                    isFollow = true;
                    agent.isStopped = false;//ȷ�������ڹ���֮����Լ���׷�����
                    agent.destination = attackTarget.transform.position;
                }
                //�����������ж�����Ƿ��ڹ�����Χ֮��
                if (TargetInAttackRange()||TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0 && !characterStats.isBeenHit)//�˴�Ϊ��������CD��ȴ��ɲ��ҽ�ɫ��δ�����ܻ���ֱ��ѣ��״̬
                    {
                        lastAttackTime = characterStats.attackData.coolDown;//��ʱ����һ�εĹ����¼�����Ϊ������CDʱ��

                        //��ʱ���б������ж�
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;//���ȡֵ�ж��Ƿ�С�ڱ����ʣ����С�����жϷ����˱���
                        //ִ�й���
                        Attack();
                    }
                }



                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;//��Сagent�ķ�Χ
                Destroy(gameObject, 2f);//�����������������Ϸ����

                break;
        }
    }

    private void Attack()
    {
        //��һ����������Ҫ�ȿ�����Ĺ���Ŀ��
        transform.LookAt(attackTarget.transform);//�˴�Ӧ�ÿ���ͬʱʹ��transform���͵ı�����transform.position���͵ı���
        //�ڶ������ж��ǽ��������Ǽ��ܹ���
        bool inAttackRange = TargetInAttackRange();
        bool inSkillRange = TargetInSkillRange();

        if (inAttackRange && inSkillRange) // ��Ŀ��ͬʱ�ڼ��ܷ�Χ�ں͹�����Χ��
        {
            // ���ѡ���ܻ򹥻�
            if (Random.value < 0.5f) // 50% �Ļ�����й�����50% �Ļ�����м���
            {
                anim.SetTrigger("Attack");
                Debug.Log("��ͨ������");
            }
            else
            {
                anim.SetTrigger("Skill");
                Debug.Log("���ܹ�����");
            }
        }
        else if (inSkillRange && !inAttackRange) // ֻ�ڼ��ܷ�Χ��
        {
            anim.SetTrigger("Skill");
            Debug.Log("���ܹ�����");
        }
        else if (inAttackRange && !inSkillRange) // ֻ�ڹ�����Χ��
        {
            anim.SetTrigger("Attack");
            Debug.Log("��ͨ������");
        }


    }
    public bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//ʹ��unity���õķ�����һ����Χ��������ڲ�����ײ��
        
        foreach (var target in colliders) 
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }

        }
        attackTarget = null;//���뷶Χ
        return false;
    }

    private bool TargetInAttackRange()
    {
        if(attackTarget != null)
        {
            //�ж��Ƿ��ڹ�����Χ����
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
            //�ж��Ƿ��ڹ�����Χ����
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
        //���������ĵ��ǿ����ƶ������ƶ���ȥ������������ƶ�����ǰ�㣨Ҳ���ǲ�����Ȼ��ʼ��һ�����ȡ�㣩
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position : transform.position;


    }

    private void OnDrawGizmosSelected()//�˴��ǽ�Ѳ�߷�Χ����ⷶΧ���Ƴ�������������
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//��Ұ��Χ


    }

    //�����¼�
    private void Hit()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))//�жϹ���Ŀ�겻Ϊ���Ҵ��ڹ����Ƕ���,ͬʱ����Ҫ�����ʱ������δ�����
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);

        }
    }

    //�ܻ���϶���
    public void ResetAttack()
    {
        anim.ResetTrigger("Attack");//���ù����Ĵ�����
    }

    public void ResetHit()
    {
        anim.ResetTrigger("Hit");//���ý�ֱ
    }

    public void ResetDizzy()
    {
        anim.ResetTrigger("Dizzy");//����ѣ��
    }


    public void EndNotify()
    {
        //�����ʤ����
        anim.SetBool("Win", true);
        playerDead = true;
        //ֹͣ�ƶ�
        isChase = false;
        isWalk = false;
        attackTarget = null;

        //ֹͣagent

    }
}
