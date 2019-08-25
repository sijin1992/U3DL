using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour {
    [SerializeField] private GameObject cubePrefab;//立方体的预制
    public void Generate()
    {
        //将立方体的预制实例化
        GameObject obj = Instantiate(cubePrefab) as GameObject;
        //将实例设为"CubeGnerator"对象的子元素
        obj.transform.SetParent(transform);
        //使实例的比例与预制相匹配
        obj.transform.localScale = cubePrefab.transform.localScale;
        //使实例的位置与"CubeGnerator"对象相匹配
        obj.transform.position = transform.position;
        //设置随机的旋转角度以使每次下落时旋转角度都会发生变化
        obj.transform.rotation = Random.rotation;
    }
}
