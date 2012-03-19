using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
namespace SensorSim
{
    class Program
    {
        // Enumerator for different sensor actions
        enum Actions {Add, Edit, Trig};

        // Needed for sending messages to the security system to handle
        static string queueName = @".\Private$\security";

        // Send a message to the central system with component configuration.
        // Retrieve an Id to save into the sensor
        static public void SimulatorAddSensor(List<Mes.Sensor> inList)
        {
            Mes.SensorMessage sensorMessage = new Mes.SensorMessage();
            sensorMessage.messageType = (int)Actions.Add;
            if (MessageQueue.Exists(queueName))
            {
                MessageQueue queue = new MessageQueue(queueName);
                queue.Send(sensorMessage);
            }
            else
            {
                Console.WriteLine("Terminal - Queue .\\security not Found");
            }
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
            List<Mes.Sensor> realSensors = new List<Mes.Sensor>();
            List<Mes.Monitor> realMonitors = new List<Mes.Monitor>();
            List<Mes.Alarm> realAlarms = new List<Mes.Alarm>();
            Mes.SensorMessage sensorMessage = new Mes.SensorMessage();

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
                            SimulatorAddSensor(realSensors);
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
                        break;
                    case "CONFIG":
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
