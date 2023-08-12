using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拓展类
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)//this后的时扩展对应的类，“,”号后的才是函数的变量
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;

    }
}
