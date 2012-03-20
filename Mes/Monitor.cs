using System;
using System.Collections.Generic;
using System.Messaging;
namespace Mes
{
    abstract public class Monitor
    {
        bool isEnabled;
        bool isTriggered;
        string location;
        string type;
        int id;
        int parentId;

        public delegate void OnEnableEventHandler();
        public event EventHandler EnableChanged;
        public event EventHandler TriggerChanged;

        public Monitor()
        {
           observers = new List<IObserver<Monitor>>();
        }

        private List<IObserver<Monitor>> observers;

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
                    message.type = "log";
                    message.message = string.Format("{0} Monitor {1} was enabled.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

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
                    message.type = "log";
                    message.message = string.Format("{0} Monitor {1} was disabled.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

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
                    message.type = "log";
                    message.message = string.Format("{0} Monitor {1} was triggered.", this.Type, this.Id);
                    queue.Send(message);

                    // Send a message to the security system to trigger all alarms in the same location.
                    message = new Mes.MesMessage();
                    message.type = "trigger";
                    message.message = string.Format(",,alarm,true,,{0}", this.Location);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
            }
        }

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
                    message.type = "log";
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
    }
}
