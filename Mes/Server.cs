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

        String activeUserName;
        String activePassword;

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
            mesDB.SetConnection();
            terminalTest = new TestHarness();
            Console.WriteLine("Intializing Security Server.");
            terminalLog.appendLog("Security Server Initializating");
            MessageQueue queue = new MessageQueue(queueName);
            MesMessage mesMsg = new MesMessage();

            while (authentication == -1)
            {
                Console.WriteLine("Please Enter Your Username");
                activeUserName = Console.ReadLine();
                Console.WriteLine("Please Enter Your Password");
                activePassword = Console.ReadLine();
                authentication = mesDB.CheckAuthentication(activeUserName, activePassword);
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
            Console.WriteLine("Valid commands are:");
            Console.WriteLine("     ADD");
            Console.WriteLine("     REMOVE");
            Console.WriteLine("     EDIT");
            Console.WriteLine("     VIEW");
            Console.WriteLine("     TEST");
            Console.WriteLine("     EXIT");
            Console.WriteLine("");
            while (online)
            {
                String command = Console.ReadLine();
                string deviceType = null;
                string deviceCategory = null;
                string isEnable = null;
                string threshold = null;
                string location = null;
                string deviceId = null;
                int tmp;

                switch (command.ToUpper())
                {
                    case "CHANGEPASS":
                            Console.WriteLine("Enter Your Password");
                            string oldPass = Console.ReadLine();
                            if (oldPass.Equals(activePassword))
                            {
                                Console.WriteLine("Enter New Password");
                                string password = Console.ReadLine();
                                mesDB.ChangePassword(activeUserName, password);
                                Console.WriteLine("Password Changed");
                            }
                            else
                            {
                                Console.WriteLine("Incorrect Password");
                            }
                        break;
                    case "ADDUSER":
                        if (authentication == 1)
                        {
                            Console.WriteLine("[New User] Please Enter Username");
                            string username = Console.ReadLine();
                            Console.WriteLine("[New User] Please Enter Password");
                            string password = Console.ReadLine();
                            Console.WriteLine("[New User] Please Enter Type");
                            int type = Convert.ToInt16(Console.ReadLine());
                            mesDB.AddUser(username, password, type);
                        }
                        else
                        {
                            Console.WriteLine("You do not have permissions to add users");
                        }
                        break;
                    case "ADD":
                        Console.WriteLine("Add Device:");
                        if (MessageQueue.Exists(queueName))
                        {
                            // "id,deviceType(magnetic),category(sensor),enable,threshold,location"
                            terminalLog.appendLog("Add Sensor Command Executed.");
                            mesMsg.type = "ADD";
                            Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine();
                            Console.WriteLine("Please enter the type of the device.\n(MAGNETIC|MOTION|FLOOD|SMOKE|LIGHT|SIREN|VIDEO)");
                            deviceType = Console.ReadLine();
                            Console.WriteLine("Please enter the if the device should be enabled.\n(TRUE|FALSE)");
                            isEnable = Console.ReadLine();
                            Console.WriteLine("Please enter the (THRESHOLD|SENSITIVITY) number setting.");
                            threshold = Console.ReadLine();
                            while (!(int.TryParse(threshold, out tmp)))
                            {
                                Console.WriteLine("Please enter a valid (THRESHOLD|SENSITIVITY) number setting.");
                                threshold = Console.ReadLine();
                            }
                            Console.WriteLine("Please enter the location of the device.");
                            location = Console.ReadLine();
                            mesMsg.message = "0," + deviceType + "," + deviceCategory + "," + isEnable + "," + threshold + "," + location;
                            queue.Send(mesMsg);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "EDIT":
                        Console.WriteLine("Edit Device:");
                        terminalLog.appendLog("Edit Sensor Command Executed");
                        mesMsg.type = "EDIT";
                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine();
                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        while (!(int.TryParse(deviceId, out tmp))) 
                        {
                            Console.WriteLine("Please enter a valid id of the device");
                            deviceId = Console.ReadLine();
                        }
                        Console.WriteLine("Please enter the if the device should be enabled.\n(TRUE|FALSE)");
                        isEnable = Console.ReadLine();
                        Console.WriteLine("Please enter the (THRESHOLD|SENSITIVITY) number setting.");
                        threshold = Console.ReadLine();
                        while (!(int.TryParse(threshold, out tmp)))
                        {
                            Console.WriteLine("Please enter a valid (THRESHOLD|SENSITIVITY) number setting.");
                            threshold = Console.ReadLine();
                        }
                        Console.WriteLine("Please enter the location of the device.");
                        location = Console.ReadLine();
                        mesMsg.message = deviceId + "," + deviceType + "," + deviceCategory + "," + isEnable + "," + threshold + "," + location;
                        queue.Send(mesMsg);
                        break;
                    case "REMOVE":
                        Console.WriteLine("Remove Device:");
                        terminalLog.appendLog("Remove Sensor Command Executed");
                        mesMsg.type = "REMOVE";
                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine();
                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        mesMsg.message = deviceId + "," + deviceType + "," + deviceCategory + "," + isEnable + "," + "0" + "," + location;
                        queue.Send(mesMsg);
                        break;
                    case "VIEW":
                        Console.WriteLine("Viewing Devices");
                        terminalLog.appendLog("View Sensor Command Executed");
                        mesMsg.type = "VIEW";
                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine();
                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        mesMsg.message = deviceId + "," + deviceType + "," + deviceCategory + "," + isEnable + "," + "0" + "," + location;
                        queue.Send(mesMsg);
                        break;
                    case "TEST":
                        Console.WriteLine("Please specify the file path to the test file.");
                        terminalLog.appendLog("Executing Test Simulation of Security System");
                        terminalTest.File = Console.ReadLine();
                        if (MessageQueue.Exists(queueName))
                        {
                            foreach (string cmd in terminalTest.getCommands())
                            {
                                mesMsg.type = "LOG";
                                mesMsg.message = cmd;
                                queue.Send(mesMsg);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "ENABLESIM":
                        Console.WriteLine("Starting Simulation.");
                        terminalLog.appendLog("EnableSim");
                        if (MessageQueue.Exists(queueName))
                        {
                                mesMsg.type = "ENABLESIM";
                                mesMsg.message = "";
                                queue.Send(mesMsg);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "DISABLESIM":
                        Console.WriteLine("Stopping Simulation.");
                        terminalLog.appendLog("EnableSim");
                        if (MessageQueue.Exists(queueName))
                        {
                            mesMsg.type = "DISABLESIM";
                            mesMsg.message = "";
                            queue.Send(mesMsg);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "CHANGEREADING":
                        if (MessageQueue.Exists(queueName))
                        {
                            mesMsg.type = "CHANGEREADING";
                            Console.WriteLine("Please enter the id of the sensor.");
                            String id = Console.ReadLine();
                            Console.WriteLine("Please enter it's new reading.");
                            String reading = Console.ReadLine();
                            mesMsg.message = id + "," + reading;
                            queue.Send(mesMsg);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "EXIT":
                        Console.WriteLine("Command Terminal shutting down!");
                        terminalLog.appendLog("Connection to server has been terminated by the user.");
                        online = false;
                        break;
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        terminalLog.appendLog("Invalid Command Entered: " + command);
                        Console.WriteLine("Valid commands are:");
                        Console.WriteLine("     ADD");
                        Console.WriteLine("     REMOVE");
                        Console.WriteLine("     EDIT");
                        Console.WriteLine("     VIEW");
                        Console.WriteLine("     TEST");
                        Console.WriteLine("     EXIT");
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

            mesDB.SetConnection();
            mesDB.CreateCredentialsTable();
            mesDB.CreateSystemTable();
            mesDB.CreateSensorTable();
            mesDB.CreateMonitorTable();
            mesDB.CreateAlarmTable();

            systemList = mesDB.GetSystems();
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
                    terminalThread.Abort();
                    running = false;
                    Thread.Sleep(2000);   
                    break;
                }
            }
        }

        public void AddSecuritySystem()
        {
            SecuritySystem securitySystem = new SecuritySystem();
            int id = mesDB.AddSystem(1);
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
                systemThreads[i].Start();
            }
        }
    }
}
