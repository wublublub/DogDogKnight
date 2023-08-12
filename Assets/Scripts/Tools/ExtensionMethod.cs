using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��չ��
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)//this���ʱ��չ��Ӧ���࣬��,���ź�Ĳ��Ǻ����ı���
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;

    }
}
