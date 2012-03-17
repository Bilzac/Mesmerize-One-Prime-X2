using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class LightAlarm : Alarm
    {
        public LightAlarm()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "light";
            alarmId = 0;
        }
    }
}
