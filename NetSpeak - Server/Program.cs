using System;
using System.Collections.Generic;
using System.Linq;
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

            Console.WriteLine("Type 'start' to start the server");

            IPEndPoint endpoint = new IPEndPoint(ip, port);

            Server server = new Server();
            Task InitServer;
            InitServer =  new Task(new Action(() => { server.server(endpoint); }));

            bool Continue = true;
            while (Continue)
            {
                if (Console.ReadLine() == "start")
                {
                    //This one runs the server in a separate thread, making the console responisve.
                    InitServer.Start();
                    //This one runs the server in the current thread, making the console NOT responsive. USE JUST FOR DEBUGGING!
                    //server.server(endpoint);
                    
                }
                if (Console.ReadLine() == "stop")
                {
                    InitServer.Dispose(); //Currently thows a exception, I need further investigation
                    
                }
            }
        }
    }
}
