using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;



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
        private List<Thread> simulationThreads = new List<Thread>();

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
                                deviceType = tmpParams.ElementAt(2).ToUpper();
                                enableParam = tmpParams.ElementAt(3).ToUpper();
                                if (enableParam == "TRUE") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5).ToUpper();
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
                                        securityLogger.appendLog("Added Sensor: " + sensor.Id.ToString() + " of Type: " + sensor.Type);
                                        Console.WriteLine("Added Sensor to System.");
                                        break;
                                    case "ALARM":
                                        Alarm alarm = null;
                                        switch (deviceCategory)
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
                                            default:
                                                break;
                                        }
                                        alarm.Location = location;
                                        alarm.IsEnabled = isEnable;
                                        alarm.ParentId = id;
                                        alarm.Sensitivity = threshold;
                                        alarm.Id = mesDB.AddAlarm(alarm);
                                        alarms.Add(alarm);
                                        securityLogger.appendLog("Added Alarm: " + alarm.Id.ToString() + " of Type: " + alarm.Type);
                                        Console.WriteLine("Added Alarm to System.");
                                        break;
                                    case "MONITOR":
                                        Monitor monitor = null;
                                        switch (deviceCategory)
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
                                        //securityLogger.appendLog("Added Monitor: " + monitor.Id.ToString() + " of Type: " + monitor.Type);
                                        break;
                                    default:
                                        Console.WriteLine("Failed to Register Device!");
                                        //securityLogger.appendLog("Failed to Register Device!);
                                        break;
                                }
                                break;
                            case ("VIEW"):
                                tmpParams = GetParams(mesMessage);
                                if (tmpParams.ElementAt(0).ToString() != "") {
                                    deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                } else {
                                    deviceId = -1;
                                }
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                deviceType = tmpParams.ElementAt(2).ToUpper().ToUpper();
                                enableParam = tmpParams.ElementAt(3).ToUpper();
                                if (enableParam == "TRUE") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5).ToUpper();
                                List<Sensor> sensorOutput = new List<Sensor>();
                                List<Alarm> alarmOutput = new List<Alarm>();
                                switch (deviceType) {
                                    case ("SENSOR"):
                                        if (deviceId > 0)
                                        {
                                            sensorOutput = mesDB.GetSensors(deviceId, -1);
                                            securityLogger.appendLog("User has requested to view target sensor.");
                                        }
                                        else
                                        {
                                            sensorOutput = mesDB.GetSensors(-1,-1);
                                            securityLogger.appendLog("User has requested to view all sensors.");
                                        }
                                        Console.WriteLine("======================================================================");
                                        Console.WriteLine("======================================================================");
                                        Console.WriteLine("=====================     Showing Sensors     ========================");
                                        Console.WriteLine("======================================================================");
                                        foreach (Sensor tmpSensor in sensorOutput)
                                        {
                                            Console.WriteLine("======================================================================");
                                            Console.WriteLine("Sensor Type: " + tmpSensor.Type + "     Sensor ID: " + tmpSensor.Id);
                                            Console.WriteLine("----------------------------------------------------------------------");
                                            Console.WriteLine("Sensor Armed: " + tmpSensor.IsEnabled.ToString());
                                            Console.WriteLine("Sensor Triggered: " + tmpSensor.IsTriggered.ToString());
                                            Console.WriteLine("Sensor Location: " + tmpSensor.Location);
                                            Console.WriteLine("Sensor Threshold: " + tmpSensor.Threshold);
                                        }
                                        Console.WriteLine("======================================================================");
                                        break;
                                    case ("ALARM"):
                                        if (deviceId > 0)
                                        {
                                            alarmOutput = mesDB.GetAlarms(deviceId, -1);
                                            securityLogger.appendLog("User has requested to view target alarm.");
                                        }
                                        else
                                        {
                                            alarmOutput = mesDB.GetAlarms(-1,-1);
                                            securityLogger.appendLog("User has requested to view all alarms.");
                                        }
                                        Console.WriteLine("======================================================================");
                                        Console.WriteLine("======================================================================");
                                        Console.WriteLine("=====================     Showing Alarms     ========================");
                                        Console.WriteLine("======================================================================");
                                        foreach (Alarm tmpAlarm in alarmOutput)
                                        {
                                            Console.WriteLine("======================================================================");
                                            Console.WriteLine("Alarm Type: " + tmpAlarm.Type + "     Alarm ID: " + tmpAlarm.Id);
                                            Console.WriteLine("----------------------------------------------------------------------");
                                            Console.WriteLine("Alarm Armed: " + tmpAlarm.IsEnabled.ToString());
                                            Console.WriteLine("Alarm Triggered: " + tmpAlarm.IsTriggered.ToString());
                                            Console.WriteLine("Alarm Location: " + tmpAlarm.Location);
                                            Console.WriteLine("Alarm Sensitivity: " + tmpAlarm.Sensitivity);
                                        }
                                        Console.WriteLine("======================================================================");
                                        break;
                                    case ("MONITOR"):

                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case ("EDIT"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                deviceType = tmpParams.ElementAt(2).ToUpper();
                                enableParam = tmpParams.ElementAt(3).ToUpper();
                                if (enableParam == "true") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5).ToUpper();

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
                                                securityLogger.appendLog("Successfully edited sensor: " + sensors.ElementAt(index).Id.ToString());
                                                Console.WriteLine("Finished edit sensor.");
                                            }
                                            else
                                            {
                                                securityLogger.appendLog("Failed to edited sensor: " + sensors.ElementAt(index).Id.ToString());
                                                Console.WriteLine("Failed editing sensor on system.");
                                            }
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add sensor!");
                                            Console.WriteLine("Failed editing sensor on system.");
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
                                            if (mesDB.EditAlarm(alarms.ElementAt(index)))
                                            {
                                                securityLogger.appendLog("Successfully edited alarm: " + alarms.ElementAt(index).Id.ToString());
                                                Console.WriteLine("Finished editing alarm on system.");
                                            }
                                            else
                                            {
                                                securityLogger.appendLog("Failed to edited alarm: " + alarms.ElementAt(index).Id.ToString());
                                                Console.WriteLine("Failed editing alarm on system.");
                                            }
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to add alarm");
                                            Console.WriteLine("Failed editing alarm on system.");
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
                                deviceType = tmpParams.ElementAt(2).ToUpper();
                                enableParam = tmpParams.ElementAt(3).ToUpper();
                                if (enableParam == "true") {
                                    isEnable = true;
                                } else {
                                    isEnable = false;
                                }
                                threshold = Convert.ToInt16(tmpParams.ElementAt(4));
                                location = tmpParams.ElementAt(5).ToUpper();

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
                                            int tmpId = sensors.ElementAt(index).Id;
                                            mesDB.RemoveSensor(sensors.ElementAt(index).Id);
                                            sensors.RemoveAt(index);
                                            securityLogger.appendLog("Successfully removed sensor: " + tmpId.ToString());
                                            Console.WriteLine("Remove sensor successful.");
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to remove sensor");
                                            Console.WriteLine("Failed to remove sensor.");
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

                                            int tmpId = alarms.ElementAt(index).Id;
                                            mesDB.RemoveAlarm(alarms.ElementAt(index).Id);
                                            alarms.RemoveAt(index);
                                            securityLogger.appendLog("Successfully removed alarm: " + tmpId.ToString());
                                            Console.WriteLine("Remove alarm successful.");
                                        }
                                        else
                                        {
                                            securityLogger.appendLog("Failed to remove alarm");
                                            Console.WriteLine("Failed to remove alarm.");
                                        }
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
                                location = tmpParams.ElementAt(1).ToUpper();
                                deviceCategory = "ALARM";
                                switch (deviceCategory) {
                                    case "ALARM":
                                        foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if (deviceId == -1 && tmpAlarm.Location.Equals(location)){
                                                tmpAlarm.Trigger();     // Trigger all alarms in the location of a tripped sensor
                                                index = 1;
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
                                    securityLogger.appendLog(string.Format("Failed to trigger {0}", deviceCategory));
                                }
                                break;
                            case ("READING"):
                                tmpParams = GetParams(mesMessage);
                                if(tmpParams.Count == 2)
                                {
                                    int id = Convert.ToInt16(tmpParams.ElementAt(0));
                                    for (int z = 0; z < sensors.Count; z++) 
                                    {
                                        if (sensors[z].Id == id)
                                        {
                                            int reading = Convert.ToInt32(tmpParams.ElementAt(1));
                                            sensors[z].ThresholdCheck(reading);
                                        }
                                    }
  
                                }
                                //Thread thresholdCheckThread = new Thread(new ThreadStart(simSensor.ThresholdCheck(reading)));
                                //thresholdCheckThread.Start(reading);
                                break;
                            case ("ENABLESIM"):
                                for (int z = 0; z < Sensors.Count; z++)
                                {
                                    SimulationSensor sim = sensors.ElementAt(z).SimulationSensor;
                                    sim.On = true;
                                    Thread simulationThread = new Thread(new ThreadStart(sim.SimulationRun));
                                    //simulationThreads.Add(simulationThread);
                                    simulationThread.Start();
                                }
                                break;
                            case ("DISABLESIM"):
                                for (int z = 0; z < Sensors.Count; z++)
                                {
                                    Sensors[z].SimulationSensor.On = false;
                                    //simulationThreads.ElementAt(z).Abort();
                                    //simulationThreads.RemoveAt(z);
                                }
                                break;
                            case ("CHANGEREADING"):
                                tmpParams = GetParams(mesMessage);
                                int simId = Convert.ToInt16(tmpParams.ElementAt(0));
                                    for (int z = 0; z < sensors.Count; z++) 
                                    {
                                        if (sensors[z].Id == simId)
                                        {
                                            int reading = Convert.ToInt32(tmpParams.ElementAt(1));
                                            sensors[z].SimulationSensor.Reading = reading;
                                        }
                                    }
                                break;
                            default:    // Not any of the actions add, edit, view, trigger, etc. Error.
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

        public List<Sensor> Sensors
        {
            get
            {
                return sensors;
            }
            set
            {
                sensors = value;
            }
        }

        public List<Alarm> Alarms
        {
            get
            {
                return alarms;
            }
            set
            {
                alarms = value;
            }
        }
    }
}
