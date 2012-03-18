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
            this.Disable();
            this.Untrigger();
            this.Type = "motion";
        }
    }
}
