using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class Start
    {
        static void Main(string[] args)
        {
            // Starts the execution of the server
            Server server = new Server();
            server.Run();
            Console.WriteLine("It is now safe to close your terminal.");
        }
    }
}
