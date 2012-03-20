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
            this.Disable();
            this.Untrigger();
            this.Type = "light";
        }
    }
}
