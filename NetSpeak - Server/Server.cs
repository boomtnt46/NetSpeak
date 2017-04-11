using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetSpeak___Server
{
    class Server
    {
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Propagate prop = new Propagate();

        public void server(EndPoint endpoint)
        {
            listener.Bind(endpoint);
            listener.Listen(100);
            int num = 1;
            while (true)
            {
                /*new ServerSwitch().*/ConnectionManager(this.listener.Accept());
                Console.WriteLine("New client connected: {0}", num);
                num++;
                
            }
        }
    //}

    //class ServerSwitch
    //{
        private const byte nickVersionByte = 1;
        private const byte messageVersionByte = 0;
        public void ConnectionManager(Socket ClientSocket)
        {
            List<Object> list = new List<object>();
            list.Add(ClientSocket);
            list.Add(prop);
            new Thread(new ParameterizedThreadStart(ManageClient)).Start(list);
        }

        private void ManageClient(object general)
        {
            try
            {
                List<Object> ls = (List<Object>)general;
                Propagate propagate = (Propagate)ls[1];
                byte[] buffer;
                Socket socket = (Socket) ls[0];
                propagate.list.Add(socket);
                string nick = "";
                while (true)
                {
                    try
                    {
                        buffer = new byte[4096];
                        socket.Receive(buffer);

                        byte versionByte = (byte)buffer.GetValue(0);
                        if (versionByte == messageVersionByte)
                        {
                            
                            Byte[] messageLengthBytes = new byte[2];
                            Buffer.BlockCopy(buffer, 1, messageLengthBytes, 0, 2);
                            int length = BitConverter.ToInt16(messageLengthBytes, 0); //the length of bytes of the message

                            byte[] messageBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 3, messageBytes, 0, length);

                            string message = nick + ":" + Encoding.UTF8.GetString(messageBytes);
                            Console.WriteLine(message);
                            propagate.PropagateMessages(message);
                        }
                        else if (versionByte == nickVersionByte)
                        {
                            Byte[] messageLengthBytes = new byte[2];
                            Buffer.BlockCopy(buffer, 1, messageLengthBytes, 0, 2);
                            int length = (int) BitConverter.ToInt16(messageLengthBytes, 0);

                            byte[] messageBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 3, messageBytes, 0, length);
                            string newNick = Encoding.UTF8.GetString(messageBytes);

                            if (nick != String.Empty)
                            {
                                string message = "User changer its nick: " + nick + "  -------->  " + newNick;
                                Console.WriteLine(message);
                                propagate.PropagateMessages(message);
                            }
                            nick = newNick;
                        }

                    }
                    catch (Exception e)
                    {
                        if (e is SocketException)
                        {
                            Console.WriteLine(e.Message);
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SocketException)
                {
                    Console.WriteLine(e.Message);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }

    class Propagate
    {

        public List<Socket> list = new List<Socket>();

        public void PropagateMessages(string message)
        {
            for (int clientsNumber = 0; clientsNumber < list.Count; clientsNumber++)
            {
                list[clientsNumber].Send(Encoding.UTF8.GetBytes(message));
            }
            //Console.WriteLine("Message transmitted!");
        }
    }
}
 
