using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Collections;
using System.Threading;
namespace SensorSim
{
    // A sensor extended to run as a thread and receive messages in a simulation
    class SimSensor : Mes.Sensor
    {
        MessageQueue queue;
        string queueName;
        int realSensorNum;

        public SimSensor(int number) {
            // format the queueName and check if it exists before making it
            realSensorNum = number;
            queueName = string.Format(@".\Private$\sensor{0}", realSensorNum);
            if (MessageQueue.Exists(queueName)){
                queue = new MessageQueue(queueName);
                Console.WriteLine("The queueName is taken and will be reused.");
            }
            else
            {
                queue = MessageQueue.Create(queueName);
                queue.Label = string.Format("Sensor {0} queue", realSensorNum);
                Console.WriteLine("Queue Created:");
                Console.WriteLine("Path: {0}, queue.Path");
                Console.WriteLine("FormatName: {0}, queue.FormatName");
            }
        }
        public void Run()
        {
            while(true)
            {
            }
        }
        ~SimSensor(){
            MessageQueue.Delete(queueName); // Delete the queue from the server
        }
    }

    class Program
    {
        // Enumerator for different sensor actions
        enum Actions {Add, Edit, Trig};

        // Needed for sending messages to the security system to handle
        static string queueName = @".\Private$\security";

        // Send a message to the central system with component configuration.
        // Retrieve an Id to save into the sensor
        static public SimSensor SimulatorAddSensor(ArrayList inList, int sensorCount)
        {
            
            SimSensor simSensor = new SimSensor(sensorCount);
            Message message = new Message(simSensor);
            message.Label = "add";
            if (MessageQueue.Exists(queueName))
            {
                MessageQueue queue = new MessageQueue(queueName);
                
                queue.Send(message);
            }
            else
            {
                Console.WriteLine("Terminal - Queue .\\security not Found");
            }
            return simSensor;
        }

        // Send a message to the central system to request component configuration for sensor of Id.
        // Retrieve configuration, edit it and send back to server.
        public void SimulatorEditSensor()
        {

        }

        // Send a message to the central system indicating intentional disconnection of sensor.
        public void SimulatorRemoveSensor()
        {

        }

        // Send a message to the central system indicating triggered sensor.
        public void SimulatorTriggerSensor()
        {

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Sensor Simulation for Mesmerize One Prime X2");

            // instantiate objects
            ArrayList realSensors = new ArrayList();
            List<Mes.Monitor> realMonitors = new List<Mes.Monitor>();
            List<Mes.Alarm> realAlarms = new List<Mes.Alarm>();
            Mes.SensorMessage sensorMessage = new Mes.SensorMessage();
            Hashtable threads = new Hashtable();
            int sensorCount = 0;
            int alarmCount = 0;
            int monitorCount = 0;
            while (true)
            {
                Console.WriteLine("Enter a command:");
                String command = Console.ReadLine();
                string inText;

                switch (command.ToUpper())
                {
                    case "ADD":
                        Console.WriteLine("Add sensor, monitor or alarm? [s|m|a]:");
                        inText = Console.ReadLine().ToUpper();
                        if (inText.Equals("S"))
                        {
                            Mes.SensorMessage msg = new Mes.SensorMessage();

                            MessageQueue queue = new MessageQueue(queueName);
                            queue.Send(msg);
                        }
                        else if (inText.Equals("M"))
                        {
                        }
                        else if (inText.Equals("A"))
                        {
                        }
                        else
                        {
                            Console.WriteLine("Invalid type of node");
                        }
                        break;
                    case "EDIT":
                        Console.WriteLine("Edit Sensor:");
                        break;
                    case "TRIGGER":
                        Console.WriteLine("Trigger sensor #: ");
                        inText = Console.ReadLine();
                        // set message data
                        sensorMessage.sensorId = Convert.ToInt32(inText);
                        sensorMessage.messageType = (int)Actions.Trig;

                        // construct message
                        Message message = new Message();
                        message.Body = sensorMessage;

                        // send the message to the security system
                        if (MessageQueue.Exists(queueName))
                        {
                            MessageQueue queue = new MessageQueue(queueName);
                            queue.Send(message);
                        }
                        else
                        {
                            Console.WriteLine("Terminal - Queue .\\security not Found");
                        }
                        break;
                    case "REMOVE":
                        Console.WriteLine("Remove Sensor:");
                        break;
                    case "EXIT":
                        return;
                    case "HELP":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("add\nedit\ntrigger\nremove\nexit\nhelp");
                        break;
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        break;
                }
            }
        }
    }
}
