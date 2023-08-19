using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene, DifferenceScene
    }

    [Header("Transition Info")]//���ڴ��͵���Ϣ
    public string sceneName;

    public TransitionType transitionType;//���������жϴ��͵����͵�ö�ٱ���

    public TransitionDestination.DestinationTag destinationTag;//�ƶ���ö�ٱ���

    private bool canTrans;//�ж��ܷ���

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) &&canTrans)
        {
            //ִ��SceneManager����
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;//�����ɫ�ı�ǩ���������Ա�����
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;//�������뿪��������򲻿ɴ���
        }
    }

}
