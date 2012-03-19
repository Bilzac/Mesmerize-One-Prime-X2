using System;
using System.Collections.Generic;

namespace Mes{
    public abstract class Sensor : IObservable<Sensor>
    {
        bool isEnabled;
        bool isTriggered;
        string location;
        string sensorType;
        bool canTrigger;
        int sensorId;
        int parentId;

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