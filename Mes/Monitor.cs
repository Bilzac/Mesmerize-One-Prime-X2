using System;

namespace Mes
{
    abstract public class Monitor
    {
        public bool isEnabled;
        public bool isTriggered;
        public string location;
        public string sensorType;
        public int monitorId;

        public void SendEvent()
        {
        }
        public void ReceiveEvent()
        {
        }
    }
}
