using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField]private float rotationSpeed;//设置旋转速度的字段
	// Use this for initialization
	void Start () {
        //rotationSpeed = 10.0f;//初始化旋转速度
        Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update () {
        //计算旋转角度
        float yAngle = rotationSpeed * Time.deltaTime;
        //使游戏对象以Y轴为基准旋转
        transform.Rotate(0.0f, yAngle, 0.0f);
	}
}
