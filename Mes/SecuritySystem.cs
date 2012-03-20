using System;
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
        DB_Manager mesDB = new DB_Manager();

        private int id;

        public SecuritySystem()
        {
            createMessageQueue();
        }

        public SecuritySystem(int identification)
        {
            createMessageQueue();
            id = identification;
            queue.Purge();
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

            mesDB.SetConnection();

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
                        Console.WriteLine("Message Received!");

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
                                if (enableParam == "TRUE") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5);
                                Console.WriteLine("Decoded Add Message!");

                                switch (deviceType) {
                                    case "SENSOR":
                                        Sensor sensor = null;
                                        switch (deviceCategory)
                                        {
                                            case "MAGNETIC":
                                                sensor = new MagneticSensor();
                                                sensor.Type = "MAGNETIC";
                                                break;
                                            case "MOTION":
                                                sensor = new MotionSensor();
                                                sensor.Type = "MOTION";
                                                Console.WriteLine("Adding Motion Sensor!");
                                                break;
                                            case "FLOOD":
                                                sensor = new FloodSensor();
                                                sensor.Type = "FLOOD";
                                                break;
                                            case "SMOKE":
                                                sensor = new SmokeSensor();
                                                sensor.Type = "SMOKE";
                                                break;
                                            default:
                                                break;
                                        }
                                        sensor.Location = location;
                                        sensor.IsEnabled = isEnable;
                                        sensor.ParentId = 1;
                                        sensor.Threshold = threshold;
                                        sensor.Id = mesDB.AddSensor(sensor);
                                        sensors.Add(sensor);
                                        Console.WriteLine("Added Sensor successfully!");
                                        break;
                                    case "ALARM":
                                        Alarm alarm = null;
                                        switch (deviceType)
                                        {
                                            case "LIGHT":
                                                alarm = new LightAlarm();
                                                alarm.Type = "LIGHT";
                                                alarm.Id = deviceId;
                                                break;
                                            case "SIREN":
                                                alarm = new SirenAlarm();
                                                alarm.Type = "SIREN";
                                                break;
                                        }
                                        alarm.Location = location;
                                        alarm.IsEnabled = isEnable;
                                        alarm.ParentId = id;
                                        alarm.Sensitivity = threshold;
                                        //Add to DB
                                        //Store into List<device>
                                        break;
                                    case "MONITOR":
                                        Monitor monitor = null;
                                        switch (deviceType)
                                        {
                                            case "MOTION":
                                                monitor = new VideoCamera();
                                                monitor.Type = "VIDEO";
                                                break;
                                        }
                                        monitor.Id = deviceId;
                                        monitor.IsEnabled = isEnable;
                                        monitor.ParentId = id;
                                        monitor.Location = location;
                                        //threshold
                                        //Add to DB
                                        //Store into List<device>
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

                                switch (deviceType) {
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
                                            sensors.ElementAt(index).IsEnabled = isEnable;
                                            sensors.ElementAt(index).Threshold = threshold;
                                            if (mesDB.EditSensor(sensors.ElementAt(index)))
                                            {
                                                securityLogger.appendLog("Successfully added sensor.");
                                            }
                                            else
                                            {
                                                securityLogger.appendLog("Failed to add sensor!");
                                            }
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add sensor!");
                                        }
                                        break;
                                    case "ALARM":
                                        foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if (tmpAlarm.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            alarms.ElementAt(index).Location = location;
                                            alarms.ElementAt(index).IsEnabled = isEnable;
                                            alarms.ElementAt(index).Sensitivity = threshold;
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add alarm");
                                        }
                                        //Send alarms.ElementAt(Index) object to DB Manager
                                        break;
                                    case "MONITOR":
                                        foreach (Monitor tmpMonitor in monitors)
                                        {
                                            if (tmpMonitor.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            monitors.ElementAt(index).Location = location;
                                            monitors.ElementAt(index).IsEnabled = isEnable;
                                            //monitors.ElementAt(index).Threshold = threshold;
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add monitor");
                                        }
                                        //Send monitors.ElementAt(Index) object to DB Manager
                                        break;
                                    default:
                                        break;
                                }
                                break;  // EDIT
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

                                switch (deviceType) {
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
                                            mesDB.RemoveSensor(sensors.ElementAt(index).Id);
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
                                securityLogger.appendLog(mesMessage.message);
                                break;
                            case ("TRIGGER"):
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
                                                tmpSensor.Trigger();
                                                index=i;
                                                break;
                                            }
                                            i++;
                                        }
                                        break;
                                    case "ALARM":
                                        foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if (deviceId.Equals(null) && tmpAlarm.Location == location){
                                                tmpAlarm.Trigger();     // Trigger all alarms in the location of a tripped sensor
                                            }
                                            else if (tmpAlarm.Id == deviceId) {
                                                tmpAlarm.Trigger();
                                                index = i;
                                                break;
                                            }
                                            i++;
                                        }
                                        break;
                                    case "MONITOR":
                                        foreach (Monitor tmpMonitor in monitors)
                                        {
                                            if (tmpMonitor.Id == deviceId) {
                                                tmpMonitor.Trigger();
                                                index = i;
                                                break;
                                            }
                                            i++;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                if (index >= 0)
                                {
                                    // do nothing, this is an error check. If the node wasn't found
                                    // then index was never set, thus the deviceId is not valid or null
                                }
                                else
                                {
                                    securityLogger.appendLog(string.Format("Failed to add {0}", deviceCategory));
                                }
                                break;
                            default:    // Not a any of the actions add, edit, view, trigger, etc. Error.
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
