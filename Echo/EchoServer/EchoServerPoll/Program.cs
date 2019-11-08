using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace EchoServerPoll
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }

    class MainClass
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
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
            //主循环
            while (true)
            {
                //检查listenfd
                if (listenfd.Poll(0,SelectMode.SelectRead))
                {
                    ReadListenfd(listenfd);
                }
                //检查clientfd
                foreach (ClientState s in clients.Values)
                {
                    Socket clientfd = s.socket;
                    if (clientfd.Poll(0,SelectMode.SelectRead))
                    {
                        if (!ReadClientfd(clientfd))
                        {
                            break;
                        }
                    }
                }
                //防止CPU占用过高
                System.Threading.Thread.Sleep(1);
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
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Receive SocketException " + ex.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            //广播
            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine("Receive" + recvStr);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
            return true;
        }
    }
}


