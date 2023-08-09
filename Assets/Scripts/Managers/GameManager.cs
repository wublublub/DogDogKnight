using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{


    public CharacterStats playerStats;//设置为public方便其他代码进行访问

    //创造列表收集所有在GameManager中加载了IEndGameObserver接口的函数
    List<IEndGameObserver>endGameObservers = new List<IEndGameObserver>();


    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//向所有观察者广播
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }


}
