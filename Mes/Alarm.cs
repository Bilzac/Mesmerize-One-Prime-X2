using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{

    abstract public class Alarm
    {

        public bool isEnabled;
        public bool isTriggered;
        public string location;
        public string sensorType;
        public int alarmId;
        

        public virtual void Enable()
        {
            isEnabled = true;
        }

        public virtual void Disable()
        {
            isEnabled = false;
        }

        public virtual void Trigger()
        {
            isTriggered = true;
        }

        public virtual void Untrigger()
        {
            isTriggered = false;
        }

        public virtual bool IsEnabled()
        {
            return isEnabled;
        }

        public virtual void SendEvent()
        {

        }

        public void ReceiveEvent()
        {

        }

    }
}
