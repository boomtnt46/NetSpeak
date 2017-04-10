using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetSpeak___Server
{
    class Server
    {
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
        private const byte nickVersionByte = 1;
        private const byte messageVersionByte = 0;
        public void ConnectionManager(Socket ClientSocket)
        {
            new Thread(new ParameterizedThreadStart(this.ManageClient)).Start(ClientSocket);
        }

        private void ManageClient(object clientObj)
        {
            try
            {
                byte[] buffer;
                Socket socket = (Socket)clientObj;
                string nick = "";
                while (true)
                {
                    while (socket.Available == 0)
                    {
                        Thread.Sleep(50);
                    }
                    buffer = new byte[socket.Available];
                    socket.Receive(buffer);
                    byte d = (byte)buffer.GetValue(0);
                    if (d == messageVersionByte)
                    {
                        Console.WriteLine(nick + ":" + Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1));
                    }
                    else if (d == nickVersionByte)
                    {
                        if (nick != "")
                        {
                            Console.WriteLine("User changer its nick: " + nick  + "  -------->  " + Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1));
                        }
                        nick = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
 
