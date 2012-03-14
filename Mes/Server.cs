using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class Server
    {
        static void Main(string[] args)
        {

            bool online = true;

            Console.WriteLine("Intializing Security Server.");
            Console.WriteLine("Welcome to Mesmerize One Prime X2");

            while (online)
            {

                String command = Console.ReadLine();

                switch (command.ToUpper())
                {
                    case "ADDSENSOR":
                        Console.WriteLine("Add Sensor:");
                        break;
                    case "EDITSENSOR":
                        Console.WriteLine("Edit Sensor:");
                        break;
                    case "REMOVESENSOR":
                        Console.WriteLine("Remove Sensor:");
                        break;
                    case "VIEWSENSORS":
                        Console.WriteLine("Adding Sensor");
                        break;
                    case "EXIT":
                        Console.WriteLine("Server is now shutting down...");
                        break;
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        break;
                }
            }
        }
    }
}
