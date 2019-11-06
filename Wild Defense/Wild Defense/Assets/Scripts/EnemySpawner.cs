using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public PathNode m_startNode;//起始节点
    private int m_liveEnemy = 0;//存活的敌人数量
    public List<WaveData> waves;//战斗波数配置数组
    int enemyIndex = 0;         //生成敌人数组的下标
    int waveIndex = 0;          //战斗波数数组的下标

	void Start () {
        StartCoroutine(SpawnEnemies());//开始生成敌人
	}

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForEndOfFrame();//保证在Start函数后执行
        GameManager.Instance.SetWave((waveIndex + 1));//设置UI上的波数显示
        WaveData wave = waves[waveIndex];//获得当前波的配置
        yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔
        while (enemyIndex < wave.enemyPrefab.Count)//如果没有生成全部敌人
        {
            Vector3 dir = m_startNode.transform.position - this.transform.position;//初始方向
            GameObject enmeyObj = (GameObject)Instantiate(wave.enemyPrefab[enemyIndex], transform.position, Quaternion.LookRotation(dir));//创建敌人
            Enemy enemy = enmeyObj.GetComponent<Enemy>();//获得敌人的脚本
            enemy.m_currentNode = m_startNode;//设置敌人的第一个路点
            //设置敌人数组，这里只是简单示范
            //数值配置适合放到一个专用的数据库(SQLite数据库或者JSON、XML格式的配置)中读取
            enemy.m_life = wave.level * 3;
            enemy.m_maxlife = enemy.m_life;
            m_liveEnemy++;//增加敌人数量
            enemy.onDeath = new System.Action<Enemy>((Enemy e) => { m_liveEnemy--; });
            enemyIndex++;//更新敌人数组下标
            yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔
        }
        //创建完全部敌人，等待敌人全部被消灭
        while (m_liveEnemy > 0)
        {
            yield return 0;
        }
        enemyIndex = 0;//重置敌人数组下标
        waveIndex++;//更新战斗波数
        if (waveIndex < waves.Count)//如果不是最后一波
        {
            print("waveIndex:"+ waveIndex);
            print("count:" + waves.Count);
            StartCoroutine(SpawnEnemies());//继续生成后面的敌人
        }
        else
        {
            //通知胜利
            print("Victory!");
        }
    }
    //在编辑器中显示一个图标
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.tif");
    }
}
