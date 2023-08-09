using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{


    public CharacterStats playerStats;//����Ϊpublic��������������з���

    //�����б��ռ�������GameManager�м�����IEndGameObserver�ӿڵĺ���
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

    public void NotifyObservers()//�����й۲��߹㲥
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }


}
