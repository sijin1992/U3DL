using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //ByteArrayTest();
        NetManagerTest();
	}

    private void ByteArrayTest()
    {
        //[1 创建]
        ByteArray buff = new ByteArray(8);
        Debug.Log("[1 debug] ->" + buff.Debug());
        Debug.Log("[1 string] ->" + buff.ToString());
        //[2 write]
        byte[] wb = new byte[] { 1, 2, 3, 4, 5 };
        buff.Write(wb, 0, 5);
        Debug.Log("[2 debug] ->" + buff.Debug());
        Debug.Log("[2 string] ->" + buff.ToString());
        //[3 read]
        byte[] rb = new byte[4];
        buff.Read(rb, 0, 2);
        Debug.Log("[3 debug] - >" + buff.Debug());
        Debug.Log("[3 string] ->" + buff.ToString());
        Debug.Log("[3 rb] - >" + BitConverter.ToString(rb));
        //[4 write,resize]
        wb = new byte[] { 6, 7, 8, 9, 10, 11 };
        buff.Write(wb, 0, 6);
        Debug.Log("[4 debug] ->" + buff.Debug());
        Debug.Log("[4 string] ->" + buff.ToString());
    }

    private void NetManagerTest()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
    }

    //玩家点击连接按钮
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
        //TODO:开始转圈圈,提示“连接中”
    }

    //主动关闭
    public void OnCloseClick()
    {
        NetManager.Close();
    }

    //连接成功回调
    void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
        //TODO:进入游戏
    }

    //连接失败回调
    void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail" + err);
        //TODO:弹出提示框
    }

    //关闭连接
    void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose");
        //TODO:弹出提示框(网络断开)
        //TODO:弹出按钮(重新连接)
    }
}
