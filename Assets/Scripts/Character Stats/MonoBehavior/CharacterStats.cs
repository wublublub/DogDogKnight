using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;//作为模板数据用于解决多个敌人共用同一模板导致状态同步（同生共死）的问题

    public CharacterData_SO characterData;

    public AttackData_SO attackData;



    [HideInInspector]//此处是在检查器界面隐藏这个选项，但是我仍然可以在其他代码中进行访问
    public bool isCritical;//判断是否暴击
    public bool isBeenHit;//主要用于实现角色受击，眩晕时进行操作禁用

    private void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    //使用属性的方法
    public int MaxHealth 
    {
        get
        {
            if(characterData != null)//当前面板上有数据的模板
            {
                return characterData.maxHealth;
            }
            else { return 0; }
        }
        set
        {
            characterData.maxHealth = value;//此处的value代表外部对这个属性赋的值
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)//当前面板上有数据的模板
            {
                return characterData.currentHealth;
            }
            else { return 0; }
        }
        set
        {
            characterData.currentHealth = value;//此处的value代表外部对这个属性赋的值
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)//当前面板上有数据的模板
            {
                return characterData.baseDefence;
            }
            else { return 0; }
        }
        set
        {
            characterData.baseDefence = value;//此处的value代表外部对这个属性赋的值
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)//当前面板上有数据的模板
            {
                return characterData.currentDefence;
            }
            else { return 0; }
        }
        set
        {
            characterData.currentDefence = value;//此处的value代表外部对这个属性赋的值
        }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker,CharacterStats defener)//获取两个角色，一个是攻击方一个是防御方
    {

        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        defener.CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//确保是防御的一方的血量减少
        //只有小怪的暴击才能触发打断受击，大怪都是靠技能
        if(attacker.isCritical && attacker.attackData.skillRange == 0)
        {
            defener.GetComponent<Animator>().ResetTrigger("Attack");
            defener.GetComponent<Animator>().ResetTrigger("Skill");
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //TODO:更新UI显示新的血量
        //TODO:玩家获取经验值
    }

    public void TakeDamage(int damage, CharacterStats defener)//函数的重载，专门写一个投掷的石头的伤害的函数
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        //如果发生了暴击，则将伤害乘上暴击系数
        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！"+coreDamage);
        }

        return (int)coreDamage;
    }





    #endregion

}
