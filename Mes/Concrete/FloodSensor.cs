using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Flood Sensor Class - Implements the default constructor of a flood sensor
    * ===========================================================================
    */
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
