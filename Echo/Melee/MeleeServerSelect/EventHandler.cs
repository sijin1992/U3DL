using System;

namespace MeleeServerSelect
{
    public class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            Console.WriteLine("OnDisconnect");
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            foreach (ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }
    }
}
