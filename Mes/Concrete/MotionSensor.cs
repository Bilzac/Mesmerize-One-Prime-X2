using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Motion Sensor Class - Implements the default constructor of a motion sensor
    * ===========================================================================
    */
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
