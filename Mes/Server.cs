using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mes
{

    public class Terminal
    {
        
        bool online = true;

        public void startTerminal()
        {
            online = true;
        }

        public void stopTerminal()
        {
            online = false;
        }

        public bool terminalState()
        {
            return online;
        }

        public void runTerminal()
        {

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
                        Console.WriteLine("Command Terminal shutting down!");
                        online = false;
                        break;
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        break;
                }
                Thread.Sleep(0);
            }
        }
    }

    class Server
    {

        static void Main(string[] args)
        {

            bool running = true;
            Terminal serverTerminal = new Terminal();
            Thread terminalThread = new Thread(new ThreadStart(serverTerminal.runTerminal));

            terminalThread.Start();

            while (running)
            {

                Thread.Sleep(1000);

                if (!serverTerminal.terminalState())
                {
                    Console.WriteLine("Server shutting down!");
                    Thread.Sleep(2000);
                    terminalThread.Abort();
                    running = false;
                    break;
                }
            }
        }
    }
}
