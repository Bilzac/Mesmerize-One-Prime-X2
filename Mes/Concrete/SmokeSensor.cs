using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
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
