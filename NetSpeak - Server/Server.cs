using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetSpeak___Server
{
    class Server
    {
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        public void server(EndPoint endpoint)
        {
            listener.Bind(endpoint);
            listener.Listen(100);
            int num = 1;
            while (true)
            {
                new ServerSwitch().ConnectionManager(this.listener.Accept());
                Console.WriteLine("New client connected: {0}", num);
                num++;
            }
        }
    }

 


    class ServerSwitch
    {
        public void ConnectionManager(Socket ClientSocket)
        {
            new Thread(new ParameterizedThreadStart(this.ManageClient)).Start(ClientSocket);
        }

        private void ManageClient(object clientObj)
        {
            try
            {
                while (true)
                {
                    Socket socket = (Socket)clientObj;
                    byte[] buffer = new byte[socket.Available];
                    socket.Receive(buffer);
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
 
