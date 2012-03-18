using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;


namespace Mes
{
    public class SensorMessage {
        public int sensorId;
        public int messageType;         // The type may indicate what parameter is changing
        public string messageString;    // This is the parameter value (for string parameters)
        public int messageValue;        // This is the parameter value (for integer parameters)
    }
    
    class SecuritySystem
    {
        MessageQueue queue = null;
        Message message = null;
        SensorMessage sensorMessage = null;
        string queueName = @".\Private$\security";
        public SecuritySystem()
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
                    MessageQueue queue = new MessageQueue(queueName);
                    queue.Formatter = new XmlMessageFormatter(new string[]{"System.String"});
                    Message Mymessage = queue.Receive();
                    Console.WriteLine(Mymessage.Body);            
                }
                else
                {
                    Console.WriteLine("Queue .\\security not Found");
                }
            }
        }
    }
}
