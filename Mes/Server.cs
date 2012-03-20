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

        //Terminal Attributes
        int authentication = -1;
        bool online = true;
        DB_Manager mesDB;
        Logger terminalLog;
        TestHarness terminalTest;

        //User Credentials
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

        // Terminal Process
        public void runTerminal()
        {
            //Connects to Logging and database
            terminalLog = new Logger();
            mesDB = new DB_Manager();
            mesDB.SetConnection();
            terminalTest = new TestHarness();

            Console.WriteLine("Intializing Security Server.");
            terminalLog.appendLog("Security Server Initializating");
            
            //Set up message passing to Security System
            MessageQueue queue = new MessageQueue(queueName);
            MesMessage mesMsg = new MesMessage();

            //User Login Verification
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
            //Terminal Startup
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
            Console.WriteLine("     ADDUSER");
            Console.WriteLine("     CHANGEPASS");
            Console.WriteLine("     ADD");
            Console.WriteLine("     REMOVE");
            Console.WriteLine("     EDIT");
            Console.WriteLine("     VIEW");
            Console.WriteLine("     STATUS");
            Console.WriteLine("     ARM");
            Console.WriteLine("     DISARM");
            Console.WriteLine("     UNTRIGGER");
            Console.WriteLine("     ENABLESIM");
            Console.WriteLine("     DISABLESIM");
            Console.WriteLine("     CHANGEREADING");
            Console.WriteLine("     EXIT");
            Console.WriteLine("");
            //Terminal Process
            while (online)
            {
                //Command Retriever
                String command = Console.ReadLine();

                //Input Parameters
                string deviceType = null;
                string deviceCategory = null;
                string isEnable = null;
                string threshold = null;
                string location = null;
                string deviceId = null;
                int tmp;

                //Command Decoder
                switch (command.ToUpper())
                {
                    case "LOGOUT":
                        //User Login Verification
                        Console.WriteLine("You have successfully logged out!");
                        authentication = -1;
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
                        //Terminal Startup
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
                        Console.WriteLine("     ADDUSER");
                        Console.WriteLine("     CHANGEPASS");
                        Console.WriteLine("     ADD");
                        Console.WriteLine("     REMOVE");
                        Console.WriteLine("     EDIT");
                        Console.WriteLine("     VIEW");
                        Console.WriteLine("     STATUS");
                        Console.WriteLine("     ARM");
                        Console.WriteLine("     DISARM");
                        Console.WriteLine("     UNTRIGGER");
                        Console.WriteLine("     ENABLESIM");
                        Console.WriteLine("     DISABLESIM");
                        Console.WriteLine("     CHANGEREADING");
                        Console.WriteLine("     EXIT");
                        Console.WriteLine("");
                        break;
                    case "CHANGEPASS":
                        //-----------------------------------------------------------------------------------
                        //Change Pass Command
                        //-----------------------------------------------------------------------------------
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
                        //-----------------------------------------------------------------------------------
                        // Add User Command
                        //-----------------------------------------------------------------------------------
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
                        //-----------------------------------------------------------------------------------
                        // Add Device Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Add Device:");
                        if (MessageQueue.Exists(queueName))
                        {
                            terminalLog.appendLog("Add Sensor Command Executed.");
                            mesMsg.type = "ADD";
                            
                            Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine().ToUpper();
                            while (deviceCategory != "SENSOR" && deviceCategory != "MONITOR" && deviceCategory != "ALARM") {
                                Console.WriteLine("Please enter the valid category of the device.\n(SENSOR|MONITOR|ALARM)");
                                deviceCategory = Console.ReadLine().ToUpper();
                            }

                            if (deviceCategory == "SENSOR") {
                                Console.WriteLine("Please enter the type of the device.\n(MAGNETIC|MOTION|FLOOD|SMOKE)");
                                deviceType = Console.ReadLine().ToUpper();
                                while (deviceType != "MAGNETIC" && deviceType != "MOTION" && deviceType != "FLOOD" && deviceType != "SMOKE")
                                {
                                    Console.WriteLine("Please enter the valid type of the device.\n(MAGNETIC|MOTION|FLOOD|SMOKE|LIGHT|SIREN|VIDEO)");
                                    deviceType = Console.ReadLine().ToUpper();
                                }
                            } else if (deviceCategory == "ALARM") {
                                Console.WriteLine("Please enter the type of the device.\n(LIGHT|SIREN)");
                                deviceType = Console.ReadLine().ToUpper();
                                while (deviceType != "LIGHT" && deviceType != "SIREN")
                                {
                                    Console.WriteLine("Please enter the valid type of the device.\n(LIGHT|SIREN)");
                                    deviceType = Console.ReadLine().ToUpper();
                                }
                            } else {
                                Console.WriteLine("Please enter the type of the device.\n(VIDEO)");
                                deviceType = Console.ReadLine().ToUpper();
                                while(deviceType != "VIDEO") {
                                    Console.WriteLine("Please enter the valid type of the device.\n(VIDEO)");
                                    deviceType = Console.ReadLine().ToUpper();
                                }
                            }

                            
                            Console.WriteLine("Please enter if the device should be enabled.\n(TRUE|FALSE)");
                            isEnable = Console.ReadLine().ToUpper();
                            while (isEnable != "TRUE" && isEnable != "FALSE")
                            {
                                Console.WriteLine("Please enter if the device should be enabled.\n(TRUE|FALSE)");
                                isEnable = Console.ReadLine().ToUpper();
                            }

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
                        //-----------------------------------------------------------------------------------
                        // Edit Device Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Edit Device:");
                        terminalLog.appendLog("Edit Sensor Command Executed");
                        mesMsg.type = "EDIT";

                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine().ToUpper();
                        while (deviceCategory != "SENSOR" && deviceCategory != "MONITOR" && deviceCategory != "ALARM")
                        {
                            Console.WriteLine("Please enter the valid category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine().ToUpper();
                        }

                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        while (!(int.TryParse(deviceId, out tmp))) 
                        {
                            Console.WriteLine("Please enter a valid id of the device");
                            deviceId = Console.ReadLine();
                        }

                        Console.WriteLine("Please enter if the device should be enabled.\n(TRUE|FALSE)");
                        isEnable = Console.ReadLine().ToUpper();
                        while (isEnable != "TRUE" && isEnable != "FALSE")
                        {
                            Console.WriteLine("Please enter if the device should be enabled.\n(TRUE|FALSE)");
                            isEnable = Console.ReadLine().ToUpper();
                        }

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
                        //-----------------------------------------------------------------------------------
                        //Remove Device Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Remove Device:");
                        terminalLog.appendLog("Remove Sensor Command Executed");
                        mesMsg.type = "REMOVE";

                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine().ToUpper();
                        while (deviceCategory != "SENSOR" && deviceCategory != "MONITOR" && deviceCategory != "ALARM")
                        {
                            Console.WriteLine("Please enter the valid category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine().ToUpper();
                        }

                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        while (!(int.TryParse(deviceId, out tmp))) 
                        {
                            Console.WriteLine("Please enter a valid id of the device");
                            deviceId = Console.ReadLine();
                        }
                       
                        mesMsg.message = deviceId + "," + deviceType + "," + deviceCategory + "," + isEnable + "," + "0" + "," + location;
                        queue.Send(mesMsg);
                        break;
                    case "VIEW":
                        //-----------------------------------------------------------------------------------
                        //View Device Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Viewing Devices");
                        terminalLog.appendLog("View Sensor Command Executed");
                        mesMsg.type = "VIEW";

                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine().ToUpper();
                        while (deviceCategory != "SENSOR" && deviceCategory != "MONITOR" && deviceCategory != "ALARM")
                        {
                            Console.WriteLine("Please enter the valid category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine().ToUpper();
                        }

                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        while (!(int.TryParse(deviceId, out tmp)) && deviceId != "") 
                        {
                            Console.WriteLine("Please enter a valid id of the device");
                            deviceId = Console.ReadLine();
                        }

                        mesMsg.message = deviceId + "," + deviceType + "," + deviceCategory + "," + isEnable + "," + "0" + "," + location;
                        queue.Send(mesMsg);
                        break;
                    case "TEST1234567890":
                        //-----------------------------------------------------------------------------------
                        //Test Command 
                        //-----------------------------------------------------------------------------------
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
                        //-----------------------------------------------------------------------------------
                        //Enable Simulator Command
                        //-----------------------------------------------------------------------------------
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
                        //-----------------------------------------------------------------------------------
                        //Disable Simulator Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Stopping Simulation.");
                        terminalLog.appendLog("DisableSim");
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
                        //-----------------------------------------------------------------------------------
                        //Change reading value of sensors command
                        //-----------------------------------------------------------------------------------
                        if (MessageQueue.Exists(queueName))
                        {
                            mesMsg.type = "CHANGEREADING";
                            Console.WriteLine("Please enter the id of the sensor.");
                            string id = Console.ReadLine();
                            while (!(int.TryParse(id, out tmp)))
                            {
                                Console.WriteLine("Please enter a valid id the sensor.");
                                id = Console.ReadLine();
                            }
                            Console.WriteLine("Please enter the a reading of the sensor.");
                            string reading = Console.ReadLine();
                            while (!(int.TryParse(reading, out tmp)))
                            {
                                Console.WriteLine("Please enter the a valid reading of the sensor.");
                                reading = Console.ReadLine();
                            }
                            
                            mesMsg.message = id + "," + reading;
                            queue.Send(mesMsg);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "STATUS":
                        //-----------------------------------------------------------------------------------
                        //Display Status Command
                        //-----------------------------------------------------------------------------------
                        mesMsg.type = "STATUS";
                        mesMsg.message = "";
                        queue.Send(mesMsg);
                        break;
                    case "ARM":
                        //-----------------------------------------------------------------------------------
                        //Arm Devices Command
                        //-----------------------------------------------------------------------------------
                        mesMsg.type = "ARM";
                        mesMsg.message = "";
                        queue.Send(mesMsg);
                        break;
                    case "DISARM":
                        //-----------------------------------------------------------------------------------
                        //Disarm Devices Command
                        //-----------------------------------------------------------------------------------
                        mesMsg.type = "DISARM";
                        mesMsg.message = "";
                        queue.Send(mesMsg);
                        break;
                    case "UNTRIGGER":
                        //-----------------------------------------------------------------------------------
                        //Untrigger Device Command
                        //-----------------------------------------------------------------------------------
                        mesMsg.type = "UNTRIGGER";
                        Console.WriteLine("Please enter the category of the device.\n(SENSOR|MONITOR|ALARM)");
                        deviceCategory = Console.ReadLine().ToUpper();
                        while (deviceCategory != "SENSOR" && deviceCategory != "MONITOR" && deviceCategory != "ALARM") {
                            Console.WriteLine("Please enter the valid category of the device.\n(SENSOR|MONITOR|ALARM)");
                            deviceCategory = Console.ReadLine().ToUpper();
                        }

                        Console.WriteLine("Please enter the id of the device");
                        deviceId = Console.ReadLine();
                        while (!(int.TryParse(deviceId, out tmp))) 
                        {
                            Console.WriteLine("Please enter a valid id of the device");
                            deviceId = Console.ReadLine();
                        }
                        
                        mesMsg.message = deviceId + "," + deviceCategory;
                        queue.Send(mesMsg);
                        break;
                    case "EXIT":
                        //-----------------------------------------------------------------------------------
                        //Exit Terminal Command
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Command Terminal shutting down!");
                        terminalLog.appendLog("Connection to server has been terminated by the user.");
                        online = false;
                        break;
                    default:
                        //-----------------------------------------------------------------------------------
                        //Displays the help to the system
                        //-----------------------------------------------------------------------------------
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        terminalLog.appendLog("Invalid Command Entered: " + command);
                        Console.WriteLine("Valid commands are:");
                        Console.WriteLine("     ADDUSER");
                        Console.WriteLine("     CHANGEPASS");
                        Console.WriteLine("     ADD");
                        Console.WriteLine("     REMOVE");
                        Console.WriteLine("     EDIT");
                        Console.WriteLine("     VIEW");
                        Console.WriteLine("     STATUS");
                        Console.WriteLine("     ARM");
                        Console.WriteLine("     DISARM");
                        Console.WriteLine("     UNTRIGGER");
                        Console.WriteLine("     ENABLESIM");
                        Console.WriteLine("     DISABLESIM");
                        Console.WriteLine("     CHANGEREADING");
                        Console.WriteLine("     EXIT");
                        break;
                }
                Thread.Sleep(0);
            }
        }
    }

    class Server
    {
        //Connetion to database, systems, and threads.
        DB_Manager mesDB;
        List<GenericSystem> systemList;
        Thread terminalThread;
        List<Thread> systemThreads;

        //Runs the Server
        bool running;

        public Server()
        {
            systemList = new List<GenericSystem>();
            mesDB = new DB_Manager();
            systemThreads = new List<Thread>();

            // Initializes the Database Tables
            mesDB.SetConnection();
            mesDB.CreateCredentialsTable();
            mesDB.CreateSystemTable();
            mesDB.CreateSensorTable();
            mesDB.CreateMonitorTable();
            mesDB.CreateAlarmTable();

            //Initializes the Database Table
            systemList = mesDB.GetSystems();
            if (systemList.Count == 0)
            {
                AddSecuritySystem();
            }
        }

        // Run Instance of the Server
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

                //Drives the Terminal of the server
                if (!serverTerminal.terminalState())
                {
                    Console.WriteLine("Server shutting down!");
                    terminalThread.Abort();
                    running = false;   
                    break;
                }
            }
        }

        //Initializes the Security System
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

        //Starts the Systems that have been initialized
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
