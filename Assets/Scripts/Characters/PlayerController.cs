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

    private GameObject attackTarget;//��������

    private float lastAttackTime;//����������CD

    private bool isDead;

    public float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        //ÿ����Ϸ��ʼʱ������ѽ�ɫ�ĵ�ǰѪ������Ϊ���Ѫ��
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
        isDead = characterStats.CurrentHealth == 0;//�˴�������˳���������ұߵ��Ƿ���ڵ�bool���㣬Ȼ���ٽ�boolֵ��ֵ��isDead
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
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);//�˴���sqrMagnitude�ǽ�vector3��ֵת��Ϊ������
        anim.SetBool("Death", isDead);
    }



    //��������ʱע���Ӧ��mousemanager
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//�������Э�̣������ƶ��Ĺ���
        if(isDead)
        {
            return;
        }
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;//�˴���Ϊ�˱����ڵ������֮�������û���ƶ���
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead)
        {
            return;
        }
        if (target != null)//������Ŀ�겻Ϊ��ʱ
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

        transform.LookAt(attackTarget.transform);//�˴��Ƕ�Ӧ���˷������ת��


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
            //������ȴʱ��
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //�����¼�
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
