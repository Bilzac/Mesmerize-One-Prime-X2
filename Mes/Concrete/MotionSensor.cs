using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class MotionSensor : Sensor
    {
        public MotionSensor()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "motion";
            canTrigger = true;
            sensorId = 0;
        }
    }
}
