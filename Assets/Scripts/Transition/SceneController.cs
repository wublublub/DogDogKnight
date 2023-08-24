using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;//�õ����ǵ�Ԥ���壬���ں��������л�ʱ���ؽ�ɫ

    GameObject player;

    NavMeshAgent playerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferenceScene: 
                StartCoroutine(Transition(transitionPoint.sceneName,transitionPoint.destinationTag));

                break;
        }


    }
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //��������
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)//�����������ͬ��Ϊ�쳣������
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//�ڵȴ��첽����������ȫ����ִ������Ĵ���
            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();//�л�����������¶�ȡ����
            yield break;//��������ɺ�ִ�иô����Э��������ȥ
        }
        else
        {
            //ͬ��������
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();//��ȡ���
            playerAgent.enabled = false;//�ڴ���֮ǰ�ر�NavMeshAgent�������Դ��Ͳ���Ӱ��
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;//������ɺ������������
            yield return null;
        }

    }

    //���ؽ�ɫӦ�ô��͵�������
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }

        return null;//Ĭ�Ϸ��ؿ�
    }

}
