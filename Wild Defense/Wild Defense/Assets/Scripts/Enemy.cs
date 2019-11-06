using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public PathNode m_currentNode;      //敌人的当前路点
    public int m_life = 15;             //敌人的生命
    public int m_maxlife = 15;          //敌人的最大生命
    public float m_speed = 2;           //敌人的移动速度
    public System.Action<Enemy> onDeath;//敌人的死亡事件
    public float nextvalue = 0.0f;

    Transform m_lifebarObj;//敌人的UI生命条GameObject
    UnityEngine.UI.Slider m_lifebar;//控制生命条显示的Slider

    void Start()
    {
        GameManager.Instance.m_EnemyList.Add(this);

        //读取生命条prefab
        GameObject prefab = (GameObject)Resources.Load("Canvas3D");
        //创建生命条，将当前Transform设为父物体
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        if (this.name.Contains("pig"))
            m_lifebarObj.localPosition = new Vector3(0, 1.0f, 0);//将生命条放到角色头上
        if (this.name.Contains("bird"))
            m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            m_lifebarObj.localPosition = new Vector3(0, 0.01f, 0);
        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();
        //更新生命条位置和角度
        StartCoroutine(UpdateLifebar());
    }

    void Update()
    {
        RotateTo();
        MoveTo();
    }
    //转向目标
    public void RotateTo()
    {
        var position = m_currentNode.transform.position - transform.position;
        position.y = 0;     //保证仅旋转Y轴
        var targetRotation = Quaternion.LookRotation(position);//获得目标旋转角度
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime);//获得中间旋转角度
        nextvalue = next;
        this.transform.eulerAngles = new Vector3(0, next, 0);//旋转
    }

    //向目标移动
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 1.0f)//如果到达路点的位置
        {
            if (m_currentNode.m_next == null)//没有路点，说明已经到达我方基地
            {
                GameManager.Instance.SetDamage(1);//扣除1点伤害
                DestroyMe();
            }
            else
                m_currentNode = m_currentNode.m_next;//更新到下一个路点

        }
        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }
    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);
        Destroy(this.gameObject);//注意在实际项目中一般不要直接调用Destroy
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            //每消灭一个敌人增加一些铜钱
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }

    IEnumerator UpdateLifebar()
    {
        //更新生命条的值
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        //更新角度，如终面向摄像机
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0;//没有任何等待
        StartCoroutine(UpdateLifebar());//循环执行
    }
}
