using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 30;

    public GameObject rockPerfab;//��Ӧ��Ͷ����ʯͷ

    public Transform handPos;//Ͷ��ʱ�ֵ�λ��

    //�����¼�
    public void KickOff()
    {
        if (attackTarget != null)
        {
            attackTarget.GetComponent<Animator>().ResetTrigger("Attack");
            transform.LookAt(attackTarget.transform.position);//���������������ȵ��������

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//�˴��ǽ�����淶����Ҳ�������������ƽ���͵���1��

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//�ȴ����ҵ��ƶ�
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//�ٶȵ��ڷ�����������˴��Ǽ򻯵�ģ���ʾ������������ʵ������
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//����ұ����ʱ�����ѣ��
        }
    }

    public void ThrowRock()
    {
        

        if(attackTarget != null)
        {
            var rock = Instantiate(rockPerfab,handPos.position,Quaternion.identity);//�˴���ʯͷԤ���壬Ͷ����λ�ã���ʼ�ĽǶ�

            rock.GetComponent<Rock>().target = attackTarget;


        }
    }

}
