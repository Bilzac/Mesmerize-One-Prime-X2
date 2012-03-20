using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mes
{
    class Logger
    {

        string logFileName = "MesLog.txt";
        string logFilePath = System.IO.Directory.GetCurrentDirectory() + "\\Log\\";

        public string formatSensorMessage(SensorMessage sensorMsg)
        {
            string retVal = "<Sending Message for sensor> Sensor ID: " + sensorMsg.sensorId + " Sensor Type: " + sensorMsg.messageType + " Sensor Message: " + sensorMsg.messageString + " Sensor Value: " + sensorMsg.messageValue;
            return retVal;
        }

        public void appendLog(string Message)
        {
            string logFile = logFilePath + logFileName;
            string timeStamp = DateTime.Now.ToString();
            StreamWriter logWriter;

            if (!Directory.Exists(logFilePath)) {
                Directory.CreateDirectory(logFilePath);
            }

            if (!File.Exists(logFile)) {
                FileStream logStream = File.Create(logFile);
                logStream.Close();
            }

            try
            {
                logWriter = File.AppendText(logFile);
                logWriter.WriteLine(timeStamp + " - " + Message);
                logWriter.Flush();
                logWriter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(Message);
                Console.WriteLine(e.Message.ToString());
            }
        }
    }
}
