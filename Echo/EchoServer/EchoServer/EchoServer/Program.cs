using System;
using System.Net;
using System.Net.Sockets;

namespace EchoServer
{
    class MainClass
    {
        static void Main(string[] args)
        {
            //Socket
            Socket Listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            Listenfd.Bind(ipEp);
            //Listen
            Listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            while (true){
                //Accept
                Socket connfd = Listenfd.Accept();
                Console.WriteLine("[服务器]Accept");
                //Receive
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string readStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器]" + readStr);
                //Send
                string sendtime = System.DateTime.Now.ToString();
                readStr = sendtime + ":" + readStr;
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(readStr);
                connfd.Send(sendBytes);
            }
        }
    }
}
