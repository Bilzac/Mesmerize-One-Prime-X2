using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    class SirenAlarm : Alarm
    {
        public SirenAlarm()
        {
            isEnabled = false;
            isTriggered = false;
            location = null;
            sensorType = "siren";
            alarmId = 0;
        }
    }
}
