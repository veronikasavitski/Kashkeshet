using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class MyTcpClient
    {
        private static readonly Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public MyTcpClient()
        {

        }

        public void StartClient()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            while (!clientSocket.Connected)
            {
                try
                {
                    Console.WriteLine("Connection attempt");

                    clientSocket.Connect(IPAddress.Loopback, 4445);

                }
                catch(SocketException)
                {
                    Console.Clear();
                }

            }

            Console.Clear();
            Console.WriteLine("Connected");
           
        }

        private static void RequestLoop()
        {
            Console.WriteLine("type exit to Disconnect");
            //to recieve response and also send another message

            while(true)
            {
                Task.Run(() => SendRequest());
                ReceiveResponse();
            }


        }

        private static void Exit()
        {
            SendString("exit");
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Environment.Exit(0);
            
        }

        private static void SendRequest()
        {
            Console.WriteLine("Send a request :");
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "exit")
            {
               Exit();
            }
        } 


        private static void SendString(string txt)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(txt);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

        }

        private static void ReceiveResponse()
        {
            byte[] buffer = new byte[2048];
            int recieved = clientSocket.Receive(buffer, SocketFlags.None);
          

            if (recieved == 0) return;

            byte[] data = new byte[recieved];
            Array.Copy(buffer, data, recieved);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text); //write what is recieved fro server
          
        }

    }
}
