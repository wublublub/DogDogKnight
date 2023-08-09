using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;//��Ϊģ���������ڽ��������˹���ͬһģ�嵼��״̬ͬ����ͬ��������������

    public CharacterData_SO characterData;

    public AttackData_SO attackData;



    [HideInInspector]//�˴����ڼ���������������ѡ���������Ȼ���������������н��з���
    public bool isCritical;//�ж��Ƿ񱩻�

    private void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    //ʹ�����Եķ���
    public int MaxHealth 
    {
        get
        {
            if(characterData != null)//��ǰ����������ݵ�ģ��
            {
                return characterData.maxHealth;
            }
            else { return 0; }
        }
        set
        {
            characterData.maxHealth = value;//�˴���value�����ⲿ��������Ը���ֵ
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)//��ǰ����������ݵ�ģ��
            {
                return characterData.currentHealth;
            }
            else { return 0; }
        }
        set
        {
            characterData.currentHealth = value;//�˴���value�����ⲿ��������Ը���ֵ
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)//��ǰ����������ݵ�ģ��
            {
                return characterData.baseDefence;
            }
            else { return 0; }
        }
        set
        {
            characterData.baseDefence = value;//�˴���value�����ⲿ��������Ը���ֵ
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)//��ǰ����������ݵ�ģ��
            {
                return characterData.currentDefence;
            }
            else { return 0; }
        }
        set
        {
            characterData.currentDefence = value;//�˴���value�����ⲿ��������Ը���ֵ
        }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker,CharacterStats defener)//��ȡ������ɫ��һ���ǹ�����һ���Ƿ�����
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        defener.CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//ȷ���Ƿ�����һ����Ѫ������

        if(isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //TODO:����UI��ʾ�µ�Ѫ��
        //TODO:��һ�ȡ����ֵ
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        //��������˱��������˺����ϱ���ϵ��
        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("������"+coreDamage);
        }

        return (int)coreDamage;
    }




    #endregion

}
