using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class ClientBootstrapper
    {
        MyTcpClient myTcpClient = new MyTcpClient();

        public void Start()
        {
            myTcpClient.StartClient();
        }
    }
}
