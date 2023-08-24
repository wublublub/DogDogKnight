using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;//拿到主角的预制体，便于后续场景切换时加载角色

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
        //保存数据
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)//如果场景名不同则为异常景传送
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//在等待异步场景加载完全后再执行下面的代码
            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();//切换场景后会重新读取数据
            yield break;//都加载完成后执行该代码从协程中跳出去
        }
        else
        {
            //同场景传送
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();//获取组件
            playerAgent.enabled = false;//在传送之前关闭NavMeshAgent组件避免对传送产生影响
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;//传送完成后重新启用组件
            yield return null;
        }

    }

    //返回角色应该传送到的坐标
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

        return null;//默认返回空
    }

}
