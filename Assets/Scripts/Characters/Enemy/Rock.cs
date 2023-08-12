using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates {HitPlayer, HitEnemy, HitNothing }//ʯͷ������״̬���ֱ��Ӧײ����ң�ײ�����ײ������

    private Rigidbody rb;

    public RockStates rockStates;//��¼��ǰ��ʯͷ��״̬

    [Header("Basic Settings")]
    public float force;//Ͷ����ʯͷ������

    public int damage;//Ͷ����ʯͷ������ɵ��˺�

    public GameObject target;//Ͷ����Ŀ��

    public Vector3 direction;//���еķ���

    public GameObject breakEffect;//��Ҫ���ڴ���������Ч

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;//��ʼ����ʱ��Ϊ�˴����ҵ�
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
            target = FindObjectOfType<PlayerController>().gameObject;//ע�⣬�˴��������ڳ�����ֻ��һ�����ǵ����
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch(rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player") && rb.velocity.sqrMagnitude > 1f)//�����ײ�������
                {
                    //����ҵ��������һ���˺�
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent <NavMeshAgent>().velocity = direction * force;


                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//���ѣ�ε�Ч��
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
