using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mes
{
    class Logger
    {
        // Log file and directory attributes
        string logFileName = "MesLog.txt";
        string logFilePath = System.IO.Directory.GetCurrentDirectory() + "\\Log\\";

        // Method to append to log file.
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
