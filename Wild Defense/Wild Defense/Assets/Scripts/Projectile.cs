using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    //当打击到目标时执行的动作
    System.Action<Enemy> onAttack;
    //目标对象
    Transform m_target;
    //目标对象模型的边框
    Bounds m_targetCenter;
    //静态函数 创建弓箭
    public static void Create(Transform target, Vector3 spawnPos, System.Action<Enemy> onAttack)
    {
        //读取弓箭模型
        GameObject prefab = Resources.Load<GameObject>("arrow");
        GameObject go = (GameObject)Instantiate(prefab, spawnPos, Quaternion.LookRotation(target.position - spawnPos));
        //添加弓箭脚本组件
        Projectile arrowmodel = go.AddComponent<Projectile>();
        //设置弓箭的目标
        arrowmodel.m_target = target;
        //获得目标模型的边框
        arrowmodel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        //取得Action
        arrowmodel.onAttack = onAttack;
        //3秒之后自动销毁
        Destroy(go, 3.0f);
    }
	void Update () {
        //瞄准目标中心位置
        if (m_target != null)
            this.transform.LookAt(m_targetCenter.center);
        //向目标前进
        this.transform.Translate(new Vector3(0, 0, 10 * Time.deltaTime));
        if (m_target!=null)
        {
            //简单通过距离检测是否打击到目标
            if (Vector3.Distance(this.transform.position,m_targetCenter.center)<0.5f)
            {
                //通知弓箭发射者
                onAttack(m_target.GetComponent<Enemy>());
                //销毁
                Destroy(this.gameObject);
            }
        }
	}
}
