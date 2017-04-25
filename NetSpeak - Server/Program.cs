using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Security;
namespace NetSpeak___Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.None;
            int port = 0;
            SecureString password = new SecureString();

            bool autostart = false;
            if (args.Length != 0)
            {
                if (args[0] == "-autostart")
                {
                    autostart = true;
                }
                if (args.Length >= 2)
                {
                    if (args[1].Contains("-ip="))
                    {
                        ip = IPAddress.Parse(args[1].Substring(args[1].IndexOf('=') + 1));
                    }
                    if (args.Length >= 3)
                    {
                        if (args[2].Contains("-port="))
                        {
                            port = int.Parse(args[2].Substring(args[2].IndexOf('=') + 1));
                        }
                        if (args.Length >= 4)
                        {
                            if (args[3].Contains("-password="))
                            {
                                 foreach (char c in args[3].Substring(args[3].IndexOf('=') + 1))
                                {
                                    password.AppendChar(c);
                                }
                            }
                        }
                    }
                } 
            }
            

            
            Console.WriteLine("Welcome to NetSpeak server");
            Console.WriteLine("Server offline...");
            if (ip == IPAddress.None)
            {
                Console.WriteLine("IP to bind the server to (type 0 to listen on any ip): ");
                string response = Console.ReadLine();

                if (response == "0")
                {
                    ip = IPAddress.Any;
                }
                else
                {
                    ip = IPAddress.Parse(response);
                }
            }
            else
            {
                Console.WriteLine("Ip address alredy set");
            }

            if (port == 0)
            {
                Console.WriteLine("Port to bind the server to: ");
                port = int.Parse(Console.ReadLine());
            }
            else
            {
                Console.WriteLine("Port alredy set");
            }
            
            if (password == null)
            {
                Console.WriteLine("Do you want to enter a password? Press enter if not:");
                string temp;
                temp = Console.ReadLine();
                int tries = 0;
                while (tries < 3)
                {
                    if (temp != String.Empty)
                    {
                        Console.WriteLine("Type the password again:");
                        if (temp == Console.ReadLine())
                        {
                            foreach (char c in temp)
                            {
                                password.AppendChar(c);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The password do not match! {0}Retry");
                            tries++;
                        }
                    }
                }
                temp = null;
                if (password == null)
                {
                    Console.WriteLine("Too many failures; password not set!");
                }
            }

            IPEndPoint endpoint = new IPEndPoint(ip, port);

            if (!autostart)
            {
                Console.WriteLine("Type 'start' to start the server");

            }
            else
            {
                CallServer(endpoint, password);
                Console.WriteLine("Server starting...");
                while (true)
                {
                    if (Console.ReadLine() == "stop")
                    {
                        Environment.Exit(1);
                    }
                }
            }

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "start")
                {
                    CallServer(endpoint, password);
                    Console.WriteLine("Server starting...");
                }
                else if (input == "stop")
                {
                    Environment.Exit(1);
                    
                }
            }
        }

        private static async void CallServer(IPEndPoint e, SecureString pass)
        {
            Server server = new Server();
            await server.server(e, pass);

        }
    }
}
