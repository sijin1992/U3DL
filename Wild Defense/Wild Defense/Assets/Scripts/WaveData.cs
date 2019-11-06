using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//一定要添加该属性这个类才能序列化
public class WaveData {

    public int wave = 0;
    public List<GameObject> enemyPrefab;//每波敌人的Prefab
    public int level = 1;//敌人的等级
    public float interval = 3;//每3秒创建一个敌人
}
