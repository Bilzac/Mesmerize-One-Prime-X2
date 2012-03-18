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
            this.Disable();
            this.Untrigger();
            this.Type = "magnetic";
        }
    }
}
