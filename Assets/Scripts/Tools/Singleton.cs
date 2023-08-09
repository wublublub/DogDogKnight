using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ע�⣬�˹���������Ϊ���͵���
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
            instance = (T)this;//�˴�ʹ��T����ȷ�������������������ͬ
        }
    }

    //�жϵ���ģʽ�Ƿ��Ѿ�������
    public static bool IsInitialized
    {
        get{ return instance != null; }
    }

    protected virtual void OnDestory()
    {
        if (instance == this)//�����ǰ��ʾ�������ٵĻ�������Ϊ��
        {
            instance = null;
        }
    }
}
