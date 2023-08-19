using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//����EventVector3��û�м̳�MonoBehaviour��������Ҫ����ϵͳ���л�
/*
[System.Serializable]
public class EventVector3 : UnityEvent<Vector3>//��Ҫһ��vector3����������֪���������ƶ����¼�
{

}
*/

public class MouseManager : Singleton<MouseManager>
{
    RaycastHit hitInfo;//��Ϊ������ײ����Ϣ���˴�Ϊʵ�����㰴�ƶ���ģ�飩


    //public EventVector3 OnMouseClicked;//�˴����ó�һ��������UI��ť�ĵ���¼�������Ҫ�������������vector3����

    //ʹ��unity���õ��¼�
    public event Action<Vector3> OnMouseClicked;//���������¼�
    public event Action<GameObject> OnEnemyClick;//������˵��¼�


    public Texture2D point, doorway, attack, target, arrow;//�˴���Ӧ�������ֹ��



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

    void SetCursorTexture()//���õ��������Ϸ�ĵ�����ʱ��ָ����ͼ���˴�Ϊʵ�����㰴�ƶ���ģ�飩
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//����һ�����ߣ��������������յ�������λ��


        if (Physics.Raycast(ray, out hitInfo))//�˴��涨������������Լ������ֵhitInfor
        {
            //�л������ͼ
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//�˴���vector2����������ָ��ƫ��ֵ��������СΪ32x32�������Ͻ�ƫ��ֵΪ16x16

                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);//�˴���Ӧ�������ڵ�������ʱ���������ͼ
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
                //����ע���˸��¼��ķ����������
                OnMouseClicked?.Invoke(hitInfo.point); //��鵱ǰ�¼��Ƿ�Ϊ�գ����Ϊ����ִ�к���������

            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);//�˴��ǽ���ײ����ײ�����Ϸ�������¼�
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClick?.Invoke(hitInfo.collider.gameObject);//�˴��ǽ���ײ����ײ�����Ϸ�������¼�
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {

            }
        }
    }


}
