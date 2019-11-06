using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;//UI控件命名空间的引用
using UnityEngine.Events;//UI事件命名空间的引用
using UnityEngine.EventSystems;//UI事件命名空间的引用


public class GameManager : MonoBehaviour {

    public static GameManager Instance; //实例
    public LayerMask m_groundlayer;     //地面的碰撞Layer
    public int m_wave = 1;              //波数
    public int m_waveMax = 10;          //最大波数
    public int m_life = 10;             //生命
    public int m_point = 30;            //铜钱数量

    public bool m_debug = true;//显示路点的debug开关
    public List<PathNode> m_PathNodes;//路点

    public List<Enemy> m_EnemyList = new List<Enemy>();

    //UI文字控件
    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;
    //UI重新游戏按钮控件
    Button m_but_try;
    //当前是否选中的创建防守单位的按钮
    bool m_isSelectedButton = false;

    void Awake()
    {
        Instance = this;
    }


    void Start () {
        //创建UnityAction,在OnButCreateDefenderDown函数中响应按钮按下事件
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        //创建UnityAction,在OnButCreateDefenderDown函数中响应按钮抬起事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);
        //按钮按下事件
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);
        //按钮抬起事件
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        //查找所有子物体，根据名称获取UI控件
        foreach (Transform t in this.GetComponentInChildren<Transform>())
        {
            if (t.name.CompareTo("wave") == 0)//找到文字控件"波数"
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);//设为第1波
            } else if (t.name.CompareTo("life") == 0)//找到文字控件“生命”
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("生命:<color=yellow>{0}</color>", m_life);
            } else if (t.name.CompareTo("point") == 0)//找到文字控件"铜钱"
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_point.text = string.Format("铜钱:<color=yellow>{0}</color>", m_point);
            } else if (t.name.CompareTo("but_try")==0)//找到按钮控件“重新游戏”
            {
                m_but_try = t.GetComponent<Button>();
                //重新游戏按钮
                m_but_try.onClick.AddListener(delegate () { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
                //默认隐藏重新游戏按钮
                m_but_try.gameObject.SetActive(false);
            } else if (t.name.Contains("but_player"))//找到按钮控件“创建防守单位”
            {
                //给防守单位按钮添加按钮事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }
	}

    void Update()
    {
        //如果选中创建士兵的按钮，则取消摄像机操作
        if (m_isSelectedButton)
            return;
        //鼠标或触屏操作，注意不同平台的Input代码不同
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
    bool press = Input.touches.Length >0 ? true:false;//手指是否触屏
    float mx = 0;
    float my = 0;
    if (press)
	{
        if (Input.GetTouch(0).phase == TouchPhase.Moved)//获得手指移动距离
	    {
        mx = Input.GetTouch(0).deltaPosition.x * 0.01f;
        my = Input.GetTouch(0).deltaPosition.y * 0.01f;
	    }
	}
#else
        bool press = Input.GetMouseButton(0);
        //获得鼠标移动距离
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
#endif
        //移动摄像机
        GameCamera.Inst.Control(press, mx, my);
    }
    //更新文字控件“波数”
    public void SetWave(int wave)
    {
        m_wave = wave;
        m_txt_wave.text = string.Format("波数:<color=yellow>{0}/{1}</color>",m_wave,m_waveMax);
    }
    //更新文字控件"生命"
    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            m_but_try.gameObject.SetActive(true);//显示重新游戏按钮
        }
        m_txt_life.text = string.Format("生命:<color=yellow>{0}</color>", m_life);
    }
    //更新文字控件“铜钱”
    public bool SetPoint(int point)
    {
        if (m_point + point < 0 )//如果铜钱数量不够
        {
            return false;
        }
        m_point += point;
        m_txt_point.text = string.Format("铜钱:<color=yellow>{0}</color>", m_point);
        return true;
    }
    //按下“创建防守单位按钮”
    void OnButCreateDefenderDown(BaseEventData data)
    {
        m_isSelectedButton = true;
    }
    //抬起“创建防守单位按钮”
    void OnButCreateDefenderUp(BaseEventData data)
    {
        //创建射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        bool ispeng = Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer);
        print(ispeng);
        //检测是否与地面相碰撞
        if (Physics.Raycast(ray,out hitinfo,1000,m_groundlayer))
        {
            //如果选中的是一个可用的格子
            if (TileObject.Instance.getDataFromPosition(hitinfo.point.x,hitinfo.point.z)==(int)Defender.TileStatus.GUARD)
            {
                //获得碰撞点位置
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);
                //获得Grid Object坐标位置
                Vector3 gridPos = TileObject.Instance.transform.position;
                //获得格子大小
                float tilesize = TileObject.Instance.tileSize;
                //计算出所点击格子的中心位置
                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tilesize) * tilesize + tilesize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tilesize) * tilesize + tilesize * 0.5f;

                //获得选择的按钮GameObject,将简单通过按钮名字判断选择了哪个按钮
                GameObject go = data.selectedObject;
                if (go.name.Contains("1"))//如果按钮名字包括“1”
                {
                    if (SetPoint(-15))//减15个铜钱，然后创建近战防守单位
                    Defender.Create<Defender>(hitpos, new Vector3(0, 180, 0));
                }
                else if (go.name.Contains("2"))//如果按钮名字包括"2"
                {
                    if (SetPoint(-20))//减20个铜钱，然后创建远程防守单位
                        Defender.Create<Archer>(hitpos, new Vector3(0, 180, 0));
                }
            }
        }
        m_isSelectedButton = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new List<PathNode>();
        //通过路点的Tag查找所有的路点
        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");
        for (int i = 0; i < objs.Length; i++)
        {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());
        }
    }

    void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null)
            return;
        Gizmos.color = Color.blue;//将路点连线的颜色设为蓝色
        foreach (PathNode node in m_PathNodes)//遍历路点
        {
            if (node.m_next!=null)
            {
                //在路点之间画出连接线
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }

}
