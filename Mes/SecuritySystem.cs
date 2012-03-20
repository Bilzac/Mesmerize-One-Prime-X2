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
        //Messsage Queue, Logging, and Database Connections
        //Store of Virtual Device Objects
        MessageQueue queue = null;
        string queueName = @".\Private$\security";
        Logger securityLogger = new Logger();
        List<Sensor> sensors = new List<Sensor>();
        List<Alarm> alarms = new List<Alarm>();
        List<Monitor> monitors = new List<Monitor>();
        DB_Manager mesDB = new DB_Manager();

        //System Identification and Threads
        private int id;
        private List<Thread> simulationThreads = new List<Thread>();

        //Initialize the Security System
        public SecuritySystem()
        {
            createMessageQueue();
        }

        //Generate the correct Security System if it has a valid Id.
        public SecuritySystem(int identification)
        {
            createMessageQueue();
            id = identification;
            queue.Purge();
        }

        //Generation of the Messsage Queue to be used to receive and decode messages
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

        //Run Instance of the Security System.  This is to receive messages periodically and process them.
        public void Run()
        {

            mesDB.SetConnection();

            while(true)
            {
                //Check for non-empty Message Queue
                if (MessageQueue.Exists(queueName))
                {
                    //Retrieve Message
                    queue = new MessageQueue(queueName);
                    queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(MesMessage) });
                    try
                    {
                        //Formats the Message to the target type
                        Message msg = queue.Receive();
                        MesMessage mesMessage = (MesMessage)msg.Body;

                        //Variables for storing message parameters
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

                        //Message Decoding Condition.
                        switch (mesMessage.type)
                        {
                            //-------------------------------------------------------------------------------------------------------
                            //Add Device Switch Statement
                            //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Add Sensor Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
                                    case "SENSOR":
                                        Sensor sensor = null;
                                        //-------------------------------------------------------------------------------------------------------
                                        //Set Sensor Type
                                        //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Add Alarm Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
                                    case "ALARM":
                                        Alarm alarm = null;
                                        //-------------------------------------------------------------------------------------------------------
                                        //Set Alarm Type
                                        //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Add Monitor Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
                                    case "MONITOR":
                                        Monitor monitor = null;
                                        //-------------------------------------------------------------------------------------------------------
                                        //Set Monitor Type
                                        //-------------------------------------------------------------------------------------------------------
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
                                //-------------------------------------------------------------------------------------------------------
                                //View Device(s) Switch Statement
                                //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //View Sensor Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
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
                                            Console.WriteLine("Sensor Location: " + tmpSensor.Location);
                                            Console.WriteLine("Sensor Threshold: " + tmpSensor.Threshold);
                                        }
                                        Console.WriteLine("======================================================================");
                                        break;
                                    //-------------------------------------------------------------------------------------------------------
                                    //View Alarm(s) Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
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
                                            Console.WriteLine("Alarm Location: " + tmpAlarm.Location);
                                            Console.WriteLine("Alarm Sensitivity: " + tmpAlarm.Sensitivity);
                                        }
                                        Console.WriteLine("======================================================================");
                                        break;
                                    //-------------------------------------------------------------------------------------------------------
                                    //View Monitor(s) Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
                                    case ("MONITOR"):

                                        break;
                                    default:
                                        break;
                                }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Edit Device Switch Statement
                            //-------------------------------------------------------------------------------------------------------
                            case ("EDIT"):
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Edit Sensor Device
                                    //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Edit Alarm Device
                                    //-------------------------------------------------------------------------------------------------------
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
                                        break;
                                    //-------------------------------------------------------------------------------------------------------
                                    //Edit Monitor Device 
                                    //-------------------------------------------------------------------------------------------------------
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
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Remove Device Switch Statement
                            //-------------------------------------------------------------------------------------------------------
                            case ("REMOVE"):
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Remove Sensor Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Remove Alarm Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Remove Monitor Device Switch Statement
                                    //-------------------------------------------------------------------------------------------------------
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
                            //-------------------------------------------------------------------------------------------------------
                            //Log Message
                            //-------------------------------------------------------------------------------------------------------
                            case ("LOG"):
                                securityLogger.appendLog(mesMessage.message);
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Arm Security System
                            //-------------------------------------------------------------------------------------------------------
                            case ("ARM"):
                                securityLogger.appendLog("Security System has been armed.");
                                foreach (Sensor tmpSensor in sensors) {
                                    tmpSensor.Enable();
                                    if (mesDB.EditSensor(tmpSensor))
                                    {
                                        //securityLogger.appendLog("Sensor: " + tmpSensor.Id + " has been armed.");
                                    }
                                }
                                foreach (Alarm tmpAlarm in alarms)
                                {
                                    tmpAlarm.Enable();
                                    if (mesDB.EditAlarm(tmpAlarm))
                                    {
                                        //securityLogger.appendLog("Alarm: " + tmpAlarm.Id + " has been armed.");
                                    }
                                }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Disarm Security System
                            //-------------------------------------------------------------------------------------------------------
                            case ("DISARM"):
                                securityLogger.appendLog("Security System has been disarmed.");
                                foreach (Sensor tmpSensor in sensors) {
                                    tmpSensor.Disable();
                                    tmpSensor.Untrigger();
                                    tmpSensor.SimulationSensor.Reading = -1;
                                    if (mesDB.EditSensor(tmpSensor))
                                    {
                                        //securityLogger.appendLog("Sensor: " + tmpSensor.Id + " has been disarmed.");
                                    }
                                }
                                foreach (Alarm tmpAlarm in alarms)
                                {
                                    tmpAlarm.Disable();
                                    tmpAlarm.Untrigger();
                                    if (mesDB.EditAlarm(tmpAlarm))
                                    {
                                        //securityLogger.appendLog("Alarm: " + tmpAlarm.Id + " has been disarmed.");
                                    }
                                }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Trigger Device Statement
                            //-------------------------------------------------------------------------------------------------------
                            case ("TRIGGER"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                location = tmpParams.ElementAt(1).ToUpper();
                                deviceCategory = "ALARM";
                                switch (deviceCategory) {
                                    //-------------------------------------------------------------------------------------------------------
                                    //Trigger Alarm statement
                                    //-------------------------------------------------------------------------------------------------------
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
                                    //-------------------------------------------------------------------------------------------------------
                                    //Trigger Monitor statement
                                    //-------------------------------------------------------------------------------------------------------
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
                            //-------------------------------------------------------------------------------------------------------
                            //Untrigger device statement
                            //-------------------------------------------------------------------------------------------------------
                            case ("UNTRIGGER"):
                                tmpParams = GetParams(mesMessage);
                                deviceId = Convert.ToInt16(tmpParams.ElementAt(0));
                                deviceCategory = tmpParams.ElementAt(1).ToUpper();
                                switch (deviceCategory) {
                                    //-------------------------------------------------------------------------------------------------------
                                    //Untrigger Alarm
                                    //-------------------------------------------------------------------------------------------------------
                                    case ("ALARM"):
                                        foreach (Alarm tmpAlarm in alarms)
                                        {
                                            if(tmpAlarm.Id == deviceId) {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0) {
                                            alarms.ElementAt(index).Untrigger();
                                        }
                                        break;
                                    //-------------------------------------------------------------------------------------------------------
                                    //Untrigger Sensor
                                    //-------------------------------------------------------------------------------------------------------
                                    case ("SENSOR"):
                                        foreach (Sensor tmpSensor in sensors)
                                        {
                                            if (tmpSensor.Id == deviceId)
                                            {
                                                index = i;
                                            }
                                            i++;
                                        }
                                        if (index >= 0)
                                        {
                                            sensors.ElementAt(index).Untrigger();
                                            sensors.ElementAt(index).SimulationSensor.Reading = -1;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Check the Reading of a Device
                            //-------------------------------------------------------------------------------------------------------
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
                            //-------------------------------------------------------------------------------------------------------
                            //Enable Simulation Environment
                            //-------------------------------------------------------------------------------------------------------
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
                            //-------------------------------------------------------------------------------------------------------
                            //Disable Simulation Environment
                            //-------------------------------------------------------------------------------------------------------
                            case ("DISABLESIM"):
                                for (int z = 0; z < Sensors.Count; z++)
                                {
                                    Sensors[z].SimulationSensor.On = false;
                                    //simulationThreads.ElementAt(z).Abort();
                                    //simulationThreads.RemoveAt(z);
                                }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Configure Reading Setting on Sensor
                            //-------------------------------------------------------------------------------------------------------
                            case ("CHANGEREADING"):
                                tmpParams = GetParams(mesMessage);
                                int simId = Convert.ToInt16(tmpParams.ElementAt(0));
                                    for (int z = 0; z < sensors.Count; z++) 
                                    {
                                        if (sensors[z].Id == simId)
                                        {
                                            int reading = Convert.ToInt32(tmpParams.ElementAt(1));
                                            sensors[z].SimulationSensor.Reading = reading;
                                            Console.WriteLine("Sensor: " + sensors[z].Id + " reading has changed");
                                            securityLogger.appendLog(string.Format("Sensor: {0} of Type: {1} at {2} reading value has changed to {3}",sensors[z].Id,sensors[z].Type,sensors[z].Location,reading.ToString()));
                                        }
                                    }
                                break;
                            //-------------------------------------------------------------------------------------------------------
                            //Displays the status of all alarms and sensors
                            //-------------------------------------------------------------------------------------------------------
                            case ("STATUS"):
                                Console.WriteLine("======================================================================");
                                Console.WriteLine("=====================      Sensor Status      ========================");
                                Console.WriteLine("======================================================================");
                                foreach (Sensor tmpSensor in sensors)
                                {
                                    Console.WriteLine("Sensor ID: " + tmpSensor.Id + " Sensor Type: " + tmpSensor.Type);
                                    Console.WriteLine("Location: " + tmpSensor.Location);
                                    Console.WriteLine("Triggered: " + tmpSensor.IsTriggered.ToString());
                                    Console.WriteLine("Reading: " + tmpSensor.SimulationSensor.Reading.ToString());
                                    Console.WriteLine("----------------------------------------------------------------------");
                                }
                                Console.WriteLine("======================================================================");
                                Console.WriteLine("=====================      Alarm Status       ========================");
                                Console.WriteLine("======================================================================");
                                foreach (Alarm tmpAlarm in alarms)
                                {
                                    Console.WriteLine("Alarm ID: " + tmpAlarm.Id + " Alarm Type: " + tmpAlarm.Type);
                                    Console.WriteLine("Location: " + tmpAlarm.Location);
                                    Console.WriteLine("Triggered: " + tmpAlarm.IsTriggered.ToString());
                                    Console.WriteLine("----------------------------------------------------------------------");
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

        //-------------------------------------------------------------------------------------------------------
        //Converts the message string into a list of string.
        //-------------------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------------------
        //Accessor of Id for SecuritySystem
        //-------------------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------------------
        //Accessor of List of Sensors for SecuritySystem
        //-------------------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------------------
        //Accessor of List of Alarms for SecuritySystem
        //-------------------------------------------------------------------------------------------------------
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
