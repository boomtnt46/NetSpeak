using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetSpeak___Server
{
    class Server
    {
        //declaration of variables
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Propagate propagate = new Propagate();
        private int clientNumber = 1;

        //Constants declaration
        private const byte messagevByte = 0;
        private const byte nickvByte = 1;
        private const byte pingvByte = 2;

        //methods
        public Task server(EndPoint endpoint)
        {
            return Task.Run(() =>
            {
                listener.Bind(endpoint);
                listener.Listen(100);
                while (true)
                {
                    CallConnectionHandler(listener.Accept());
                    Console.WriteLine("New client connected: {0}", clientNumber);
                    clientNumber++;

                }
            });
        }


        private async void CallConnectionHandler(Socket ClientSocket)
        {
            await ConnectionHandler(ClientSocket, propagate);
        }

        private Task ConnectionHandler(Socket client, Propagate propagate)
        {
            string nick = "";

            byte[] buffer;
            Socket socket = client;

            DateTime keepAlive = DateTime.Now;

            propagate.list.Add(socket);
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        buffer = new byte[4096]; //Max message size
                        socket.Receive(buffer);

                        byte versionByte = (byte)buffer.GetValue(0);
                        if (versionByte == messagevByte)
                        {
                            ReceiveMessage(propagate, nick, buffer);
                        }
                        else if (versionByte == nickvByte)
                        {
                            nick = ReceiveNick(propagate, nick, buffer);
                        }
                        else if (versionByte == pingvByte)
                        {
                            keepAlive = DateTime.Now;

                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is SocketException && keepAlive.Second + 10 > DateTime.Now.Second)
                    {
                        Console.WriteLine(e.ToString());
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Client '{0}' disconnected", nick);
                        Console.ForegroundColor = ConsoleColor.White;
                        socket.Shutdown(SocketShutdown.Both);

                        socket.Disconnect(true);
                        socket.Close();

                        clientNumber--;

                        return;
                    }
                    else if (e is SocketException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Lost connection with a client. The client '{0}' probably suffered an unexpected error. {1}No ping from '{0}' was received in the last 10 seconds.", nick, Environment.NewLine);
                        Console.ForegroundColor = ConsoleColor.White;
                        socket.Shutdown(SocketShutdown.Both);

                        socket.Disconnect(true);
                        socket.Close();

                        clientNumber--;

                        return;
                    }
                    else
                    {
                        Console.WriteLine(e.ToString());
                        socket.Shutdown(SocketShutdown.Both);

                        socket.Disconnect(true);
                        socket.Close();
                        clientNumber--;

                        return;
                    }
                }
            });
        }

        private static string ReceiveNick(Propagate propagate, string nick, byte[] buffer)
        {
            Byte[] messageLengthBytes = new byte[2];
            Buffer.BlockCopy(buffer, 1, messageLengthBytes, 0, 2);
            int length = (int)BitConverter.ToInt16(messageLengthBytes, 0);

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
            return nick;
        }

        private static void ReceiveMessage(Propagate propagate, string nick, byte[] buffer)
        {
            byte[] messageLengthBytes = new byte[2];
            Buffer.BlockCopy(buffer, 1, messageLengthBytes, 0, 2);
            int length = BitConverter.ToInt16(messageLengthBytes, 0); //the length of bytes of the message

            byte[] messageBytes = new byte[length];
            Buffer.BlockCopy(buffer, 3, messageBytes, 0, length);

            string message = nick + ":" + Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine(message);
            propagate.PropagateMessages(message);
        }

        private Task PongAsync(Socket socket)
        {
            return Task.Run(() =>
            {

            });
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
 
