using System;
using System.Collections.Generic;
using System.Messaging;
namespace Mes
{
    abstract public class Monitor
    {
        bool isEnabled;             // The monitor can be triggered
        bool isTriggered;           // Triggered state indicates the monitor starts to monitor.
        string location;            // what room is the component in
        string type;                // Is this a video camera or infrared camera etc.?
        int id;                     // unique ID of the object instance
        int parentId;               // refers to the parent system (default security)
        int monitorValue;           // A value used for a monitor

        //Observer pattern event listener objects
        public delegate void OnEnableEventHandler();
        public event EventHandler EnableChanged;
        public event EventHandler TriggerChanged;

        public Monitor()
        {
           observers = new List<IObserver<Monitor>>();
        }

        private List<IObserver<Monitor>> observers;
         
        //Enables the monitor
        public void Enable()
        {
            if (!isEnabled) {
                isEnabled = true;
                if (this.EnableChanged != null)
                    this.EnableChanged(this, new EventArgs());

                if (MessageQueue.Exists(GlobalVariables.queueName))
                {
                    MessageQueue queue = new MessageQueue(GlobalVariables.queueName);

                    // log the enable event
                    Mes.MesMessage message = new Mes.MesMessage();
                    message.type = "LOG";
                    message.message = string.Format("{0} Monitor {1} was enabled.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        // Disables the monitor
        public void Disable()
        {
            if (isEnabled)
            {
                isEnabled = false;
                if (this.EnableChanged != null)
                    this.EnableChanged(this, new EventArgs());

                if (MessageQueue.Exists(GlobalVariables.queueName))
                {
                    MessageQueue queue = new MessageQueue(GlobalVariables.queueName);

                    // log the enable event
                    Mes.MesMessage message = new Mes.MesMessage();
                    message.type = "LOG";
                    message.message = string.Format("{0} Monitor {1} was disabled.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }
        // Triggers the monitor
        public void Trigger()
        {
            if (!isTriggered)
            {
                isTriggered = true;
                // Call event handler is fired if it exists
                if (this.TriggerChanged != null)
                    this.TriggerChanged(this, new EventArgs());

                if (MessageQueue.Exists(GlobalVariables.queueName)){
                    MessageQueue queue = new MessageQueue(GlobalVariables.queueName);

                    // log the trigger event
                    Mes.MesMessage message = new Mes.MesMessage();
                    message.type = "LOG";
                    message.message = string.Format("{0} Monitor {1} was triggered.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }
        // Untriggers the monitor
        public void Untrigger()
        {
            if (isTriggered)
            {
                isTriggered = false;
                if (this.TriggerChanged != null)
                    this.TriggerChanged(this, new EventArgs());

                if (MessageQueue.Exists(GlobalVariables.queueName))
                {
                    MessageQueue queue = new MessageQueue(GlobalVariables.queueName);

                    // log the enable event
                    Mes.MesMessage message = new Mes.MesMessage();
                    message.type = "LOG";
                    message.message = string.Format("{0} Monitor {1} was untriggered.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        //********************* ACCESSORS *******************//

        // Monitor type Accessor
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        // Monitor ID Accessor
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public bool IsTriggered
        {
            get { return isTriggered; }
        }

        public String Location
        {
            get { return location; }
            set { location = value; }
        }

        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        public int MonitorValue
        {
            get { return monitorValue; }
            set { monitorValue = value; }
        }
    }
}
