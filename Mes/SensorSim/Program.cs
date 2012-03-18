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
        static void Main(string[] args)
        {
            Console.WriteLine("Sensor Simulation for Mesmerize One Prime X2");
            
            Mes.SensorMessage sensorMessage = new Mes.SensorMessage();
            // Needed for sending messages to the security system to handle
            string queueName = @".\Private$\security";

            while (true)
            {
                String command = Console.ReadLine();

                switch (command.ToUpper())
                {
                    case "ADD":
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
                    case "EDIT":
                    case "CONFIG":
                        Console.WriteLine("Edit Sensor:");
                        break;
                    case "TRIGGER":
                        Console.WriteLine("Trigger sensor #: ");
                        string value = Console.ReadLine();
                        // set message data
                        sensorMessage.sensorId = Convert.ToInt32(value);
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
                    default:
                        Console.WriteLine("Error: Invalid command " + command + " was entered!");
                        break;
                }
            }
        }
    }
}
