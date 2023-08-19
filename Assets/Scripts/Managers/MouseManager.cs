using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//由于EventVector3并没有继承MonoBehaviour，所以需要进行系统序列化
/*
[System.Serializable]
public class EventVector3 : UnityEvent<Vector3>//需要一个vector3变量让人物知道往哪里移动的事件
{

}
*/

public class MouseManager : Singleton<MouseManager>
{
    RaycastHit hitInfo;//作为射线碰撞的信息（此处为实现鼠标点按移动的模块）


    //public EventVector3 OnMouseClicked;//此处设置出一个类似于UI按钮的点击事件，并且要求了输入必须是vector3变量

    //使用unity内置的事件
    public event Action<Vector3> OnMouseClicked;//点击地面的事件
    public event Action<GameObject> OnEnemyClick;//点击敌人的事件


    public Texture2D point, doorway, attack, target, arrow;//此处对应鼠标的五种光标



    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }



    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture()//设置当鼠标在游戏的地面上时的指针贴图（此处为实现鼠标点按移动的模块）
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//设置一条射线，起点是摄像机，终点是鼠标的位置


        if (Physics.Raycast(ray, out hitInfo))//此处规定了输入的射线以及输出的值hitInfor
        {
            //切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//此处的vector2适用于鼠标的指向偏移值，设鼠标大小为32x32，则左上角偏移值为16x16

                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);//此处对应当鼠标放在敌人身上时更改鼠标贴图
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;

            }

        }
    }

    public void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //所有注册了该事件的方法都会调用
                OnMouseClicked?.Invoke(hitInfo.point); //检查当前事件是否为空，如果为空则执行后续的内容

            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);//此处是将碰撞的碰撞体的游戏对象传入事件
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);//此处是将碰撞的碰撞体的游戏对象传入事件
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {

            }
        }
    }


}
