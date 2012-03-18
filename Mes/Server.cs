using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Messaging;

namespace Mes
{

    public class Terminal
    {
        // Needed for sending messages to the security system to handle
        string queueName = @".\Private$\security";

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

        public void printTerminal(string outString)
        {
            Console.WriteLine(outString);
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
                        if (MessageQueue.Exists(queueName))
                        {
                            MessageQueue queue = new MessageQueue(queueName);
                            queue.Send("Add Sensor message","Label");
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
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
            DB_Manager mesDB = new DB_Manager();
            mesDB.setConnection();
            mesDB.createSensorTable();
            mesDB.createMonitorTable();
            mesDB.createAlarmTable();

            Terminal serverTerminal = new Terminal();
            SecuritySystem securitySystem = new SecuritySystem();

            Thread terminalThread = new Thread(new ThreadStart(serverTerminal.runTerminal));
            Thread securitySystemThread = new Thread(new ThreadStart(securitySystem.Run));

            terminalThread.Start();
            securitySystemThread.Start();
            
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
