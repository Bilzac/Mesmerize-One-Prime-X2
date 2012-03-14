using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class MagneticSensor : Sensor
    {
        public MagneticSensor()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "magnetic";
            canTrigger = true;
            sensorId = 0;
        }
    }
}
