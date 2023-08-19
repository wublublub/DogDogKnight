using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene, DifferenceScene
    }

    [Header("Transition Info")]//关于传送的信息
    public string sceneName;

    public TransitionType transitionType;//创建用于判断传送的类型的枚举变量

    public TransitionDestination.DestinationTag destinationTag;//移动的枚举变量

    private bool canTrans;//判断能否传送

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) &&canTrans)
        {
            //执行SceneManager传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;//如果角色的标签是玩家则可以被传送
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;//如果玩家离开检测区域则不可传送
        }
    }

}
