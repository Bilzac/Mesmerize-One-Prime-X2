using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mes
{
    /*============================================================================
    * Light Alarm Class - Implements the default constructor of a light alarm
    * ===========================================================================
    */
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
