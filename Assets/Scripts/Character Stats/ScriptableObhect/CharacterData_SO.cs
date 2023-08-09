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

}
