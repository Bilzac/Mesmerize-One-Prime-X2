using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class SmokeSensor : Sensor
    {
        public SmokeSensor()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "smoke";
            canTrigger = true;
            sensorId = 0;
        }
    }
    
}
