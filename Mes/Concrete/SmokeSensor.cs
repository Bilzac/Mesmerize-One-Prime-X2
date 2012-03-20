using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Smoke Sensor Class - Implements the default constructor of a smoke sensor
    * ===========================================================================
    */
    class SmokeSensor : Sensor
    {
        public SmokeSensor()
        {
            this.Disable();
            this.Untrigger();
            this.Type = "smoke";
        }
    }
}
