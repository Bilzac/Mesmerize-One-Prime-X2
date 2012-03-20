using System;
using System.Collections.Generic;
using System.Messaging;
namespace Mes{
    public abstract class Sensor : IObservable<Sensor>
    {
        bool isEnabled;         // The sensor can be triggered
        bool isTriggered;       // Triggered state indicates the sensor was tripped
        string location;        // what room is the component in
        string sensorType;      // Is this flood,magnetic,motion,etc. sensor?
        int sensorId;           // unique id of the sensor
        int parentId;           // id of the system it is under (security)

        public delegate void OnEnableEventHandler();
        public event EventHandler EnableChanged;
        public event EventHandler TriggerChanged;

        public Sensor()
        {
           observers = new List<IObserver<Sensor>>();
        }

        private List<IObserver<Sensor>> observers;


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
                    message.message = string.Format("{0} Sensor {1} was enabled.", this.Type, this.Id);
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
                    message.message = string.Format("{0} Sensor {1} was disabled.", this.Type, this.Id);
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
                    message.message = string.Format("{0} Sensor {1} was triggered.", this.Type, this.Id);
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
                    message.message = string.Format("{0} Sensor {1} was untriggered.", this.Type, this.Id);
                    queue.Send(message);
                }
                else
                {
                    Console.WriteLine("Trigger - no Security message queue available");
                }
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
                return sensorId;
            }
            set
            {
                sensorId = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }

        public bool IsTriggered
        {
            get
            {
                return isTriggered;
            }
        }

        public String Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public int ParentId
        {
            get
            {
                return parentId;
            }
            set
            {
                parentId = value;
            }
        }

        public IDisposable Subscribe(IObserver<Sensor> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<Sensor>> _observers;
            private IObserver<Sensor> _observer;

            public Unsubscriber(List<IObserver<Sensor>> observers, IObserver<Sensor> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}