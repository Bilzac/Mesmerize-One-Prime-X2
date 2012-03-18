using System;

namespace Mes
{
    abstract public class Monitor
    {
        bool isEnabled;
        bool isTriggered;
        string location;
        string sensorType;
        int monitorId;

        public event EventHandler EnableChanged;
        public event EventHandler TriggerChanged;

        public void Enable()
        {
            if (!isEnabled)
            {
                isEnabled = true;
                if (this.EnableChanged != null)
                    this.EnableChanged(this, new EventArgs());
            }
        }

        public void Disable()
        {
            if (isEnabled)
            {
                isEnabled = false;
                if (this.EnableChanged != null)
                    this.EnableChanged(this, new EventArgs());
            }
        }

        public void Trigger()
        {
            if (!isTriggered)
            {
                isTriggered = true;
                if (this.TriggerChanged != null)
                    this.TriggerChanged(this, new EventArgs());
            }
        }

        public void Untrigger()
        {
            if (isTriggered)
            {
                isTriggered = false;
                if (this.TriggerChanged != null)
                    this.TriggerChanged(this, new EventArgs());
            }
        }

        //********************* ACCESSORS *******************//

        // Sensor type Accessor
        public string Type
        {
            get
            {
                return sensorType;
            }
            set
            {
                sensorType = value;
            }
        }

        // Sensor ID Accessor
        public int Id
        {
            get
            {
                return monitorId;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
        }
        public void SendEvent()
        {
        }
        public void ReceiveEvent()
        {
        }
    }
}
