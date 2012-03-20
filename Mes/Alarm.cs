using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
namespace Mes
{
    abstract public class Alarm
    {
        bool isEnabled;         // The alarm can be triggered
        bool isTriggered;       // Triggered state indicates the alarm is ringing/blinking/etc.
        string location;        // what room is the component in
        string type;            // Is this alarm a siren or light, etc.?
        int id;                 // unique ID of the object instance
        int parentId;           // refers to the parent system (default security)
        int sensitivity;        // The intensity of siren volume or light lumines

        // Observer Pattern for systems to subscribe to
        public delegate void OnEnableEventHandler();
        public event EventHandler EnableChanged;
        public event EventHandler TriggerChanged;

        public Alarm()
        {
           observers = new List<IObserver<Alarm>>();
        }

        private List<IObserver<Alarm>> observers;

        // Enable the alarm and logs the event
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
                    message.message = string.Format("{0} Alarm {1} at {2} was enabled.", this.Type, this.Id, this.location);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        // Disables the alarm and logs the event
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
                    message.message = string.Format("{0} Alarm {1} at {2} was disabled.", this.Type, this.Id, this.location);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        // Triggers the alarm is enable and sends an event
        public void Trigger()
        {
            if (!isTriggered && isEnabled)
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
                    message.message = string.Format("{0} Alarm {1} at {2} was triggered.", this.Type, this.Id, this.location);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        // Untriggers an alarm if triggered and sends an event
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
                    message.message = string.Format("{0} Alarm {1} at {2} was untriggered.", this.Type, this.Id, this.location);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

        //********************* ACCESSORS *******************//

        // Alarm type Accessor
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        // Alarm ID Accessor
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

        public int Sensitivity
        {
            get { return sensitivity; }
            set { sensitivity = value; }
        }
    }
}
