using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    public static GameCamera Inst = null;
    //摄像机距离地面的距离
    protected float m_distance = 10;
    //摄像机的角度
    protected Vector3 m_rot = new Vector3(-70, 180, 0);
    //摄像机的移动速度
    protected float m_moveSpeed = 60;
    //摄像机的移动值
    protected float m_vx = 0;
    protected float m_vy = 0;
    //Transform组件
    protected Transform m_transform;
    //摄像机的焦点
    protected Transform m_cameraPoint;

    void Awake()
    {
        Inst = this;
        m_transform = this.transform;
    }
    void Start () {
        //获得摄像机的焦点
        m_cameraPoint = CameraPoint.Instance.transform;
        Follow();
	}
	//在Update之后执行
	void LateUpdate () {
        Follow();
	}

    //摄像机对齐到焦点的位置和角度
    void Follow()
    {
        //设置旋转角度
        m_cameraPoint.eulerAngles = m_rot;
        //将摄像机移动到指定位置
        m_transform.position = m_cameraPoint.TransformPoint(new Vector3(0, 0, m_distance));
        //将摄像机镜头对准目标点
        transform.LookAt(m_cameraPoint);
    }
    //控制摄像机移动
    public void Control(bool mouse,float mx,float my)
    {
        if (!mouse)
            return;
        m_cameraPoint.eulerAngles = Vector3.zero;
        //平移摄像机目标点
        m_cameraPoint.Translate(-mx, 0, -my);
    }
}
