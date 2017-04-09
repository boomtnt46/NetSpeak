using System;
using System.Collections.Generic;
using System.Linq;
using NetSpeak___Server;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace NetSpeak___Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to NetSpeak server");
            Console.WriteLine("Server offline...");

            Console.WriteLine("IP to bind the server to (write 0 to listen on any ip): ");
            string response = Console.ReadLine();

            IPAddress ip;

            if (response == "0")
            {
                ip = IPAddress.Any;
            }
            else
            {
                ip = IPAddress.Parse(response);
            }
  
            Console.WriteLine("Port to bind the server to: ");
            int port = int.Parse(Console.ReadLine());

            IPEndPoint endpoint = new IPEndPoint(ip, port);

            Server server = new Server();
            Task InitServer;
            InitServer =  new Task(new Action(() => { server.server(endpoint); }));
            if (Console.ReadLine() == "start")
            {
                InitServer.Start();
                //Task.Run(new Action(AsynchronousSocketListener.StartListening));
            }
            if (Console.ReadLine() == "stop")
            {
                
            }
        }
    }
}
