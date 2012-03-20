﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;


namespace Mes
{
    // Global to refer to the queue name of the Security system to send it messages
    public static class GlobalVariables{
        public static string queueName = @".\Private$\security";
    }
    // object to fill a message with sensor information
    public class MesMessage {
        public string type;             // Add, edit, remove, or view
        public string message;      // written in the form "id,deviceType(magnetic),category(sensor),enable,threshold,location"
    };

    class SecuritySystem : GenericSystem
    {
        MessageQueue queue = null;
        string queueName = @".\Private$\security";
        Logger securityLogger = new Logger();
        List<Sensor> sensors = new List<Sensor>();
        List<Alarm> alarms = new List<Alarm>();
        List<Monitor> monitors = new List<Monitor>();

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
                    queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(MesMessage) });
                    try
                    {
                        Message msg = queue.Receive();
                        MesMessage mesMessage = (MesMessage)msg.Body;

                        int deviceId;
                        string deviceCategory;
                        string deviceType;
                        bool isEnable;
                        int threshold;
                        string location;
                        string enableParam;
                        List<string> tmpParams = new List<string>();
                        int index = -1;
                        int i = 0;

                        switch (mesMessage.type)
                        {
                            case ("ADD"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                deviceType = tmpParams.ElementAt(2);
                                enableParam = tmpParams.ElementAt(3);
                                if (enableParam == "true") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5);

                                switch (deviceCategory) {
                                    case "SENSOR":
                                        switch (deviceType)
                                        {
                                            case "MAGNETIC":
                                                MagneticSensor tmpMagSensor = new MagneticSensor();
                                                tmpMagSensor.Id = deviceId;
                                                tmpMagSensor.Type = "MAGNETIC";
                                                tmpMagSensor.Location = location;
                                                tmpMagSensor.IsEnabled = isEnable;
                                                tmpMagSensor.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                break;
                                            case "MOTION":
                                                MotionSensor tmpMotSensor = new MotionSensor();
                                                tmpMotSensor.Id = deviceId;
                                                tmpMotSensor.Type = "MOTION";
                                                tmpMotSensor.Location = location;
                                                tmpMotSensor.IsEnabled = isEnable;
                                                tmpMotSensor.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                break;
                                            case "FLOOD":
                                                FloodSensor tmpFloSensor = new FloodSensor();
                                                tmpFloSensor.Id = deviceId;
                                                tmpFloSensor.Type = "FLOOD";
                                                tmpFloSensor.Location = location;
                                                tmpFloSensor.IsEnabled = isEnable;
                                                tmpFloSensor.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                break;
                                            case "SMOKE":
                                                SmokeSensor tmpSmoSensor = new SmokeSensor();
                                                tmpSmoSensor.Id = deviceId;
                                                tmpSmoSensor.Type = "SMOKE";
                                                tmpSmoSensor.Location = location;
                                                tmpSmoSensor.IsEnabled = isEnable;
                                                tmpSmoSensor.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "ALARM":
                                        switch (deviceType)
                                        {
                                            case "LIGHT":
                                                /*
                                                LightAlarm tmpLigAlarm = new LightAlarm();
                                                tmpLigAlarm.Id = deviceId;
                                                tmpLigAlarm.Type = "SMOKE";
                                                tmpLigAlarm.Location = location;
                                                tmpLigAlarm.IsEnabled = isEnable;
                                                tmpLigAlarm.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                 * */
                                                break;
                                            case "SIREN":
                                                /*
                                                SirenAlarm tmpSirAlarm = new SirenAlarm();
                                                tmpSirAlarm.Id = deviceId;
                                                tmpSirAlarm.Type = "SMOKE";
                                                tmpSirAlarm.Location = location;
                                                tmpSirAlarm.IsEnabled = isEnable;
                                                tmpSirAlarm.ParentId = id;
                                                //threshold definition
                                                //Add to DB
                                                //Store into List<device>
                                                 * */
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "MONITOR":
                                        switch (deviceType)
                                        {
                                            case "MOTION":
                                                /*
                                                VideoCamera tmpCamera = new VideoCamera();
                                                tmpCamera.Id = deviceId;
                                                tmpCamera.IsEnabled = isEnable;
                                                tmpCamera.Type = "VIDEO";
                                                tmpCamera.ParentId = id;
                                                tmpCamera.Location = location;
                                                //threshold
                                                //Add to DB
                                                //Store into List<device>
                                                 * */
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case ("VIEW"):

                                break;
                            case ("EDIT"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                deviceType = tmpParams.ElementAt(2);
                                enableParam = tmpParams.ElementAt(3);
                                if (enableParam == "true") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5);

                                switch (deviceCategory) {
                                    case "SENSOR":
                                        foreach (Sensor tmpSensor in sensors)
                                        {
                                            if (tmpSensor.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            sensors.ElementAt(index).Location = location;
                                            //sensors.ElementAt(index).isEnabled = isEnable;
                                            //sensors.ElementAt(index).Threshold = threshold;
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add sensor");
                                        }
                                        //Send sensors.ElementAt(Index) object to DB Manager
                                        break;
                                    case "ALARM":
                                        /*foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if (tmpAlarm.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            //alarms.ElementAt(index).Location = location;
                                            //alarms.ElementAt(index).isEnabled = isEnable;
                                            //alarms.ElementAt(index).Threshold = threshold;
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add alarm");
                                        }
                                        //Send alarms.ElementAt(Index) object to DB Manager
                                         * */
                                        break;
                                    case "MONITOR":
                                        /*foreach (Monitor tmpMonitor in monitors)
                                        {
                                            if (tmpMonitor.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            monitors.ElementAt(index).Location = location;
                                            //monitors.ElementAt(index).isEnabled = isEnable;
                                            //monitors.ElementAt(index).Threshold = threshold;
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add monitor");
                                        }
                                        //Send monitors.ElementAt(Index) object to DB Manager
                                         * */
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case ("REMOVE"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                deviceType = tmpParams.ElementAt(2);
                                enableParam = tmpParams.ElementAt(3);
                                if (enableParam == "true") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5);

                                switch (deviceCategory) {
                                    case "SENSOR":
                                        foreach (Sensor tmpSensor in sensors)
                                        {
                                            if (tmpSensor.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            //Remove from DB send sensors.ElementAt(index).id;
                                            sensors.RemoveAt(index);
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to remove sensor");
                                        }
                                        break;
                                    case "ALARM":
                                        /*foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if (tmpAlarm.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            //Remove from DB send alarms.ElementAt(index).id;
                                            alarms.RemoveAt(index);
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to remove alarm");
                                        }
                                         * */
                                        break;
                                    case "MONITOR":
                                        /*foreach (Monitor tmpMonitor in monitors)
                                        {
                                            if (tmpMonitor.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            //Remove from DB send monitors.ElementAt(index).id;
                                            monitors.RemoveAt(index);
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to remove monitor");
                                        }
                                         * */
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case ("LOG"):

                                break;
                            case ("TRIGGER"):

                                break;
                            default:
                                break;
                        }
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

        public List<string> GetParams(MesMessage msg)
        {
            List<string> returnParam = new List<string>();

            string[] tmpArray = msg.message.Split(',');

            foreach (string param in tmpArray)
            {
                returnParam.Add(param);
            }
            return returnParam;
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
