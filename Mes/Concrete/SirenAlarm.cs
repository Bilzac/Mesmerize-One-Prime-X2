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
            this.Disable();
            this.Untrigger();
            this.Type = "siren";
        }
    }
}
