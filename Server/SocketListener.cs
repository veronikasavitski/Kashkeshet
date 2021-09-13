using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace Server
{
    class SocketListener
    {
        private static Byte[] _buffer = new byte[2048];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int BUFFER_SIZE = 2048; 

        public SocketListener()
        {
        }

        public void StartServer()
        {
            Console.Title = "SERVER";
            SetupServer();
            Console.ReadKey(); //if the user pressed enter closing all sessions!!
            CloseAllSockets(); 

        }
        public void SetupServer()
        {
            Console.WriteLine("Setting up server..");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 4445));
            _serverSocket.Listen(100);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("server setup complete");

        }

        private static void CloseAllSockets()
        {
            foreach (Socket socket in _clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            _serverSocket.Close();
        }

        public static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;
            try
            {
                socket = _serverSocket.EndAccept(AR);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
            _clientSockets.Add(socket);
            
            socket.BeginReceive(_buffer, 0, BUFFER_SIZE, SocketFlags.None, RecieveCallback, socket); 
            Console.WriteLine("Client Connected");
            _serverSocket.BeginAccept(AcceptCallback, null);

        }


        private static void RecieveCallback(IAsyncResult AR)
        {
            
            Socket current = (Socket)AR.AsyncState;
            int received;
            try
            {
                received = current.EndReceive(AR);
 
            }
            catch(SocketException)
            {
                Console.WriteLine("client disconnected");
                current.Close();
                _clientSockets.Remove(current);
                return;
            }

            byte[] dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);

            string txt = Encoding.ASCII.GetString(dataBuffer); 
            
            Console.WriteLine("text received: " +txt);
            

            //response section - need to change to send all listeners!

            //string response = string.Empty;

            if (txt != null) //sends data and then exits
            {
                byte[] data = Encoding.ASCII.GetBytes(txt);

               //current.Send(data);
                broadcast(data);

                
                Console.WriteLine("sent to client");

            }
            else if(txt == "exit")
            {
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                _clientSockets.Remove(current);
                Console.WriteLine("client Disconnected");
                return;
            }

            else
            {
                
                byte[] dta = Encoding.ASCII.GetBytes("invalid request");
                current.Send(dta);
                //Console.WriteLine("warning sent");
            }

            current.BeginReceive(_buffer, 0, BUFFER_SIZE, SocketFlags.None, RecieveCallback, current);
            
        }

        public static void broadcast(byte[] msg)
        {
           //byte[] zerobuffer = new byte[2048];
            foreach (Socket socket in _clientSockets)
            {
                socket.Send(msg);
                //socket.Receive(zerobuffer);
                
                Console.WriteLine(socket.RemoteEndPoint);

            }
        }

    }
}
