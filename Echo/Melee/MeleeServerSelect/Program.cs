using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeleeServerSelect
{
    public class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;
    }

    class MainClass
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            //checkRead
            List<Socket> checkRead = new List<Socket>();
            //主循环
            while (true)
            {
                //填充checkRead列表
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                //select
                Socket.Select(checkRead, null, null, 1000);
                //检查可读对象
                foreach (Socket s in checkRead)
                {
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
            }
        }
        //读取Listenfd
        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }
        //读取Clientfd
        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接收
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                //反射disconnect
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Receive SocketException " + ex.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                //反射disconnect
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            //反射Msg
            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine("Receive" + recvStr);
            string[] split = recvStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            Console.WriteLine("Msg" + msgName);
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            return true;
        }
        //Send
        public static void Send(ClientState cs , string sendStr)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            Console.WriteLine("Send success");
            cs.socket.Send(sendBytes);
        }
    }
}