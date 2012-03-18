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
            this.Disable();
            this.Untrigger();
            this.Type = "flood";
        }
    }
}
