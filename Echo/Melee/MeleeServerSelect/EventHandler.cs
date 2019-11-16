using System;

namespace MeleeServerSelect
{
    public class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            Console.WriteLine("OnDisconnect");
        }
    }
}
