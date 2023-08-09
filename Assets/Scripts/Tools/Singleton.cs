using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//注意，此工具类是作为泛型单例
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;//此处使用T用以确保类型与输入的类型相同
        }
    }

    //判断单例模式是否已经生成了
    public static bool IsInitialized
    {
        get{ return instance != null; }
    }

    protected virtual void OnDestory()
    {
        if (instance == this)//如果当前的示例被销毁的话则设置为空
        {
            instance = null;
        }
    }
}
