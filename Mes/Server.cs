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

        int authentication = -1;
        bool online = true;
        DB_Manager mesDB;
        Logger terminalLog;
        TestHarness terminalTest;

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
            terminalLog = new Logger();
            mesDB = new DB_Manager();
            mesDB.setConnection();
            terminalTest = new TestHarness();
            Console.WriteLine("Intializing Security Server.");
            terminalLog.appendLog("Security Server Initializating");
            while (authentication == -1)
            {
                Console.WriteLine("Please Enter Your Username");
                String username = Console.ReadLine();
                Console.WriteLine("Please Enter Your Password");
                String password = Console.ReadLine();
                authentication = mesDB.checkAuthentication(username, password);
                if (authentication == -1)
                {
                    Console.WriteLine("Incorrect Credentials Please Try Again!");
                    terminalLog.appendLog("Warning invalid credentials were entered!");
                }
            }
            terminalLog.appendLog("Connection to the server has been established.");
            Console.WriteLine("------------_________---------------");
            Console.WriteLine("------------|       |---------------");
            Console.WriteLine("---*CLICK*--________|---------------");
            Console.WriteLine("------------|       |---------------");
            Console.WriteLine("------------|   O   |---------------");
            Console.WriteLine("------------|_______|---------------");
            Console.WriteLine("Welcome to Mesmerize One Prime X2");
            Console.WriteLine("");
            while (online)
            {
                String command = Console.ReadLine();

                switch (command.ToUpper())
                {
                    case "ADDSENSOR":
                        Console.WriteLine("Add Sensor:");
                        if (MessageQueue.Exists(queueName))
                        {
                            terminalLog.appendLog("Add Sensor Command Executed");
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
                        terminalLog.appendLog("Edit Sensor Command Executed");
                        break;
                    case "REMOVESENSOR":
                        Console.WriteLine("Remove Sensor:");
                        terminalLog.appendLog("Remove Sensor Command Executed");
                        break;
                    case "VIEWSENSORS":
                        Console.WriteLine("Viewing Sensor");
                        terminalLog.appendLog("View Sensor Command Executed");
                        break;
                    case "TEST":
                        Console.WriteLine("Please specify the file path to the test file.");
                        terminalLog.appendLog("Executing Test Simulation of Security System");
                        terminalTest.File = Console.ReadLine();
                        terminalTest.printCommands(terminalTest.getCommands());
                        terminalLog.appendLog("Test Simulation of Security System Complete");
                        break;
                    case "EXIT":
                        Console.WriteLine("Command Terminal shutting down!");
                        terminalLog.appendLog("Connection to server has been terminated by the user.");
                        online = false;
                        break;
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        terminalLog.appendLog("Invalid Command Entered: " + command);
                        break;
                }
                Thread.Sleep(0);
            }
        }
    }

    class Server
    {
        DB_Manager mesDB;
        List<GenericSystem> systemList;
        Thread terminalThread;
        List<Thread> systemThreads;

        bool running;

        public Server()
        {
            systemList = new List<GenericSystem>();
            mesDB = new DB_Manager();
            systemThreads = new List<Thread>();

            
            mesDB.setConnection();
            mesDB.createCredentialsTable();
            mesDB.createSystemTable();
            mesDB.createSensorTable();
            mesDB.createMonitorTable();
            mesDB.createAlarmTable();

            systemList = mesDB.getSystems();
            if (systemList.Count == 0)
            {
                AddSecuritySystem();
            }            
        }

        public void Run()
        {
            running = true;
            startSystems();
            Terminal serverTerminal = new Terminal();
            terminalThread = new Thread(new ThreadStart(serverTerminal.runTerminal));
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

        public void AddSecuritySystem()
        {
            SecuritySystem securitySystem = new SecuritySystem();
            int id = mesDB.addSystem(1);
            if (id > 0)
            {
                securitySystem.Id = id;
                systemList.Add(securitySystem);
                Thread securitySystemThread = new Thread(new ThreadStart(securitySystem.Run));
                securitySystemThread.Start();
            }
            
        }

        public void startSystems()
        {
            for (int i = 0; i < systemList.Count; i++)
            {
                systemThreads.Add(new Thread(new ThreadStart(systemList[i].Run)));
            }
        }

    }
}
