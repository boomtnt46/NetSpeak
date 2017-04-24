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
            bool autostart = false;
            if (args[0] == "-autostart")
            {
                autostart = true;
            }
            Console.WriteLine("Welcome to NetSpeak server");
            Console.WriteLine("Server offline...");

            Console.WriteLine("IP to bind the server to (type 0 to listen on any ip): ");
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

            if (!autostart)
            {
                Console.WriteLine("Type 'start' to start the server");

            }
            else
            {
                CallServer(endpoint);
                Console.WriteLine("Server starting...");
            }

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "start")
                {
                    CallServer(endpoint);
                    Console.WriteLine("Server starting...");
                }
                else if (input == "stop")
                {
                    Environment.Exit(1);
                    
                }
            }
        }

        private static async void CallServer(IPEndPoint e)
        {
            Server server = new Server();
            await server.server(e);

        }
    }
}
