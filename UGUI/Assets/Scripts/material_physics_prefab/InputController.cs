using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (!Application.isMobilePlatform)
        {
            //移动平台之外的处理
            if (Input.GetMouseButtonUp(0))
            {
                //按下鼠标左键并释放的状态
                //调用Cube Generator组件的Generate方法
                GetComponent<CubeGenerator>().Generate();
            }
            else
            {
                //移动平台的处理
                if (Input.touchCount >= 1)
                {
                    //获取多次触摸中的第一次触摸
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        //触摸开始后，调用Cube Generator组件的Generate方法
                        GetComponent<CubeGenerator>().Generate();
                    }
                }
            }
        }
	}
}
