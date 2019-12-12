using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Linq;

//事件
public enum NetEvent
{
    ConnectSucc = 1,
    ConnectFail = 2,
    Close = 3,
}

public static class NetManager
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //事件委托类型
    public delegate void EventListener(String err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //添加事件监听
    public static void AddEventListener(NetEvent netEvent,EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        //新增事件
        else
        {
            eventListeners[netEvent] = listener;
        }
    }

    //删除事件监听
    public static void RemoveEventListener(NetEvent netEvent,EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            //删除
            if(eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    //分发事件
    private static void FireEvent(NetEvent netEvent,String err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }

    //是否正在连接
    static bool isConnecting = false;

    //连接
    public static void Connect(string ip,int port)
    {
        //状态判断
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail,already connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail,isConnecting");
        }
        //初始化成员
        InitState();
        //参数设置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    //初始化状态
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接
        isConnecting = false;
    }

    //Connect回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail " + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }
}