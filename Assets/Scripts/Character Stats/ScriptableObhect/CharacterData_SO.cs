using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]//创建一个子集菜单
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;



    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//升级经验值的倍率

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }

    }

    public void UpdateExp(int point)
    {
        currentExp += point;

        if(currentExp >= baseExp)
        {
            LeveUp();
        }
    }

    private void LeveUp()
    {
        //所有你想要提升的属性
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("Level UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
