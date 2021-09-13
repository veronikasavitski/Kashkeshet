using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ServerBootstrapper
    {
        SocketListener SocketListener = new SocketListener();
        public void Start()
        {
            SocketListener.StartServer();
        }
    }
}
