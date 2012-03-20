using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Magnetic Sensor Class - Implements the default constructor of a magnetic sensor
    * ===========================================================================
    */
    class MagneticSensor : Sensor
    {
        public MagneticSensor()
        {
            this.Disable();
            this.Untrigger();
            this.Type = "magnetic";
        }
    }
}
