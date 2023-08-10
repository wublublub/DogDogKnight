using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]//�˴���Ӧ������˵����⹥��

    public float kickForce = 10;//�����ö�Ӧ��һ���ƿ�����

    public void KickOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform.position);//���������������ȵ��������

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//�˴��ǽ�����淶����Ҳ�������������ƽ���͵���1��

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//�ȴ����ҵ��ƶ�
            attackTarget.GetComponent <NavMeshAgent>().velocity = direction * kickForce;//�ٶȵ��ڷ�����������˴��Ǽ򻯵�ģ���ʾ������������ʵ������
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//����ұ����ʱ�����ѣ��
        }
    }
    
}
