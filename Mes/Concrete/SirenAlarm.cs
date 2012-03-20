using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Siren Alarm Class - Implements the default constructor of a siren alarm
    * ===========================================================================
    */
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
