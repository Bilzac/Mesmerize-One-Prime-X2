﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;


namespace Mes
{
    // object to fill a message with sensor information
    public class SensorMessage {
        public int sensorId;
        public int messageType;         // The type may indicate what parameter is changing
        public string messageString;    // This is the parameter value (for string parameters)
        public int messageValue;        // This is the parameter value (for integer parameters)
    };
    
    class SecuritySystem : GenericSystem
    {
        MessageQueue queue = null;
        Message message = null;
        SensorMessage sensorMessage = null;
        string queueName = @".\Private$\security";
        Logger msgLogger = new Logger();

        private int id;

        public SecuritySystem()
        {
            createMessageQueue();
        }

        public SecuritySystem(int identification)
        {
            createMessageQueue();
            id = identification;
        }

        public void createMessageQueue()
        {
            if (MessageQueue.Exists(queueName))
                queue = new MessageQueue(queueName);
            else
            {
                queue = MessageQueue.Create(queueName);
                queue.Label = "First Queue";
                Console.WriteLine("Queue Created:");
                Console.WriteLine("Path: {0}, queue.Path");
                Console.WriteLine("FormatName: {0}, queue.FormatName");
            }
        }

        public void Run()
        {
            while(true)
            {
                if (MessageQueue.Exists(queueName))
                {
                    queue = new MessageQueue(queueName);
                    queue.Formatter = new XmlMessageFormatter(new string[] { "System.String" });
                    //queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(SensorMessage) });
                    try
                    {
                        // Receive and format the message.
                        //SensorMessage sensorMessage = (SensorMessage)queue.Receive().Body;
                        // msgLogger.appendLog(msgLogger.formatSensorMessage(sensorMessage));
                        //Console.WriteLine("Received: {0} {1}", sensorMessage.sensorId, sensorMessage.messageType);
                        Message message = queue.Receive();
                        string testMessage = (string)message.Body;
                        msgLogger.appendLog(testMessage);
                    }

                    catch (MessageQueueException)
                    {
                        // Handle Message Queuing exceptions.
                    }

                    // Handle invalid serialization format.
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Queue .\\security not Found");
                }
                System.Threading.Thread.Sleep(0);
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
		  
        }
    }
}
