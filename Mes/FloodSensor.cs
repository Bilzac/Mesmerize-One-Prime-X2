using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class FloodSensor : Sensor
    {
        public FloodSensor()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "flood";
            canTrigger = true;
            sensorId = 0;
        }
    }
}
